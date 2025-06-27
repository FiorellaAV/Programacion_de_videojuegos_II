using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Vector3 inputDirection;
    private bool wasInputActive;
    public Vector3 Axis => inputDirection;

    public event Action OnInputStarted;
    public event Action OnInputStopped;
    public event Action OnInputShooting;
    public event Action OnInputDashing;
    public event Action OnInputJumping;
    public event Action OnInputChangeWeapon;

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        inputDirection = new Vector3(inputX, 0f, inputZ);

        bool isInputActive = (inputX != 0f || inputZ != 0f);

        if (isInputActive && !wasInputActive)
        {
            OnInputStarted?.Invoke();
        }
        else if (!isInputActive && wasInputActive)
        {
            OnInputStopped?.Invoke();
        }

        if (Input.GetMouseButtonDown(0)) // Click izquierdo
        {
            OnInputShooting?.Invoke();
        }

        if (Input.GetMouseButtonDown(1)) // Click derecho
        {
            OnInputDashing?.Invoke();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            OnInputJumping?.Invoke();
        }

        if (Input.GetKey(KeyCode.C))
        {
            OnInputChangeWeapon?.Invoke();
        }

        wasInputActive = isInputActive;
    }
}
