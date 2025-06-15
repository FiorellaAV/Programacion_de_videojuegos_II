using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    public float health = 100f;
    public bool isDashing = false;


    public IEnumerator Dash(Rigidbody rb, float dashDistance, float dashDuration, Transform transform)
    {

        Vector3 dashDirection = transform.forward;
        float maxDashDistance = dashDistance;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dashDirection, out RaycastHit hit, dashDistance))
        {
            maxDashDistance = hit.distance - 0.1f;
        }

        float elapsed = 0f;
        float actualDashDuration = dashDuration * (maxDashDistance / dashDistance);

        while (elapsed < actualDashDuration)
        {
            rb.MovePosition(rb.position + dashDirection * (maxDashDistance / actualDashDuration) * Time.fixedDeltaTime);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
    }

    public void RotateTowardsMouse(Camera camera, Rigidbody rb)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);
            Vector3 lookDir = (target - rb.position).normalized;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                rb.MoveRotation(Quaternion.LookRotation(lookDir));
            }
        }
    }
    public float GetHealth()
    {
        return health;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health < 0f) health = 0f;
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health > 100f)
        {
            health = 100f;
        }
    }

    public bool IsDead()
    {
        return health <= 0f;
    }
}
