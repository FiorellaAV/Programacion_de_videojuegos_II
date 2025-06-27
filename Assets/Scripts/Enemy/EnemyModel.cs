using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyModel : MonoBehaviour
{
    public float health;

    public GameObject healthKit;
    private EnemySpawner spawner;
    private ObjectPool pool;
    private EnemyView ev;
    private CapsuleCollider collider;

    void Awake()
    {
        collider = GetComponent<CapsuleCollider>();
        ev = GetComponent<EnemyView>();
    }

    void Start()
    {
        
        spawner = EnemySpawner.Instance;
        pool = spawner.GetComponent<ObjectPool>();
        // if (pool != null) UnityEngine.Debug.Log("Habemus Pool en EnemyModel");
    }

    public void TakeDamage(int value)
    {
        health -= value; //de momento dejar así, cambiar por otro valor, quizás usar una clase Enemy con diferentes valores para que cada enemigo reciba diferente daño?
        checkHealth();
        UnityEngine.Debug.Log(health);
    }

    private void checkHealth()
    {
        if (health <= 0)
        {
            DisableEnemy();

            ev.Die(); // Lanza animación de muerte

            generatePowerUp();

            // Desactivar lógica y colisiones para que no afecte el juego
            

            // Esperar a que termine la animación antes de devolver al pool
            StartCoroutine(DieAfterDelay(2f));
        }
    }


    private IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reactivar componentes para el próximo uso
        EnableEnemy();

        pool.ReturnObject(this.gameObject);
    }


    private void DisableEnemy()
    {
        collider.enabled =false;

        EnemyPresenter enemyPresenter = GetComponent<EnemyPresenter>();
        if (enemyPresenter != null) enemyPresenter.enabled = false;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.ResetPath();
    }

    private void EnableEnemy()
    {
        collider.enabled = true;

        EnemyPresenter enemyPresenter = GetComponent<EnemyPresenter>();
        if (enemyPresenter != null) enemyPresenter.enabled = true;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = true;

        // Reiniciar vida u otros valores si hace falta
        health = 100f;
    }



    private void generatePowerUp()
    {
        //Cambiar luego lógica del código para otros power ups.
        float chance = UnityEngine.Random.Range(1, 61);
        if (chance <= 3)
        {
            GameObject.Instantiate(healthKit, transform.position, transform.rotation);
        }
    }
}
