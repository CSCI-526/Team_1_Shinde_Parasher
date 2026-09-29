using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float maxSpeed = 10f;

    [Header("Weight Effect")]
    [SerializeField] private float weightAccelerationEffect = 0.25f;
    [SerializeField] private float downhillWeightEffect = 0.6f;
    [SerializeField] private float downhillMaxSpeedBonus = 1.5f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private float maxTiltAngle = 30f;
    [SerializeField] private float uprightRecoverySpeed = 40f;

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
        HandleRotation(rotateInput);
        HandleBraking(moveInput);
        LimitSpeed();
    }

    private void HandleMovement(float moveInput)
    {
        if (Mathf.Abs(moveInput) < 0.01f)
            return;

        float currentWeight = weightSystem.GetCurrentWeight();

        // Heavier car = slower acceleration.
        float weightMultiplier =
            1f / (1f + (currentWeight - 1f) * weightAccelerationEffect);

        float currentAcceleration =
            acceleration * weightMultiplier;

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

        float extraWeight =
            Mathf.Max(0f, currentWeight - 1f);

        if (extraWeight <= 0f)
            return;

        float downhillGravity =
            Vector2.Dot(Physics2D.gravity, transform.right);

        if (downhillGravity > 0f)
        {
            float downhillForce =
                downhillGravity *
                extraWeight *
                downhillWeightEffect;

            rb.AddForce(transform.right * downhillForce);
        }
    }

    private void HandleRotation(float rotateInput)
    {
        if (Mathf.Abs(rotateInput) < 0.01f)
            return;

        float currentAngle =
            Mathf.DeltaAngle(0f, rb.rotation);

        // Rotate the car using W/S.
        float newAngle =
            currentAngle +
            rotateInput *
            rotationSpeed *
            Time.fixedDeltaTime;

        // Prevent the car from becoming completely vertical.
        newAngle =
            Mathf.Clamp(
                newAngle,
                -maxTiltAngle,
                maxTiltAngle
            );

        rb.MoveRotation(newAngle);
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
        float currentWeight =
            weightSystem.GetCurrentWeight();

        float extraWeight =
            Mathf.Max(0f, currentWeight - 1f);

        float effectiveMaxSpeed =
            maxSpeed;

        float downhillGravity =
            Vector2.Dot(
                Physics2D.gravity,
                transform.right
            );

        if (downhillGravity > 0f)
        {
            effectiveMaxSpeed +=
                extraWeight *
                downhillMaxSpeedBonus;
        }

        float speed =
            rb.linearVelocity.magnitude;

        if (speed > effectiveMaxSpeed)
        {
            rb.linearVelocity =
                rb.linearVelocity.normalized *
                effectiveMaxSpeed;
        }
    }
}