using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
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
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("is_alive", true); // Desactiva animación de muerte al iniciar
        animator.SetBool("is_attacking", false); // Desactiva animación de ataque al iniciar
    }



    public void Attack()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("is_attacking", true); // Activar la animación de ataque
        animator.SetBool("attack_in_cooldown", false);
    }



    public void WaitAttack()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("attack_in_cooldown", true);
    }

    public void Walk()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        animator.SetBool("is_attacking", false); // Activar la animación de caminata
        animator.SetBool("attack_in_cooldown", false);
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
