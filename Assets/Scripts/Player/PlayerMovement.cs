using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    // public parameters 
    public float groundMaxSpeed = 10f;

    public float airMaxSpeed = 10f;

    public float groundAcceleration = 15f;

    [Tooltip("low = slide, high = precise controls")]
    public float airAcceleration = 800f;

    [Tooltip("high = quick stop, low = slide")]
    public float friction = 8f;

    [Header("Jump")]
    public float jumpForce = 8f;

    [Tooltip("hold space to bunny hop")]
    public bool autoHop = true;

    [Header("Air Control")]
    public bool enableAirControl = true;

    [Tooltip("0 = none, 1 = full air control")]
    [Range(0f, 1f)]
    public float airControlAmount = 0.3f;

    public float gravity = 20f;

    public float maxFallSpeed = 40f;

    // private variables
    CharacterController _cc;
    private Inventory inventory;

    Vector3 _velocity = Vector3.zero;

    bool _isGrounded;
    bool _wasGrounded;
    bool _jumpQueued;

    bool _justJumped;

    // public properties
    public float HorizontalSpeed =>
        new Vector3(_velocity.x, 0f, _velocity.z).magnitude;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
        inventory = GetComponent<Inventory>();
    }

    private void Start() => UpdateStats();

    private void OnEnable() => inventory.OnInventoryChanged += UpdateStats;

    private void OnDisable() => inventory.OnInventoryChanged -= UpdateStats;

    void Update()
    {
        if (autoHop)
            _jumpQueued = Input.GetButton("Jump");
        else if (Input.GetButtonDown("Jump"))
            _jumpQueued = true;

        _isGrounded = _cc.isGrounded;

        if (_isGrounded)
        {
            MoveGrounded();
        }
        else
        {
            MoveAirborne();
        }

        ApplyGravity();

        _cc.Move(_velocity * Time.deltaTime);

        _wasGrounded = _isGrounded;
    }

    // ground logic
    void MoveGrounded()
    {
        if (!_wasGrounded)
            _justJumped = false;

        if (_velocity.y < 0f)
            _velocity.y = -1f;

        Vector3 wishDir = GetWishDir();
        float wishSpeed = wishDir.magnitude * groundMaxSpeed;
        wishDir = wishDir.normalized;

        ApplyFriction(ref _velocity, friction);

        Accelerate(ref _velocity, wishDir, wishSpeed, groundAcceleration);

        if (_jumpQueued)
        {
            _velocity.y = jumpForce;
            _jumpQueued = false;
            _justJumped = true;
        }
    }

    // air logic
    void MoveAirborne()
    {
        Vector3 wishDir = GetWishDir();
        float wishSpeed = wishDir.magnitude * airMaxSpeed;
        wishDir = wishDir.normalized;

        AirAccelerate(ref _velocity, wishDir, wishSpeed, airAcceleration);

        if (enableAirControl && wishDir.sqrMagnitude > 0.01f)
            ApplyAirControl(ref _velocity, wishDir, airControlAmount);
    }

    // gravitation
    void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _velocity.y -= gravity * Time.deltaTime;
            _velocity.y = Mathf.Max(_velocity.y, -maxFallSpeed);
        }
    }

    // physics
    void ApplyFriction(ref Vector3 vel, float frictionValue)
    {
        Vector3 hVel = new Vector3(vel.x, 0f, vel.z);
        float speed = hVel.magnitude;

        if (speed < 0.001f) { vel.x = vel.z = 0f; return; }

        float drop = speed * frictionValue * Time.deltaTime;
        float newSpeed = Mathf.Max(speed - drop, 0f);
        float scale = newSpeed / speed;

        vel.x *= scale;
        vel.z *= scale;
    }

    void Accelerate(ref Vector3 vel, Vector3 wishDir, float wishSpeed, float accel)
    {
        // Projection of velocity on wishDir
        float currentSpeed = Vector3.Dot(new Vector3(vel.x, 0f, vel.z), wishDir);

        float addSpeed = wishSpeed - currentSpeed;
        if (addSpeed <= 0f) return;

        float accelSpeed = Mathf.Min(accel * wishSpeed * Time.deltaTime, addSpeed);

        vel.x += wishDir.x * accelSpeed;
        vel.z += wishDir.z * accelSpeed;
    }

    void AirAccelerate(ref Vector3 vel, Vector3 wishDir, float wishSpeed, float accel)
    {
        // On cap wishSpeed for air
        float cappedWishSpeed = Mathf.Min(wishSpeed, 30f);

        float currentSpeed = Vector3.Dot(new Vector3(vel.x, 0f, vel.z), wishDir);
        float addSpeed = cappedWishSpeed - currentSpeed;

        if (addSpeed <= 0f) return;

        float accelSpeed = Mathf.Min(accel * wishSpeed * Time.deltaTime, addSpeed);

        vel.x += wishDir.x * accelSpeed;
        vel.z += wishDir.z * accelSpeed;
    }

    void ApplyAirControl(ref Vector3 vel, Vector3 wishDir, float amount)
    {
        float input_x = Input.GetAxisRaw("Horizontal");
        float input_z = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(input_x) > 0.01f) return; // on strafe : no air control

        Vector3 hVel = new Vector3(vel.x, 0f, vel.z);
        float speed = hVel.magnitude;
        if (speed < 0.001f) return;

        float dot = Vector3.Dot(hVel.normalized, wishDir);
        if (dot <= 0f) return;

        float turnSpeed = amount * dot * dot * Time.deltaTime * speed;
        Vector3 newDir = Vector3.RotateTowards(hVel.normalized, wishDir, turnSpeed, 0f);

        vel.x = newDir.x * speed;
        vel.z = newDir.z * speed;
    }

    // helpers
    Vector3 GetWishDir()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Relative au Transform du joueur (qui tourne avec la souris en Y)
        Vector3 dir = transform.right * h + transform.forward * v;

        // Clamp magnitude à 1 (diagonale)
        if (dir.magnitude > 1f) dir.Normalize();

        return dir;
    }

    void UpdateStats()
    {
        // Calculate Speed
        float groundSpeedBoost = inventory.GetTotalBoost(StatType.groundSpeed);
        groundMaxSpeed = groundMaxSpeed * (1 + groundSpeedBoost);

        float airSpeedBoost = inventory.GetTotalBoost(StatType.airSpeed);
        airMaxSpeed = airMaxSpeed * (1 + airSpeedBoost);

        // Calculate Jump
        float jumpBoost = inventory.GetTotalBoost(StatType.JumpForce);
        jumpForce = jumpForce * (1 + jumpBoost);

        Debug.Log($"Stats Updated! ground speed: {groundMaxSpeed}, air speed : {airMaxSpeed}, Jump: {jumpForce}");
    }

    // debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, new Vector3(_velocity.x, 0f, _velocity.z));
    }
}