using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerPresenter : MonoBehaviour
{
    private PlayerModel model;
    public PlayerModel GetModel() => model;
    private PlayerView view;
    private PlayerInput input;
    private Camera mainCamera;

    private float lastDashTime = -Mathf.Infinity;

    public float moveSpeed = 5f;
    public float jumpForce = 50f;
    public float rayDistance = 100f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 2f;

    void Awake()
    {
        model = new PlayerModel();
        view = GetComponent<PlayerView>();
        input = GetComponent<PlayerInput>();
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
    }
    private void OnDisable()
    {
        input.OnInputShooting -= IsShooting;
        input.OnInputDashing -= IsDashing;
        input.OnInputJumping -= IsJumping;

    }

    void Update()
    {
        if (model.health <= 0f || GameManager.Instance?.gameEnded == true)
            return;

        view.VerifyJump();
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
    private void Explode(Vector3 explosionPos, float explosionRadius, float explosionDamage)
    {
        view.PlayExplosionEffect(explosionPos);
        Collider[] colliders = Physics.OverlapSphere(explosionPos, explosionRadius);
        foreach (Collider nearby in colliders)
        {
            if (nearby.CompareTag("Enemy"))
            {
                if (nearby.TryGetComponent<EnemyModel>(out var enemy)) enemy.TakeDamage();
                if (nearby.TryGetComponent<EnemyLizardModel>(out var lizard)) lizard.TakeDamage();
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

    /*public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            EnemyPresenter enemyPresenter = collision.gameObject.GetComponent<EnemyPresenter>();
            // TODO. Llamar al Takedamaage pero con el dea�o segun enemigo
            model.TakeDamage(enemyPresenter.enemyData.Damage);
            StartCoroutine(enemyPresenter.WaitAfterHit());
        }
    }*/

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
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(origin, direction, out RaycastHit hit, rayDistance))
        {
            view.DrawShotLine(origin, hit.point);

            if (hit.collider.CompareTag("Enemy"))
            {
                view.ShowBloodEffect(hit.collider.transform.position);
                GameManager.Instance?.EnemyKilled();

                if (hit.collider.TryGetComponent<EnemyModel>(out var enemy)) enemy.TakeDamage();
                if (hit.collider.TryGetComponent<EnemyLizardModel>(out var lizard)) lizard.TakeDamage();
            }

            if (hit.collider.CompareTag("Barrel"))
            {
                Vector3 explosionPos = hit.collider.transform.position;
                Destroy(hit.collider.gameObject);

                float radius = 4f;
                float damage = 200f;
                Explode(explosionPos, radius, damage);
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


