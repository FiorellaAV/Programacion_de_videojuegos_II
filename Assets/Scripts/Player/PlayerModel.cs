using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private Animator animator;

    public void MovePlayer(float moveSpeed, Rigidbody rb)
    {
        // Obtener entrada del jugador
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Crear vector de movimiento
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Mover al jugador
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Actualizar animaciones
        UpdateAnimationParameters(moveX, moveZ);
    }

    private void UpdateAnimationParameters(float moveX, float moveZ)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Crear el vector de movimiento en el espacio global
        Vector3 globalMovement = new Vector3(moveX, 0f, moveZ);

        // Transformar el vector de movimiento al espacio local del personaje
        Vector3 localMovement = transform.InverseTransformDirection(globalMovement);

        // Si hay movimiento, actualizar aim_x y aim_y en el espacio local
        if (localMovement != Vector3.zero)
        {
            animator.SetFloat("aim_x", localMovement.x);
            animator.SetFloat("aim_y", localMovement.z);
        }
        else
        {
            // Si no hay movimiento, establecer ambos en 0
            animator.SetFloat("aim_x", 0f);
            animator.SetFloat("aim_y", 0f);
        }
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
            direction.y = 0f; // Mantenerlo en el plano horizontal (ignorar la altura)

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);

                // Ajustar la rotación con un offset para compensar la orientación inicial
                Quaternion offset = Quaternion.Euler(0f, 0f, 0f);
                rb.MoveRotation(rotation * offset);
            }
        }
    }

    public void HandleShooting(float distance, LineRenderer lineRenderer)
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Vector3 origin = transform.position + Vector3.up * 1.5f; // opcional: levantarlo un poco si querés
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                Debug.Log("Disparaste a: " + hit.collider.name);
                DrawShotLine(origin, hit.point, lineRenderer);

                // Agregar efectos visuales o daño
                if (hit.collider.CompareTag("Enemy"))
                {
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

        StartCoroutine(ClearLineAfterDelay(0.05f, lineRenderer));
    }

    private IEnumerator ClearLineAfterDelay(float delay, LineRenderer lineRenderer)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.positionCount = 0;
    }
}