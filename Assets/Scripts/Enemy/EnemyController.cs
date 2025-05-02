using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;      // Asignar desde el Inspector
    public float moveSpeed = 3f;  // Velocidad de movimiento

    void Update()
    {
        if (player != null)
        {
            // Calcula dirección hacia el player
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
}
