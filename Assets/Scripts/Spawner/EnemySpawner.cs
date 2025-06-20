using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemySpawner : MonoBehaviour
{
    // public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int catfishPerWave = 4;
    public int lizardsPerWave = 1;
    private bool isSpawning = true;
    private Transform player;
    // Pool de objetos
    private ObjectPool poolCatfish;
    private ObjectPool poolLizard;

    public static EnemySpawner Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        poolCatfish = GetComponent<ObjectPool>();
        //if (poolCatfish != null) UnityEngine.Debug.Log("Habemus Pool de Catfish");

        poolLizard = GetComponent<ObjectPool>();
        //if (poolLizard != null) UnityEngine.Debug.Log("Habemus Pool de Lizard");
    }

    IEnumerator Start()
    {
        // Esperar 1 frame para asegurarte de que el Player se haya creado
        yield return null;

        // Buscar al Player si aún no está asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                UnityEngine.Debug.LogWarning("No se encontró un objeto con la etiqueta 'Player'.");
            }
        }

        StartCoroutine(SpawnWaveLoop());
    }

    // Probar con un pool

    IEnumerator SpawnWaveLoop()
    {
        while (isSpawning)
        {
            for (int i = 0; i < catfishPerWave; i++)
            {
                Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                SpawnEnemy(spawn.position);
            }
            for (int i = 0; i < lizardsPerWave; i++)
            {
                Transform spawn = spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
                SpawnLizard(spawn.position);
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
        // position.y = enemyPrefab.GetComponent<CapsuleCollider>().height / 2f;

        // Instanciar el enemigo sin pool
        // GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // newEnemy.tag = "Enemy";


        // Con Pool de Objetos chequeo que no este vacio
        if (!poolCatfish.IsEmpty())
        {
            GameObject newEnemy = poolCatfish.GetObject();
            position.y = newEnemy.GetComponent<CapsuleCollider>().height / 2f;
            newEnemy.transform.position = position;
            newEnemy.transform.rotation = Quaternion.identity;
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
                    UnityEngine.Debug.LogWarning("No se encontr� un objeto con la etiqueta 'Player' para asignar al enemigo Catfish.");
                    return; // Salir si no hay jugador
                }
            }

            // Asignar el jugador al EnemyPresenter
            EnemyPresenter enemyPresenter = newEnemy.GetComponent<EnemyPresenter>();
            if (enemyPresenter != null)
            {
                enemyPresenter.Initialize(player);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Enemy instanciado no tiene EnemyController.");
            }
        }
    }


    void SpawnLizard(Vector3 position)
    {
        // Ajustar la altura del enemigo
        // position.y = enemyPrefab.GetComponent<CapsuleCollider>().height / 2f;

        // Instanciar el enemigo sin pool
        // GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        // newEnemy.tag = "Enemy";


        // Con Pool de Objetos chequeo que no este vacio
        if (!poolLizard.IsEmpty())
        {
            GameObject newEnemy = poolLizard.GetObject();
            position.y = newEnemy.GetComponent<CapsuleCollider>().height / 2f;
            newEnemy.transform.position = position;
            newEnemy.transform.rotation = Quaternion.identity;
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
                    UnityEngine.Debug.LogWarning("No se encontr� un objeto con la etiqueta 'Player' para asignar al enemigo Lizard.");
                    return; // Salir si no hay jugador
                }
            }

            // Asignar el jugador al EnemyController
            EnemyLizardController controller = newEnemy.GetComponent<EnemyLizardController>();
            if (controller != null)
            {
                controller.Initialize(player);
            }
            else
            {
                UnityEngine.Debug.LogWarning("Enemy instanciado no tiene EnemyLizardController.");
            }
        }
    }
}