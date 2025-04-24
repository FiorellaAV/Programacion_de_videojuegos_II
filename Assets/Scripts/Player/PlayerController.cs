using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private Camera mainCamera;

    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;


    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    //Uso fixedUpdate en lugar de Update porque es mejor para el tema de fisicas, ya que se ejecuta por defecto cada 0.02s     
    void FixedUpdate()
    {
        MovePlayer();
        RotateTowardsMouse();
        if (Input.GetKey(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        HandleShooting();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }


    void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Vector3 direction = (point - transform.position);
            direction.y = 0f; // mantenerlo en el plano horizontal

            if (direction != Vector3.zero)
            {
                Quaternion rotation = Quaternion.LookRotation(direction);
                rb.MoveRotation(rotation);
            }
        }
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                Debug.Log("Disparaste a: " + hit.collider.name);

                // Aca agregar efectos visuales o daño, por ejemplo:
                // if (hit.collider.CompareTag("Enemy")) { ... }
            }
            else
            {
                Debug.Log("No se golpeó nada.");
            }
        }
    }
}