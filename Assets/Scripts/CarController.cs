using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Weight Effect")]
    [SerializeField] private float weightAccelerationEffect = 0.5f;
    [SerializeField] private float downhillWeightEffect = 0.8f;
    [SerializeField] private float downhillMaxSpeedBonus = 2f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 150f;

    [Header("Braking")]
    [SerializeField] private float brakeForce = 5f;

    private Rigidbody2D rb;
    private WeightSystem weightSystem;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weightSystem = GetComponent<WeightSystem>();
    }

    private void FixedUpdate()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");
        float rotateInput = Input.GetAxisRaw("Vertical");

        HandleMovement(moveInput);
        HandleDownhillMomentum();
        HandleRotation(moveInput, rotateInput);
        HandleBraking(moveInput);
        LimitSpeed();
    }

    private void HandleMovement(float moveInput)
    {
        if (Mathf.Abs(moveInput) < 0.01f)
            return;

        float currentWeight = weightSystem.GetCurrentWeight();

        // Heavier car = less acceleration.
        float weightMultiplier =
            1f / (1f + (currentWeight - 1f) * weightAccelerationEffect);

        float currentAcceleration = acceleration * weightMultiplier;

        if (moveInput > 0)
        {
            rb.AddForce(transform.right * currentAcceleration);
        }
        else
        {
            rb.AddForce(-transform.right * currentAcceleration);
        }
    }

    private void HandleDownhillMomentum()
    {
        float currentWeight = weightSystem.GetCurrentWeight();

        // Only the extra weight above 1.0x creates the special effect.
        float extraWeight = Mathf.Max(0f, currentWeight - 1f);

        if (extraWeight <= 0f)
            return;

        // Determine whether gravity is pulling the car toward its forward direction.
        float downhillGravity =
            Vector2.Dot(Physics2D.gravity, transform.right);

        if (downhillGravity > 0f)
        {
            float downhillForce =
                downhillGravity * extraWeight * downhillWeightEffect;

            rb.AddForce(transform.right * downhillForce);
        }
    }

    private void HandleRotation(float moveInput, float rotateInput)
    {
        if (Mathf.Abs(rotateInput) < 0.01f)
            return;

        if (Mathf.Abs(moveInput) < 0.01f)
            return;

        float direction = moveInput > 0 ? -1f : 1f;

        rb.MoveRotation(
            rb.rotation +
            rotateInput * rotationSpeed *
            Time.fixedDeltaTime * direction
        );
    }

    private void HandleBraking(float moveInput)
    {
        if (Mathf.Abs(moveInput) < 0.01f)
        {
            Vector2 velocity = rb.linearVelocity;

            velocity.x = Mathf.MoveTowards(
                velocity.x,
                0f,
                brakeForce * Time.fixedDeltaTime
            );

            rb.linearVelocity = velocity;
        }
    }

    private void LimitSpeed()
    {
        float currentWeight = weightSystem.GetCurrentWeight();
        float extraWeight = Mathf.Max(0f, currentWeight - 1f);

        // Allow a small additional top speed when going downhill.
        float effectiveMaxSpeed = maxSpeed;

        float downhillGravity =
            Vector2.Dot(Physics2D.gravity, transform.right);

        if (downhillGravity > 0f)
        {
            effectiveMaxSpeed +=
                extraWeight * downhillMaxSpeedBonus;
        }

        float speed = rb.linearVelocity.magnitude;

        if (speed > effectiveMaxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized * effectiveMaxSpeed;
        }
    }
}