using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed = 10f;
    [SerializeField]
    private EnemyData enemyData;

    public event Action<GameObject> OnRecycle;
    public Vector3 direction { get; set; }

    private int damage = 100;

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            OnRecycle?.Invoke(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerPresenter playerPresenter = collision.gameObject.GetComponent<PlayerPresenter>();
            if (playerPresenter != null)
            {
                // Debug.Log("hay presenter");
                playerPresenter.ReceiveDamage(enemyData.Damage);
            }

            OnRecycle?.Invoke(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            print("Le di al Enemy");
            EnemyModel enemy = collision.gameObject.GetComponent<EnemyModel>();
            enemy.TakeDamage(damage);
            OnRecycle?.Invoke(this.gameObject);
        }
    }
}
