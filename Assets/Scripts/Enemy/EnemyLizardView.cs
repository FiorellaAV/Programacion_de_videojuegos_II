using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLizardView : MonoBehaviour
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
        animator.SetBool("is_attacking", true);
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
        animator.SetBool("is_attacking", false);
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
