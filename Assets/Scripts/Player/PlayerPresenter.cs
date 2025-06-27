using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerPresenter : MonoBehaviour
{
    // private Transform bulletSpawnPoint;
    private PlayerModel model;
    public PlayerModel GetModel() => model;
    private PlayerView view;
    private PlayerInput input;
    private Camera mainCamera;
    private ObjectPool pool;
    private bool laser = true;
    private int damage = 20;
    private bool canShoot = true;
    private float shootCooldown = 2f;
    private float lastDashTime = -Mathf.Infinity;

    public float moveSpeed = 5f;
    public float jumpForce;
    public float rayDistance = 100f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;
    
    public float radius = 4f;
    public float spawnOffset = 1.5f; 

    void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        input = GetComponent<PlayerInput>();
        pool = GetComponent<ObjectPool>(); // if (pool != null) print("Habemus pool");
        mainCamera = Camera.main;
    }

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        view.Initialize();
    }

    private void OnEnable()
    {
        input.OnInputShooting += IsShooting;
        input.OnInputDashing += IsDashing;
        input.OnInputJumping += IsJumping;
        input.OnInputChangeWeapon += ChangeWeapon;
    }
    private void OnDisable()
    {
        input.OnInputShooting -= IsShooting;
        input.OnInputDashing -= IsDashing;
        input.OnInputJumping -= IsJumping;
        input.OnInputChangeWeapon -= ChangeWeapon;
    }

    void Update()
    {
        if (model.health <= 0f || GameManager.Instance?.gameEnded == true)
            return;

        view.VerifyJump();
        Debug.Log(laser);
    }

    void FixedUpdate()
    {
        if (model.health <= 0f || GameManager.Instance?.gameEnded == true)
            return;

        MovePlayer();
        model.RotateTowardsMouse(mainCamera, GetComponent<Rigidbody>());
    }

    private void MovePlayer()
    {
        // Crear vector de movimiento
        Vector3 movement = input.Axis.normalized;
        // Mover al jugador
        view.Move(movement, moveSpeed);
        // Actualizar animaciones
        view.AimToMouse(input.Axis.x, input.Axis.z);
    }
    private void Explode(Vector3 explosionPos, float explosionRadius, int explosionDamage)
    {
        view.PlayExplosionEffect(explosionPos);
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            if (nearby.CompareTag("Enemy"))
            {
                if (nearby.TryGetComponent<EnemyModel>(out var enemy)) enemy.TakeDamage(explosionDamage);

            }
            if (nearby.CompareTag("Player"))
            {

                if (nearby.TryGetComponent<PlayerPresenter>(out var playerPresenter))
                {
                    playerPresenter.ReceiveDamage(explosionDamage);
                }
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        model.TakeDamage(damage);

        if (model.IsDead())
        {
            view.Die();
            GameManager.Instance.GameOver();
        }
    }

    public void ApplyHealthKit(float amount)
    {
        model.Heal(amount);
    }

    public void IsShooting()
    {
        if(Time.timeScale == 0f) { return; } //Solucionar bug que dispara en pausa
        if (laser)
        {
            ShootLine();
        }
        else
        {
            ShootBullet();
        }
    }

    public void ChangeWeapon()
    {
        print("Change Weapon");
        laser = !laser;
    }

    private void ShootBullet()
    {

        if (canShoot)
        {
            view.PlayRockShootSfx();
            canShoot = false;
            // Obtener ray desde cámara al mouse
            Ray ray = mainCamera.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit hit;

            Vector3 targetPoint;

            // Si choca algo, disparamos hacia ahí; si no, hacia un punto adelante
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * rayDistance;
            }

            // Calcular dirección normalizada
            Vector3 shootDirection = (targetPoint - transform.position).normalized;

            // Definir posición de spawn: un poco adelante del player en esa dirección        
            Vector3 spawnPosition = transform.position + shootDirection * spawnOffset;

            // Instanciar bala
            if (!pool.IsEmpty())
            {
                GameObject bullet = pool.GetObject();
                Bullet bulletBehavior = bullet.GetComponent<Bullet>();
                bulletBehavior.OnRecycle += OnBulletRecyclechanged;
                bullet.transform.position = spawnPosition;
                bulletBehavior.direction = shootDirection;
            }

            StartCoroutine(RestartCooldown(shootCooldown));
        }
    }

    IEnumerator RestartCooldown (float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        canShoot = true;
    }

    private void OnBulletRecyclechanged(GameObject bullet)
    {
        pool.ReturnObject(bullet);
    }

    private void ShootLine()
    {
        view.PlayShootSfx();
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            view.DrawShotLine(origin, hit.point);

            if (hit.collider.CompareTag("Enemy"))
            {
                view.ShowBloodEffect(hit.collider.transform.position);
                

                if (hit.collider.TryGetComponent<EnemyModel>(out var enemy)) enemy.TakeDamage(damage);
            }

            if (hit.collider.CompareTag("Barrel"))
            {
                Vector3 explosionPos = hit.collider.transform.position;
                Destroy(hit.collider.gameObject);               
               
                Explode(explosionPos, radius, 800);
            }
        }
    }

    public void IsDashing()
    {
        if (Time.time >= lastDashTime + dashCooldown && !model.isDashing)
        {
            lastDashTime = Time.time;
            model.isDashing = true;
            StartCoroutine(model.Dash(GetComponent<Rigidbody>(), dashDistance, dashDuration, transform));
        }
    }

    public void IsJumping()
    {
        if (view.IsGrounded())
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            view.Jump();
        }
    }

}


