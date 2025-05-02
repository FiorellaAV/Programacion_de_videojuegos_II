using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    public void MovePlayer(float moveSpeed, Rigidbody rb)
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public bool IsGrounded(Transform groundCheck, float groundCheckRadius, LayerMask groundLayer)
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void RotateTowardsMouse(Camera mainCamera, Rigidbody rb)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = (point - transform.position);
            direction.y = 0f; // mantenerlo en el plano horizontal y solo gire izquierda-derecha (no arriba y abajo)

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(rotation);
            }
        }
    }
    

    public void HandleShooting(float distance, LineRenderer lineRenderer)
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Vector3 origin = transform.position + Vector3.up * 1f; // opcional: levantarlo un poco si querés
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                Debug.Log("Disparaste a: " + hit.collider.name);
                DrawShotLine(origin, hit.point, lineRenderer);

                // Aca agregar efectos visuales o daño, por ejemplo:
                if (hit.collider.CompareTag("Enemy")) 
                {
                    // ESto se va a cambiar por pool de Enemigos mas adelante
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {
                Debug.Log("No se golpeó nada.");
            }
        }
    }    

    public void DrawDebugRayFromPlayer(float distance)
    {
        Vector3 origin = transform.position + Vector3.up; 
        Vector3 direction = transform.forward;

        Debug.DrawRay(origin, direction * distance, Color.green);
    }

    private void DrawShotLine(Vector3 start, Vector3 end, LineRenderer lineRenderer)
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Mostrar la línea por x segundos NO DEJAR HARDCODEADO, ahora es para pruebas
        StartCoroutine(ClearLineAfterDelay(0.05f, lineRenderer));
    }

    private IEnumerator ClearLineAfterDelay(float delay, LineRenderer lineRenderer)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.positionCount = 0;
    }

}
