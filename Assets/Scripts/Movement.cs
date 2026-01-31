using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 4f;
    [SerializeField] private float iceSpeedMultiplier = 1.5f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 15f;

    private float currentSpeed;
    private bool onIceFloor = false;
    private bool isTouchingWall = false;
    private bool isWallSliding = false;
    private bool jump;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        CheckWallSliding();
    }

    void FixedUpdate()
    {
        float speed = onIceFloor ? baseSpeed * iceSpeedMultiplier : currentSpeed;

        // Movimiento constante hacia la derecha
        rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);

        if (jump && isWallSliding)
        {
            WallJump();
        }

        jump = false;
    }

    private void CheckWallSliding()
    {
        if (isTouchingWall && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlideSpeed);
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        rb.linearVelocity = new Vector2(-baseSpeed, wallJumpForce);
        isWallSliding = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("iceFloor"))
            onIceFloor = true;

        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("iceFloor"))
            onIceFloor = false;

        if (collision.gameObject.CompareTag("Wall"))
            isTouchingWall = false;
    }
}