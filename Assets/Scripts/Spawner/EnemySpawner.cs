using System.Collections;
using System.Collections.Generic;
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
        
        position.y = enemyPrefab.GetComponent<CapsuleCollider>().height / 2f; // calculo la posion correcta para los enemigos
        GameObject newEnemy = Instantiate(enemyPrefab, position, Quaternion.identity);
        newEnemy.tag = "Enemy"; // Aseguro que tengan el tag

        //EnemyView view = newEnemy.GetComponent<EnemyView>();
        EnemyModel model = new EnemyModel(); // por si queremos customizar

        EnemyController controller = newEnemy.GetComponent<EnemyController>(); //A los enemy le paso el player para que en el script del controller lo sigan
    
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                controller.Initialize(playerObj.transform);
            }
        }
        

    }
}