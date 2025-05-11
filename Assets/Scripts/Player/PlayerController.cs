using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private PlayerModel pm;
    private PlayerView pv;
    private Camera mainCamera;
    private LineRenderer lr;

    public float jumpForce = 50f;
    public float rayDistance = 100f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;

    private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerModel>();
        pv = GetComponent<PlayerView>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }

    void FixedUpdate()
    {
        // Mover al jugador
        pm.MovePlayer(moveSpeed, rb);

        // Rotar al jugador hacia el mouse
        pm.RotateTowardsMouse(mainCamera, rb);

        // Saltar si se presiona la barra espaciadora
        if (Input.GetKey(KeyCode.Space) && pm.IsGrounded(groundCheck, groundCheckRadius, groundLayer))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetBool("is_jumping", true); // Activar la animación de salto
        }
    }

    void Update()
    {
        // Manejar disparos y dibujar rayos de depuración
        pm.HandleShooting(rayDistance, lr);
        pm.DrawDebugRayFromPlayer(rayDistance);

        // Verificar si la animación de salto ha terminado
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("is_jumping", false); // Desactivar la animación de salto
        }
    }

}