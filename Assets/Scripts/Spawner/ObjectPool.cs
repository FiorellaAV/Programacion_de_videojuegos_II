using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple object pool para reutilizar instancias de un prefab
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("Prefab a instanciar")]
    public GameObject prefab;

    [Header("Cantidad inicial")]
    public int initialSize = 10;


    [Header("Prefab a instanciar")]
    public GameObject prefab2;

    [Header("Cantidad inicial")]
    public int initialSize2 = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        // Pre-instanciar objetos inactivos
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
            // Debug.Log("Create object: " + i);
        }
        // Pre-instanciar objetos inactivos
        for (int i = 0; i < initialSize2; i++)
        {
            GameObject obj = Instantiate(prefab2);
            obj.SetActive(false);
            pool.Enqueue(obj);
            // Debug.Log("Create object: " + i);
        }
    }

    /// <summary>
    /// Obtiene un objeto del pool
    /// </summary>
    public GameObject GetObject()
    {
        GameObject obj;

        // Si hay objetos disponibles, usar uno
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return null;
            // Sino, crear uno nuevo
            // obj = Instantiate(prefab);
        }

    }

    /// <summary>
    /// Devuelve un objeto al pool
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Devuelve true si el pool está vacío
    /// </summary>
    public bool IsEmpty()
    {
        return pool.Count == 0;
    }
}
