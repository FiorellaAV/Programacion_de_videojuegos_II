using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyModel : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float health = 100f;

    public GameObject healthKit;

    void Awake()
    {

    }

    void Start()
    {
        
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
            generatePowerUp();
            Destroy(this.gameObject);
        }
    }

    private void generatePowerUp()
    {
        //Cambiar luego l�gica del c�digo para otros power ups.
        float chance = Random.Range(1, 61);
        if (chance <= 3)
        {
            GameObject.Instantiate(healthKit, transform.position, transform.rotation);
        }
    }
}
