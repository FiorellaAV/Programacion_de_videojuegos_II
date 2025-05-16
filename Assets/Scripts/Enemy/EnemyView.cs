using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    private EnemyController controller;

    public void SetController(EnemyController enemyController)
    {
        controller = enemyController;
    }

    private void FixedUpdate()
    {
        controller?.Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }
}
