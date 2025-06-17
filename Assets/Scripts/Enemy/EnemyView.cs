using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    private Animator animator;

    public void Start()
    {
        animator = GetComponent<Animator>();
    }



    public void Attack()
    {
        animator.SetTrigger("attack");
        //animator.SetBool("attack_in_cooldown", false);
    }



    /*public void WaitAttack()
    {
        animator.SetBool("attack_in_cooldown", true);
    }*/

    /*public void Walk()
    {
        animator.SetBool("is_attacking", false); // Activar la animación de caminata
        animator.SetBool("attack_in_cooldown", false);
    }*/


    public void Die()
    {
        animator.SetTrigger("die");
    }







    
}
