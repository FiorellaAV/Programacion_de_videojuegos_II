using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthKit : MonoBehaviour
{
    float healthGiven = 30;

    public float rotationSpeed;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 1 * rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerPresenter player = other.GetComponent<PlayerPresenter>();
            if (player != null)
            {
                player.ApplyHealthKit(GetHealthGiven());
                Destroy(gameObject);
            }
        }
    }

    public float GetHealthGiven()
    {

        return healthGiven;
    }
}
