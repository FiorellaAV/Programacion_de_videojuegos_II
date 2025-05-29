using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyModel : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float health = 100f;

    public GameObject healthKit;
    private EnemySpawner spawner;
    private ObjectPool pool;

    void Awake()
    {
        
    }

    void Start()
    {
        spawner = EnemySpawner.Instance;
        pool = spawner.GetComponent<ObjectPool>();
        if (pool != null) Debug.Log("Habemus Pool en EnemyModel");
    }

    void Update()
    {
        
    }

    public void TakeDamage()
    {
        health -= 100; //de momento dejar así, cambiar por otro valor, quizás usar una clase Enemy con diferentes valores para que cada enemigo reciba diferente daño?
        checkHealth();
    }

    private void checkHealth()
    {
        if (health <= 0)
        {
            generatePowerUp();
            // Cambiar por Recicle
            pool.ReturnObject(this.gameObject);
            // Destroy(this.gameObject);
        }
    }

    private void generatePowerUp()
    {
        //Cambiar luego lógica del código para otros power ups.
        float chance = Random.Range(1, 61);
        if (chance <= 3)
        {
            GameObject.Instantiate(healthKit, transform.position, transform.rotation);
        }
    }
}
