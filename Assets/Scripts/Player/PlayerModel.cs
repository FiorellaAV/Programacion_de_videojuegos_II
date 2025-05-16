using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : MonoBehaviour
{

    private PlayerView pv;
    private Animator animator;

    public float health = 200f;


    void Awake()
    {
        pv = GetComponent<PlayerView>();
    }



    public void MovePlayer(float moveSpeed, Rigidbody rb)
    {
        //Para que el jugador deje de moverse si el juego termino
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

                // Ajustar la rotaci�n con un offset para compensar la orientaci�n inicial
                Quaternion offset = Quaternion.Euler(0f, 0f, 0f);
                rb.MoveRotation(rotation * offset);
            }
        }
    }

    public void HandleShooting(float distance, LineRenderer lineRenderer)
    {
        //Para que el jugador deje de disparar si el juego termino
        if (GameManager.Instance != null && GameManager.Instance.gameEnded)
            return;

        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f; // opcional: levantarlo un poco si quer�s (tener en cuenta la altura del barril)
            Vector3 direction = transform.forward;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
            {
                UnityEngine.Debug.Log("Disparaste a: " + hit.collider.name);
                DrawShotLine(origin, hit.point, lineRenderer);

                // Agregar efectos visuales o da�o
                if (hit.collider.CompareTag("Enemy"))
                {
                    GameObject bloodVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Blood Splash"), hit.collider.transform.position, Quaternion.identity);
                    GameObject.Destroy(bloodVFX, 1f); // Destruir efecto tras 2s

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EnemyKilled();
                    }
                    Destroy(hit.collider.gameObject);
                }

                if (hit.collider.CompareTag("Barrel"))
                {
                    Vector3 explosionPos = hit.collider.transform.position;
                    Destroy(hit.collider.gameObject);

                    float explosionRadius = 5f; // ajust� seg�n tu necesidad
                    float explosionDamage = 200f;

                    Explode(explosionPos, explosionRadius, explosionDamage);
                }
            }
            else
            {
                UnityEngine.Debug.Log("No se golpe� nada.");
            }
        }
    }



    private void Explode(Vector3 position, float radius, float damage)
    {
        // Efecto visual (opcional)
        GameObject explosionVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Explosion Fire"), position, Quaternion.identity);
        GameObject.Destroy(explosionVFX, 2f); // Destruir efecto tras 2s

        //UnityEngine.Debug.DrawLine(position, position + Vector3.up * 2, Color.red, 1f);

        // Detectar enemigos en �rea
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (Collider nearby in colliders)
        {
            if (nearby.CompareTag("Enemy"))
            {
                // Aqu� podr�as aplicar da�o si ten�s una clase Enemy con TakeDamage
                // Ejemplo:
                // nearby.GetComponent<Enemy>().TakeDamage(damage);
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EnemyKilled();
                }
                GameObject.Destroy(nearby.gameObject); // Temporal: destruir directamente
            }
            if (nearby.CompareTag("Player"))
            {
                // Aqu� podr�as aplicar da�o si ten�s una clase Enemy con TakeDamage
                // Ejemplo:
                nearby.GetComponent<PlayerModel>().TakeDamage(damage);
            }
        }
    }

    // Dash
    public IEnumerator Dash(Rigidbody rb, float dashDistance, float dashDuration, bool isDashing, float dashCooldown, float lastDashTime)
    {
        //// Verificar cooldown
        //if (Time.time < lastDashTime + dashCooldown || isDashing)
        //    yield break;

        //isDashing = true;
        //lastDashTime = Time.time;

        //float elapsed = 0f;
        //Vector3 dashDirection = transform.forward;

        //while (elapsed < dashDuration)
        //{
        //    rb.MovePosition(rb.position + dashDirection * (dashDistance / dashDuration) * Time.fixedDeltaTime);
        //    elapsed += Time.fixedDeltaTime;
        //    yield return new WaitForFixedUpdate();
        //}

        //isDashing = false;

        // Verificar cooldown
        if (Time.time < lastDashTime + dashCooldown || isDashing)
            yield break;

        isDashing = true;
        lastDashTime = Time.time;

        Vector3 dashDirection = transform.forward;
        float maxDashDistance = dashDistance;

        // Verificar colisi�n con raycast
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dashDirection, out RaycastHit hit, dashDistance))
        {
            // Ajustar distancia m�xima hasta el obst�culo, dejando un peque�o margen
            maxDashDistance = hit.distance - 0.1f;
        }

        float elapsed = 0f;
        float actualDashDuration = dashDuration * (maxDashDistance / dashDistance); // Ajustar duraci�n si distancia es menor

        while (elapsed < actualDashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * (maxDashDistance / actualDashDuration) * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void HandleDash(Rigidbody rb, float dashDistance, float dashDuration, bool isDashing, float dashCooldown, float lastDashTime)
    {
        if (Input.GetMouseButtonDown(1))
        {
            // Ejecutar dash
            StartCoroutine(Dash(rb, dashDistance, dashDuration, isDashing, dashCooldown, lastDashTime));

        }
    }

    public void DrawDebugRayFromPlayer(float distance)
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        UnityEngine.Debug.DrawRay(origin, direction * distance, Color.green);
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


    public void TakeDamage(float amount)
    {
        health -= amount;
        UnityEngine.Debug.Log("Player recibi� da�o. Salud restante: " + health);
        if (health <= 0)
        {
            pv.Die();
            Die();
        }
    }

    private void Die()
    {
        UnityEngine.Debug.Log("�El jugador ha muerto!");
        // Pod�s agregar animaciones, sonido, pantalla de derrota, etc.
        StartCoroutine(DieAfterDelay(2f)); // Espera 2 segundos
    }

    private IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Destroy(gameObject);
        // Llama a GameOver
        GameManager.Instance.GameOver();
        // Reinicio la escena?
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public float GetHealth()
    {
       return health;
    }
}