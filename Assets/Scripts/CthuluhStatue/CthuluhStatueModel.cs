using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CthuluhStatueModel : MonoBehaviour
{    
    private ObjectPool _pool;
    private Transform _bulletSpawArea;
    private Coroutine _rotationCoroutine;
    private Quaternion _initialRotation;
    void Awake()
    {
        _pool = GetComponent<ObjectPool>();
        _bulletSpawArea = GetComponentsInChildren<Transform>().FirstOrDefault(t => t.CompareTag("BulletSpawnArea"));   // if (_bulletSpawArea != null) print("Habemus transform");
        _initialRotation = transform.rotation;  // Guardamos la rotación inicial
        RotarPingPong();
    }

    public void Teletransport()
    {

    }

    public void RotarPingPong()
    {
        if (gameObject.activeSelf)
        {
            _rotationCoroutine = StartCoroutine(Rotar());
        }
    }

    private IEnumerator Rotar()
    {
        while (true)
        {
            // Oscila entre -45 y +45 usando Mathf.PingPong
            float angle = Mathf.PingPong(Time.time * 45f, 90f) - 45f;  // 0-90 -> -45 a +45

            // Sumar rotación relativa a la inicial
            transform.rotation = _initialRotation * Quaternion.Euler(0f, angle, 0f);

            yield return null;
        }
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
