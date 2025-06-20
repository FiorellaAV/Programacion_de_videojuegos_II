using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CthuluhStatueModel : MonoBehaviour
{    
    private ObjectPool _pool;
    private Transform _bulletSpawArea;
    void Awake()
    {
        _pool = GetComponent<ObjectPool>();
        _bulletSpawArea = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("BulletSpawnArea"));   // if (_bulletSpawArea != null) print("Habemus transform");
    }

    public void Teletransport()
    {

    }

    public void shoot()
    {
        if (!_pool.IsEmpty())
        {
            GameObject bullet = _pool.GetObject();
            Bullet bulletBehavior = bullet.GetComponent<Bullet>();  // if (bulletBehavior != null) print("BulletBehavior");
            bulletBehavior.OnRecycle += OnBulletRecyclechanged;
            bullet.transform.position = _bulletSpawArea.position;
            // Setear la direccion de la bala
            Vector3 direction = -transform.right;
            bulletBehavior.direction = direction;
        }
    }

    private void OnBulletRecyclechanged(GameObject bullet)
    {
        _pool.ReturnObject(bullet);
    }
}
