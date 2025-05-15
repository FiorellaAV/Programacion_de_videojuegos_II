using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;      // Asignar desde el Inspector
    public float moveSpeed = 3f;  // Velocidad de movimiento
    public float damage = 10f;
    public float reachDistance = 2f; // Distancia para considerar que "alcanzó" al jugador

    private bool isWaiting = false;
    //Metodo para pasarle el player a enemy y pueda seguirlo con move
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
            // Calcula direcci�n hacia el player
            Vector3 direction = (player.position - transform.position).normalized;

            // Mueve al enemigo hacia el player
            transform.position += direction * moveSpeed * Time.deltaTime;

            // Opcional: rotarlo hacia el player
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            }
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
                UnityEngine.Debug.Log("El enemigo alcanzó al jugador.");
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
                    playerModel.TakeDamage(damage);
                    UnityEngine.Debug.Log("Jugador alcanzado y dañado");
                }
                else
                {
                    UnityEngine.Debug.LogWarning($"No se encontró PlayerModel en el objeto {player.name} ni en sus hijos/padres.");
                }

                StartCoroutine(WaitAfterHit());
            }
        }
        else
        {
            UnityEngine.Debug.LogWarning("La referencia a player es null.");
        }
    }

    private IEnumerator WaitAfterHit()
    {
        isWaiting = true;
        yield return new WaitForSeconds(2f);
        isWaiting = false;
    }
}
