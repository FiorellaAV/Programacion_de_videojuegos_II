using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private PlayerModel pm;
    private PlayerView pv;
    private LineRenderer lr;
    private Camera mainCamera;

    public LayerMask groundLayer;
    public Transform groundCheck;

    public float moveSpeed = 5f;
    public float jumpForce = 50f;
    public float rayDistance = 100f;
    public float groundCheckRadius = 0.3f;

    // Dash variables
    private float lastDashTime = -Mathf.Infinity;
    private bool isDashing = false;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    private float dashCooldown = 2f;

    //  Inicializaci�n de referencias internas
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerModel>();
        pv = GetComponent<PlayerView>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }

    //  L�gica de arranque
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pv.Start(); // Si PlayerView tiene su propio Start(), llamalo expl�citamente
    }

    void FixedUpdate()
    {

        if (pm.health <= 0f)
        {
            return; // Salir si el jugador est� muerto
        }

        // Mover al jugador
        MovePlayer(moveSpeed, rb);

        // Rotar al jugador hacia el mouse
        pm.RotateTowardsMouse(mainCamera, rb);

        // Saltar si se presiona la barra espaciadora
        if (Input.GetKey(KeyCode.Space) && pm.IsGrounded(groundCheck, groundCheckRadius, groundLayer))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            pv.Jump(); // Llamar al m�todo de salto en PlayerView
        }
    }

    void Update()
    {
        if (pm.health <= 0f)
        {
            return; // Salir si el jugador est� muerto
        }

        // Manejar disparos y dibujar rayos de depuraci�n
        HandleShooting(rayDistance, lr);
        pm.DrawDebugRayFromPlayer(rayDistance);
        HandleDash(this, rb, dashDistance, dashDuration, dashCooldown);


        pv.VerifyJump(); // Verificar si el jugador est� en el suelo y desactivar la animaci�n de salto
    }



    // MANEJO DE INPUTS

    // Movements
    public void MovePlayer(float moveSpeed, Rigidbody rb)
    {
        //Para que el jugador deje de moverse si el juego termin�
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        // Obtener entrada del jugador
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Crear vector de movimiento
        Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

        // Mover al jugador
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        // Actualizar animaciones
        pv.AimToMouse(moveX, moveZ);
    }

    public void HandleShooting(float distance, LineRenderer lineRenderer)
    {
        //Para que el jugador deje de disparar si el juego termin�
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f; // opcional: levantarlo un poco si quer�s (tener en cuenta la altura del barril)
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                UnityEngine.Debug.Log("Disparaste a: " + hit.collider.name);
                pm.DrawShotLine(origin, hit.point, lineRenderer);

                // Agregar efectos visuales o da�o
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject bloodVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Blood Splash"), hit.collider.transform.position, Quaternion.identity);
                    GameObject.Destroy(bloodVFX, 1f); // Destruir efecto tras 1s

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EnemyKilled();
                    }
                    if (hit.collider.gameObject.GetComponent<EnemyModel>() != null) hit.collider.gameObject.GetComponent<EnemyModel>().TakeDamage();
                    if (hit.collider.gameObject.GetComponent<EnemyLizardModel>() != null) hit.collider.gameObject.GetComponent<EnemyLizardModel>().TakeDamage();
                }

                if (hit.collider.CompareTag("Barrel"))
                {
                    Vector3 explosionPos = hit.collider.transform.position;
                    Destroy(hit.collider.gameObject);
                    float explosionRadius = 5f; // ajust� seg�n tu necesidad
                    float explosionDamage = 200f;

                    pm.Explode(explosionPos, explosionRadius, explosionDamage);
                }
            }
            else
            {
                UnityEngine.Debug.Log("No se golpe� nada.");
            }
        }
    }
    // Dash
    public void HandleDash(MonoBehaviour caller, Rigidbody rb, float dashDistance, float dashDuration, float dashCooldown)
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Ejecutar dash si no est� en cooldown
            if (Time.time < lastDashTime + dashCooldown || isDashing)
                return;

            lastDashTime = Time.time;
            pm.isDashing = true;

            caller.StartCoroutine(pm.Dash(rb, dashDistance, dashDuration));
        }
    }

    //private IEnumerator Dash(Rigidbody rb, float dashDistance, float dashDuration)
    //{
    //    Vector3 dashDirection = transform.forward;
    //    float maxDashDistance = dashDistance;

    //    // Verificar colisi�n con raycast
    //    if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dashDirection, out RaycastHit hit, dashDistance))
    //    {
    //        // Ajustar distancia m�xima hasta el obst�culo, dejando un peque�o margen
    //        maxDashDistance = hit.distance - 0.1f;
    //    }

    //    float elapsed = 0f;
    //    float actualDashDuration = dashDuration * (maxDashDistance / dashDistance); // Ajustar duraci�n si distancia es menor

    //    while (elapsed < actualDashDuration)
    //    {
    //        rb.MovePosition(rb.position + dashDirection * (maxDashDistance / actualDashDuration) * Time.fixedDeltaTime);
    //        elapsed += Time.fixedDeltaTime;
    //        yield return new WaitForFixedUpdate();
    //    }

    //    isDashing = false;
    //}


}