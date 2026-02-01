using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float baseSpeed = 1.5f;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 15f;
    [SerializeField] private float iceSpeedMultiplier = 1.5f;
    [SerializeField] private float speedIncreasePerSecond = 0.12f;

    [Header("Velocidad máxima")]
    [SerializeField] private float maxSpeed = 8f;

    private bool jump = false;
    private bool onIceFloor = false;
    private bool isWallSliding = false;
    private bool isTouchingWall = false;

    private float currentSpeed;
    private float cronometro = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        cronometro += Time.deltaTime;

        if (cronometro > 30f && cronometro < 30.2f)
        {
            currentSpeed += 0.025f;
            speedIncreasePerSecond += 0.2f;
        }

        if (Input.GetButtonDown("Jump"))
            jump = true;

        CheckWallSliding();
    }

    void FixedUpdate()
    {
        currentSpeed += speedIncreasePerSecond * Time.fixedDeltaTime;

        float speedTotal = onIceFloor
            ? currentSpeed * iceSpeedMultiplier
            : currentSpeed;

        // 🔒 Clamp de velocidad máxima (Unity 6)
        float clampedX = Mathf.Clamp(speedTotal, -maxSpeed, maxSpeed);

        rb.linearVelocity = new Vector2(clampedX, rb.linearVelocity.y);

        if (jump && isWallSliding)
            WallJump();

        jump = false;
    }

    private void CheckWallSliding()
    {
        if (isTouchingWall && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                -wallSlideSpeed
            );
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        float jumpX = Mathf.Clamp(-baseSpeed, -maxSpeed, maxSpeed);
        rb.linearVelocity = new Vector2(jumpX, wallJumpForce);
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
