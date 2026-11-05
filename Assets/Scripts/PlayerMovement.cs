using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private new Camera camera;
    private new Rigidbody2D rigidbody;
    private new Collider2D collider;

    private Vector2 velocity;
    private float inputAxis;
    private readonly ContactPoint2D[] contacts = new ContactPoint2D[24];
    private ContactFilter2D terrainFilter;
    private LevelCamera levelCamera;
    [Range(0f, .25f)] public float coyoteTime = .1f;
    [Range(0f, .25f)] public float jumpBuffer = .12f;
    private float groundGrace, bufferedJump;
    private bool jumpConsumed;

    public float moveSpeed = 8f;
    public float maxJumpHeight = 5f;
    public float maxJumpTime = 1f;
    public float jumpForce => (2f * maxJumpHeight) / (maxJumpTime / 2f);
    public float gravity => (-2f * maxJumpHeight) / Mathf.Pow(maxJumpTime / 2f, 2f);

    public bool grounded { get; private set; }
    public bool jumping { get; private set; }
    public bool running => Mathf.Abs(velocity.x) > 0.25f || Mathf.Abs(inputAxis) > 0.25f;
    public bool sliding => (inputAxis > 0f && velocity.x < 0f) || (inputAxis < 0f && velocity.x > 0f);
    public bool falling => velocity.y < 0f && !grounded;

    private void Awake()
    {
        camera = Camera.main;
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        terrainFilter = new ContactFilter2D { useLayerMask = true, layerMask = LayerMask.GetMask("Default"), useTriggers = false };
        levelCamera = camera.GetComponent<LevelCamera>();
    }

    private void OnEnable()
    {
        rigidbody.isKinematic = false;
        collider.enabled = true;
        velocity = Vector2.zero;
        jumping = false;
        groundGrace = bufferedJump = 0;
        jumpConsumed = false;
    }

    private void OnDisable()
    {
        rigidbody.isKinematic = true;
        collider.enabled = false;
        velocity = Vector2.zero;
        jumping = false;
    }

    private void Update()
    {
        if(Time.timeScale==0)return;
        ProcessInput(Input.GetAxisRaw("Horizontal"), Input.GetButtonDown("Jump"), Input.GetButton("Jump"), Time.deltaTime);
    }

    private void ProcessInput(float axis, bool jumpPressed, bool jumpHeld, float deltaTime)
    {
        grounded = false;
        int count = rigidbody.GetContacts(terrainFilter, contacts);
        for (int i = 0; i < count; i++) {
            if (contacts[i].normal.y > 0.65f && contacts[i].point.y < collider.bounds.center.y) grounded = true;
        }
        HorizontalMovement(axis, deltaTime);
        if (grounded && velocity.y <= 0) jumpConsumed = false;
        groundGrace = grounded && !jumpConsumed ? coyoteTime : Mathf.Max(0, groundGrace - deltaTime);
        bufferedJump = jumpPressed ? jumpBuffer : Mathf.Max(0, bufferedJump - deltaTime);
        if (grounded && velocity.y < 0) velocity.y = 0;
        if (bufferedJump > 0 && groundGrace > 0 && !jumpConsumed) {
            GroundedMovement(true);
            jumpConsumed = true;
            groundGrace = bufferedJump = 0;
        }
        ApplyGravity(jumpHeld, deltaTime);
    }

    private void FixedUpdate()
    {
        // move mario based on his velocity
        Vector2 position = rigidbody.position;
        position += velocity * Time.fixedDeltaTime;

        // Camera smoothing must never push the player into a wall.
        position.x = Mathf.Clamp(position.x, 0.5f, levelCamera != null ? levelCamera.end - 0.5f : float.PositiveInfinity);

        rigidbody.MovePosition(position);
    }

    private void HorizontalMovement(float axis, float deltaTime)
    {
        // accelerate / decelerate
        inputAxis = axis;
        velocity.x = Mathf.MoveTowards(velocity.x, inputAxis * moveSpeed, 55f * deltaTime);

        // flip sprite to face direction
        if (velocity.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        } else if (velocity.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void GroundedMovement(bool jumpPressed)
    {
        // prevent gravity from infinitly building up
        velocity.y = Mathf.Max(velocity.y, 0f);
        jumping = velocity.y > 0f;

        // perform jump
        if (jumpPressed)
        {
            velocity.y = jumpForce;
            jumping = true;
            MarioAudio.Play("jump");
        }
    }

    private void ApplyGravity(bool jumpHeld, float deltaTime)
    {
        // check if falling
        bool falling = velocity.y < 0f || !jumpHeld;
        float multiplier = falling ? 2f : 1f;

        // apply gravity and terminal velocity
        velocity.y += gravity * multiplier * deltaTime;
        velocity.y = Mathf.Max(velocity.y, gravity / 2f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // bounce off enemy head
            if (HasNormal(collision, 0.65f, false))
            {
                velocity.y = jumpForce / 2f;
                jumping = true;
            }
        }
        else if (collision.gameObject.layer != LayerMask.NameToLayer("PowerUp"))
        {
            // stop vertical movement if mario bonks his head
            if (velocity.y > 0f && HasNormal(collision, -0.65f, true)) {
                velocity.y = 0f;
            }
        }
    }

    private static bool HasNormal(Collision2D collision, float threshold, bool ceiling)
    {
        for (int i = 0; i < collision.contactCount; i++) {
            float y = collision.GetContact(i).normal.y;
            if (ceiling ? y < threshold : y > threshold) return true;
        }
        return false;
    }

}
