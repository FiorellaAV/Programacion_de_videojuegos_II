using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 5f;
    public int barrelsPerWave = 1;
    public float checkRadius = 0.5f; // radio para verificar colisión con otros barriles

    void Start()
    {
        StartCoroutine(SpawnWaveLoop());
    }

    IEnumerator SpawnWaveLoop()
    {
        while (true)
        {
            for (int i = 0; i < barrelsPerWave; i++)
            {
                Transform spawn = GetAvailableSpawnPoint();
                if (spawn != null)
                {
                    SpawnBarrel(spawn.position);
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Transform GetAvailableSpawnPoint()
    {
        // Intentamos encontrar un punto de spawn libre
        List<Transform> shuffled = new List<Transform>(spawnPoints);
        ShuffleList(shuffled);

        foreach (Transform point in shuffled)
        {
            if (!Physics.CheckSphere(point.position, checkRadius, LayerMask.GetMask("Default")))
            {
                return point;
            }
        }

        return null; // No hay punto libre
    }

    void SpawnBarrel(Vector3 position)
    {
        //Quaternion rotation = Quaternion.Euler(-90f, 0f, 0f); // rota sobre el eje X
        GameObject newBarrel = Instantiate(barrelPrefab, position, Quaternion.identity);
        newBarrel.tag = "Barrel";
    }

    // Utilidad para mezclar la lista
    void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}