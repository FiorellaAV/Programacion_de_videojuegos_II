using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPresenter : MonoBehaviour
{
    private EnemyView enemyView;
    NavMeshAgent agent;
    private Transform player;      // Asignar desde el Inspector
    // public float moveSpeed = 3f;   // Velocidad de movimiento
    public EnemyData enemyData;

    private bool isWaiting;
    //Metodo para pasarle el player a enemy y pueda seguirlo con move

    //  Inicialización de referencias internas
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyView = GetComponent<EnemyView>();
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
        else
        {
            agent.isStopped = true;
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

    public void Attack()
    {
        enemyView.Attack();
        StartCoroutine(WaitAfterHit());
    }

    public IEnumerator WaitAfterHit()
    {
        isWaiting = true;
        //enemyView.WaitAttack();
        yield return new WaitForSeconds(enemyData.AttackDelay);
        isWaiting = false;
        agent.isStopped = false;
        //enemyView.Walk();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPresenter playerPresenter = collision.gameObject.GetComponent<PlayerPresenter>();
            this.Attack();
            playerPresenter.ReceiveDamage(enemyData.Damage);
        }
    }
}
