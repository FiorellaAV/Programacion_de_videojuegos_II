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

    // Dash variables
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    private bool isDashing = false;
    private float dashCooldown = 1f;
    private float lastDashTime = -10f;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerModel>();
        pv = GetComponent<PlayerView>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
        pv.Inicio();
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
            pv.Saltar(); // Llamar al método de salto en PlayerView
        }
    }

    void Update()
    {
        // Manejar disparos y dibujar rayos de depuración
        pm.HandleShooting(rayDistance, lr);
        pm.DrawDebugRayFromPlayer(rayDistance);
        pm.HandleDash(rb, dashDistance, dashDuration, isDashing, dashCooldown, lastDashTime);

        pv.VerificarSalto(); // Verificar si el jugador está en el suelo y desactivar la animación de salto
    }

}