using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : MonoBehaviour
{
    private PlayerView playerView;

    public float health = 200f;
    private float initialHealth = 200f;

    private EnemySpawner spawner;
    private ObjectPool pool;

    public bool isDashing = false;

    void Awake()
    {
        playerView = GetComponent<PlayerView>();
    }

    private void Start()
    {
        spawner = EnemySpawner.Instance;
        pool = spawner.GetComponent<ObjectPool>();
        if (pool != null) UnityEngine.Debug.Log("Habemus Pool en PlayerModel");
    }

    //public void MovePlayer(float moveSpeed, Rigidbody rb)
    //{
    //    //Para que el jugador deje de moverse si el juego terminó
    //    if (GameManager.Instance != null && GameManager.Instance.gameEnded)
    //        return;

    //    // Obtener entrada del jugador
    //    float moveX = Input.GetAxis("Horizontal");
    //    float moveZ = Input.GetAxis("Vertical");

    //    // Crear vector de movimiento
    //    Vector3 movement = new Vector3(moveX, 0f, moveZ).normalized;

    //    // Mover al jugador
    //    rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

    //    // Actualizar animaciones
    //    playerView.AimToMouse(moveX, moveZ);
    //}

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

    //public void HandleShooting(float distance, LineRenderer lineRenderer)
    //{
    //    //Para que el jugador deje de disparar si el juego terminó
    //    if (GameManager.Instance != null && GameManager.Instance.gameEnded)
    //        return;

    //    if (Input.GetMouseButtonDown(0)) // Click izquierdo
    //    {
    //        Vector3 origin = transform.position + Vector3.up * 0.5f; // opcional: levantarlo un poco si querés (tener en cuenta la altura del barril)
    //        Vector3 direction = transform.forward;

    //        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
    //        {
    //            UnityEngine.Debug.Log("Disparaste a: " + hit.collider.name);
    //            DrawShotLine(origin, hit.point, lineRenderer);

    //            // Agregar efectos visuales o daño
    //            if (hit.collider.CompareTag("Enemy"))
    //            {
    //                GameObject bloodVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Blood Splash"), hit.collider.transform.position, Quaternion.identity);
    //                GameObject.Destroy(bloodVFX, 1f); // Destruir efecto tras 1s

    //                if (GameManager.Instance != null)
    //                {
    //                    GameManager.Instance.EnemyKilled();
    //                }
    //                hit.collider.gameObject.GetComponent<EnemyModel>().TakeDamage();
    //            }

    //            if (hit.collider.CompareTag("Barrel"))
    //            {
    //                Vector3 explosionPos = hit.collider.transform.position; 
    //                Destroy(hit.collider.gameObject);
    //                float explosionRadius = 5f; // ajustá según tu necesidad
    //                float explosionDamage = 200f;

    //                Explode(explosionPos, explosionRadius, explosionDamage);
    //            }
    //        }
    //        else
    //        {
    //            UnityEngine.Debug.Log("No se golpeó nada.");
    //        }
    //    }
    //}

    public void Explode(Vector3 position, float radius, float damage)
    {
        // Efecto visual (opcional)
        GameObject explosionVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Explosion Fire"), position, Quaternion.identity);
        GameObject.Destroy(explosionVFX, 2f); // Destruir efecto tras 2s

        //UnityEngine.Debug.DrawLine(position, position + Vector3.up * 2, Color.red, 1f);

        // Detectar enemigos en área
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        foreach (Collider nearby in colliders)
        {
            if (nearby.CompareTag("Enemy"))
            {
                EnemyModel enemyModel = nearby.GetComponent<EnemyModel>();
                if (enemyModel != null)
                {
                    enemyModel.TakeDamage(); // Llama al método correctamente

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.EnemyKilled();
                    }
                }
                else
                {

                    EnemyLizardModel enemyLizardModel = nearby.GetComponent<EnemyLizardModel>();
                    if (enemyLizardModel != null)
                    {
                        enemyLizardModel.TakeDamage(); // Llama al método correctamente

                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.EnemyKilled();
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("El enemigo no tiene componente EnemyModel.");
                    }
                }
            }
            if (nearby.CompareTag("Player"))
            {
                // Acá podrías aplicar daño si tenés una clase Enemy con TakeDamage
                // Ejemplo:
                nearby.GetComponent<PlayerModel>().TakeDamage(damage);
            }
        }
    }

    //// Dash
    //public void HandleDash(MonoBehaviour caller, Rigidbody rb, float dashDistance, float dashDuration, float dashCooldown)
    //{
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        // Ejecutar dash si no está en cooldown
    //        if (Time.time < lastDashTime + dashCooldown || isDashing)
    //            return;

    //        lastDashTime = Time.time;
    //        isDashing = true;

    //        caller.StartCoroutine(Dash(rb, dashDistance, dashDuration));
    //    }
    //}

    public IEnumerator Dash(Rigidbody rb, float dashDistance, float dashDuration)
    {
        Vector3 dashDirection = transform.forward;
        float maxDashDistance = dashDistance;

        // Verificar colisión con raycast
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dashDirection, out RaycastHit hit, dashDistance))
        {
            // Ajustar distancia máxima hasta el obstáculo, dejando un pequeño margen
            maxDashDistance = hit.distance - 0.1f;
        }

        float elapsed = 0f;
        float actualDashDuration = dashDuration * (maxDashDistance / dashDistance); // Ajustar duración si distancia es menor

        while (elapsed < actualDashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * (maxDashDistance / actualDashDuration) * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void DrawDebugRayFromPlayer(float distance)
    {
        Vector3 origin = transform.position + Vector3.up;
        Vector3 direction = transform.forward;

        UnityEngine.Debug.DrawRay(origin, direction * distance, Color.green);
    }

    public void DrawShotLine(Vector3 start, Vector3 end, LineRenderer lineRenderer)
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
        // UnityEngine.Debug.Log("Player recibió daño. Salud restante: " + health);
        if (health <= 0)
        {
            playerView.Die();
            die();
        }
    }

    private void die()
    {
        UnityEngine.Debug.Log("¡El jugador ha muerto!");
        StartCoroutine(DieAfterDelay(2f)); // Espera 2 segundos
    }

    private IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Llama a GameOver
        GameManager.Instance.GameOver();
    }

    public float GetHealth()
    {
        return health;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("HealthKit"))
        {
            if (health < initialHealth)
            {
                heal(other.GetComponent<HealthKit>().GetHealthGiven());
                Destroy(other.gameObject);
            }      
        }
    }

    void heal(float amount)
    {
        health += amount;
        if(health > initialHealth)
        {
            health = initialHealth;
        }
    }
}