using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthuluhStatuePresenter : MonoBehaviour
{
    public float shootInterval = 0.5f;
    private CthuluhStatueModel model;
    private CthuluhStatueView view;

    void Awake()
    {
        model = GetComponent<CthuluhStatueModel>();
        view = GetComponent<CthuluhStatueView>();
    }
    void Start()
    {
        StartCoroutine(DispararCadaMedioSegundo());
    }

    IEnumerator DispararCadaMedioSegundo()
    {
        while (true)
        {
            model.shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }
}
