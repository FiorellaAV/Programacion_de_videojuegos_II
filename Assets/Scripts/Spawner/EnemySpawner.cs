using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player;
    public float spawnInterval = 5f;
    public int enemiesPerWave = 4;
    private bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnWaveLoop());
    }

    IEnumerator SpawnWaveLoop()
    {
        while (isSpawning)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnEnemy(spawn.position);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    public void StopSpawning()
    {
        isSpawning = false;
    }
    void SpawnEnemy(Vector3 position)
    {
        // Ajustar la altura del enemigo
        position.y = enemyPrefab.GetComponent<CapsuleCollider>().height / 2f;

        // Instanciar el enemigo
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.tag = "Enemy";

        // Asegurar que la referencia al jugador est� asignada
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                UnityEngine.Debug.LogWarning("No se encontr� un objeto con la etiqueta 'Player' para asignar al enemigo.");
                return; // Salir si no hay jugador
            }
        }

        // Asignar el jugador al EnemyController
        EnemyController controller = newEnemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.Initialize(player);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Enemy instanciado no tiene EnemyController.");
        }
    }

    //void SpawnEnemy(Vector3 position)
    //{
    //    NavMeshHit hit;
    //    if (NavMesh.SamplePosition(position, out hit, 2.0f, NavMesh.AllAreas))
    //    {
    //        // Instanciar sobre posición válida
    //        GameObject newEnemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
    //        newEnemy.tag = "Enemy";

    //        if (player == null)
    //        {
    //            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
    //            if (playerObj != null)
    //            {
    //                player = playerObj.transform;
    //            }
    //            else
    //            {
    //                UnityEngine.Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player' para asignar al enemigo.");
    //                return;
    //            }
    //        }

    //        EnemyController controller = newEnemy.GetComponent<EnemyController>();
    //        if (controller != null)
    //        {
    //            controller.Initialize(player);
    //        }
    //        else
    //        {
    //            UnityEngine.Debug.LogWarning("Enemy instanciado no tiene EnemyController.");
    //        }
    //    }
    //    else
    //    {
    //        UnityEngine.Debug.LogWarning("No se encontró una posición válida en el NavMesh para el enemigo.");
    //    }
    //}
}