using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyLizardModel : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float health = 100f;

    public GameObject healthKit;
    private EnemySpawner spawner;
    private ObjectPool pool;
    private EnemyLizardView ev;

    void Awake()
    {
        ev = GetComponent<EnemyLizardView>();

    }

    void Start()
    {
        spawner = EnemySpawner.Instance;
        pool = spawner.GetComponent<ObjectPool>();
        if (pool != null) UnityEngine.Debug.Log("Habemus Pool en EnemyModel");
    }

    void Update()
    {

    }

    public void TakeDamage()
    {
        health -= 100; //de momento dejar as�, cambiar por otro valor, quiz�s usar una clase Enemy con diferentes valores para que cada enemigo reciba diferente da�o?
        checkHealth();
    }

    private void checkHealth()
    {
        if (health <= 0)
        {
            ev.Die(); // Lanza animaci�n de muerte
            generatePowerUp();

            // Desactivar l�gica y colisiones para que no afecte el juego
            DisableEnemy();

            // Esperar a que termine la animaci�n antes de devolver al pool
            StartCoroutine(DieAfterDelay(2f));
        }
    }


    private IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Reactivar componentes para el pr�ximo uso
        EnableEnemy();

        pool.ReturnObject(this.gameObject);
    }


    private void DisableEnemy()
    {
        // Evitar que siga interactuando con el mundo
        //Collider col = GetComponent<Collider>();
        //if (col != null) col.enabled = false;

        EnemyLizardController ec = GetComponent<EnemyLizardController>();
        if (ec != null) ec.enabled = false;
        ec.moveSpeed = 3f;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.ResetPath();
        //if (agent != null) agent.enabled = false;


        // Tambi�n pod�s apagar otros componentes si hiciera falta
        // Rigidbody rb = GetComponent<Rigidbody>();
        // if (rb != null) rb.isKinematic = true;
    }

    private void EnableEnemy()
    {
        //Collider col = GetComponent<Collider>();
        //if (col != null) col.enabled = true;

        EnemyLizardController ec = GetComponent<EnemyLizardController>();
        if (ec != null) ec.enabled = true;

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = true;

        // Reiniciar vida u otros valores si hace falta
        health = 100f;
    }



    private void generatePowerUp()
    {
        //Cambiar luego l�gica del c�digo para otros power ups.
        float chance = UnityEngine.Random.Range(1, 61);
        if (chance <= 3)
        {
            GameObject.Instantiate(healthKit, transform.position, transform.rotation);
        }
    }
}
