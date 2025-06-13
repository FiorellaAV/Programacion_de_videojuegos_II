using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class EnemyLizardController : MonoBehaviour
{
    private EnemyLizardView ev;
    NavMeshAgent agent;
    public Transform player;      // Asignar desde el Inspector
    public float moveSpeed = 2f;  // Velocidad de movimiento
    public float damage = 20f;
    public float reachDistance = 2f; // Distancia para considerar que "alcanzó" al jugador

    private bool isWaiting;
    //Metodo para pasarle el player a enemy y pueda seguirlo con move

    //  Inicialización de referencias internas
    void Awake()
    {
        ev = GetComponent<EnemyLizardView>();
        isWaiting = false;

    }

    public void Initialize(Transform playerTarget)
    {
        player = playerTarget;

    }

    void Update()
    {
        if (!isWaiting)
        {
            Move();

            PlayerIsReached();

        }
    }

    public void Move()
    {
        if (player != null)
        {
            if (player == null)
            {
                UnityEngine.Debug.Log("no hay player");
            }
            agent = GetComponent<NavMeshAgent>();

            // Se agrego validacion de navmesh
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(player.position);
            }
            else
            {
                UnityEngine.Debug.LogWarning("El agente no está sobre un NavMesh.");
            }
            // agent.SetDestination(player.position);
            // Calcula direcci�n hacia el player
            /*Vector3 direction = (player.position - transform.position).normalized;

            // Mueve al enemigo hacia el player
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Opcional: rotarlo hacia el player
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }*/
        }
    }

    public void PlayerIsReached()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            //UnityEngine.Debug.Log($"Distancia al jugador: {distanceToPlayer}");

            if (distanceToPlayer <= reachDistance)

            {
                // UnityEngine.Debug.Log("El enemigo alcanzó al jugador.");
                PlayerModel playerModel = player.GetComponent<PlayerModel>();
                if (playerModel == null)
                {
                    playerModel = player.GetComponentInParent<PlayerModel>();
                }
                if (playerModel == null)
                {
                    playerModel = player.GetComponentInChildren<PlayerModel>();
                }

                if (playerModel != null)
                {
                    ev.Attack();
                    playerModel.TakeDamage(damage);
                    // UnityEngine.Debug.Log("Jugador alcanzado y dañado");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"No se encontró PlayerModel en el objeto {player.name} ni en sus hijos/padres.");
                }

                StartCoroutine(WaitAfterHit(2f));
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("La referencia a player es null.");
        }
    }

    private IEnumerator WaitAfterHit(float delay)
    {
        isWaiting = true;
        ev.WaitAttack();
        yield return new WaitForSeconds(delay);
        isWaiting = false;
    }
}
