using UnityEngine;

public class PlayerAirMovement : MonoBehaviour
{
    [SerializeField] private PlayerVisuals playerVisuals;
    [SerializeField] private AnimationCurve liftByThrust;

    [SerializeField] private float moveSpeed;

    [SerializeField] private float jumpForce;
    [SerializeField] private float liftForce;
    [SerializeField] private float thrustForce;
    [SerializeField] private float maxSpeed;

    public bool IsInAir { get; private set; }
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;

    private new Rigidbody2D rigidbody2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (moveDirection != Vector2.zero)
        {
            lastMoveDirection = moveDirection;
        }
        moveDirection = Vector2.zero;

        SetVisuals();

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection.x -= 1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection.x += 1f;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }


        if (Input.GetKey(KeyCode.Space))
        {
            IsInAir = true;
        }
        else
        {
            IsInAir = false;
            Move();
        }

        if (rigidbody2D.velocity.magnitude > maxSpeed)
        {
            LimitVelocity();
        }
    }

    private void SetVisuals()
    {
        if (moveDirection.x < 0f)
            playerVisuals.SetDirectionLeft();
        else if (moveDirection.x > 0f)
            playerVisuals.SetDirectionRight();
    }

    private void LimitVelocity()
    {
        rigidbody2D.velocity = new Vector2(Mathf.Min(rigidbody2D.velocity.x, maxSpeed), rigidbody2D.velocity.y);
    }

    private void FixedUpdate()
    {
        if (IsInAir)
        {
            Glide();
        }
    }

    private void Move()
    {
        transform.Translate(moveSpeed * Time.deltaTime * moveDirection);
    }

    private void Jump()
    {
        rigidbody2D.AddForce(Vector2.up * jumpForce);
    }

    private void Glide()
    {
        var thrust = thrustForce * Time.fixedDeltaTime * lastMoveDirection;
        rigidbody2D.AddForce(thrust);

        rigidbody2D.AddForce(-Physics2D.gravity.y * Vector2.up);
        rigidbody2D.AddForce(liftByThrust.Evaluate(Mathf.Abs(rigidbody2D.velocity.x) / maxSpeed) * liftForce * -Physics2D.gravity.y * Vector2.up);
    }
}
