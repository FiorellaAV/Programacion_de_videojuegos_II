using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private EnemyView ev;
    NavMeshAgent agent;
    private Transform player;      // Asignar desde el Inspector
    // public float moveSpeed = 3f;   // Velocidad de movimiento
    public EnemyData enemyData;

    private bool isWaiting;
    //Metodo para pasarle el player a enemy y pueda seguirlo con move

    //  Inicialización de referencias internas
    void Awake()
    {
        ev = GetComponent<EnemyView>();
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
        }
    }

    public IEnumerator WaitAfterHit()
    {
        isWaiting = true;
        ev.WaitAttack();
        yield return new WaitForSeconds(enemyData.AttacDelay);
        isWaiting = false;
    }
}
