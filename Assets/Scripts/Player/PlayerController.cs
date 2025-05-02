using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody rb;
    private PlayerModel pm;
    private PlayerView pv;
    private Camera mainCamera;
    private LineRenderer lr;

    public float jumpForce = 5f;
    public float rayDistance = 100f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.3f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerModel>();
        pv = GetComponent<PlayerView>();
        lr = GetComponent<LineRenderer>();
        mainCamera = Camera.main;
    }
    
    void FixedUpdate()
    {
        pm.MovePlayer(moveSpeed, rb);
        pm.RotateTowardsMouse(mainCamera, rb);

        if (Input.GetKey(KeyCode.Space) && pm.IsGrounded(groundCheck, groundCheckRadius, groundLayer))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Update()
    {
        pm.HandleShooting(rayDistance, lr);
        pm.DrawDebugRayFromPlayer(rayDistance);
    }
}
