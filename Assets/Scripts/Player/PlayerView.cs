using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerView : MonoBehaviour
{
    private Animator animator;
    public LineRenderer lineRenderer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer;

    [SerializeField] private ParticleSystem particles;

    void Awake()
    {
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        particles = GetComponentInChildren<ParticleSystem>();
    }

    public void Initialize()
    {
        animator.SetBool("is_alive", true);
    }
    void Start()
    {
        lineRenderer.enabled = false;
    }

    public bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void Move(Vector3 movement, float speed)
    {
        GetComponent<Rigidbody>().MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }

    public void RotateTowards(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            GetComponent<Rigidbody>().MoveRotation(rotation);
        }
    }

    public void StartParticles()
    {
        particles.Play();
    }

    public void DrawShotLine(Vector3 origin, Vector3 target, float duration = 0.05f)
    {
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, target);
        StartCoroutine(DisableLineAfterDelay(duration));
    }
    private IEnumerator DisableLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        lineRenderer.enabled = false;
    }

    public void ShowBloodEffect(Vector3 position)
    {
        GameObject vfx = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Blood Splash"), position, Quaternion.identity);
        GameObject.Destroy(vfx, 1f);
    }

    public void Jump()
    {
        animator.SetBool("is_jumping", true);
    }

    public void VerifyJump()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("is_jumping", false);
        }
    }

    public void AimToMouse(float moveX, float moveZ)
    {
        Vector3 globalMovement = new Vector3(moveX, 0f, moveZ);
        Vector3 localMovement = transform.InverseTransformDirection(globalMovement);

        animator.SetFloat("aim_x", localMovement.x);
        animator.SetFloat("aim_y", localMovement.z);
    }

    public void PlayExplosionEffect(Vector3 position)
    {
        GameObject explosionVFX = GameObject.Instantiate(Resources.Load<GameObject>("Simple FX Kit/Prefabs/Explosion Fire"), position, Quaternion.identity);
        GameObject.Destroy(explosionVFX, 2f);
    }

    public void Die()
    {
        animator.SetBool("is_alive", false);
    }
}



