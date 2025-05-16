using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerView : MonoBehaviour
{

    private Animator animator;

    public void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public void Start()
    {
        animator.SetBool("is_alive", true); // Desactiva animación de muerte al iniciar
    }


    public void Jump()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("is_jumping", true); // Activar la animación de salto
    }


    public void Dash()
    {
        // No hay animación de Dash
    }


    public void VerifyJump()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        // Verificar si la animación de salto ha terminado
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            animator.SetBool("is_jumping", false); // Desactivar la animación de salto
        }
        
    }


    public void AimToMouse(float moveX, float moveZ)
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Crear el vector de movimiento en el espacio global
        Vector3 globalMovement = new Vector3(moveX, 0f, moveZ);

        // Transformar el vector de movimiento al espacio local del personaje
        Vector3 localMovement = transform.InverseTransformDirection(globalMovement);

        // Si hay movimiento, actualizar aim_x y aim_y en el espacio local
        if (localMovement != Vector3.zero)
        {
            animator.SetFloat("aim_x", localMovement.x);
            animator.SetFloat("aim_y", localMovement.z);
        }
        else
        {
            // Si no hay movimiento, establecer ambos en 0
            animator.SetFloat("aim_x", 0f);
            animator.SetFloat("aim_y", 0f);
        }
    }

    public void Die()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("is_alive", false);
    }


}
