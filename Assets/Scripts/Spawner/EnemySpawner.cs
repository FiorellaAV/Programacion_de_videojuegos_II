using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public Transform player;
    public float spawnInterval = 5f;
    public int enemiesPerWave = 4;

    void Start()
    {
        StartCoroutine(SpawnWaveLoop());
    }

    IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            for (int i = 0; i < enemiesPerWave; i++)
            {
                Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
                SpawnEnemy(spawn.position);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy(Vector3 position)
    {
        // Ajustar la altura del enemigo
        position.y = enemyPrefab.GetComponent<CapsuleCollider>().height / 2f;

        // Instanciar el enemigo
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.tag = "Enemy";

        // Asegurar que la referencia al jugador esté asignada
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                UnityEngine.Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player' para asignar al enemigo.");
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
}



