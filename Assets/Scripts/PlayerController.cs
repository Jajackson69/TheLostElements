using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private Vector2 moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;
    public static Action<int> OnPlayerTakeDamage;

    [Header("Bounce Settings")]
    public float bounceForce = 6f;
    public float bounceForceMultiplier = 1.3f;

    [Header("Fire Point Settings")]
    public Transform firePointRight;
    public Transform firePointLeft;
    public Transform firePointMiddle; 

    [HideInInspector] public Transform currentFirePoint;

    public PlayerControls Controls => controls;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Default firepoint
        if (firePointRight != null)
            currentFirePoint = firePointRight;
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
        controls.Player.Attack.performed += OnAttack;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.performed -= OnJump;
        controls.Player.Attack.performed -= OnAttack;
        controls.Player.Disable();
    }

    private void Update()
    {
        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Move
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Animation: running
        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > 0.1f);

        // Jump/fall animations
        UpdateJumpAndFallAnimations();

        // Handle facing direction and firepoint
        FlipSprite();
    }

    private void UpdateJumpAndFallAnimations()
    {
        float velY = rb.linearVelocity.y;

        if (!isGrounded && velY > 0.1f)
        {
            animator.SetBool("isJumping", true);
            animator.SetBool("isFalling", false);
        }
        else if (!isGrounded && velY < -0.1f)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            animator.SetBool("isJumping", true);
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("attack");
    }

    // 🔁 Flip direction & switch current firepoint (no disabling!)
    private void FlipSprite()
    {
        if (moveInput.x > 0.05f && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
            currentFirePoint = firePointRight;
        }
        else if (moveInput.x < -0.05f && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
            currentFirePoint = firePointLeft;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            CollideWithEnemy(other);
        }
    }

    private void CollideWithEnemy(Collision2D other)
    {
        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null) return;

        if (Physics2D.Raycast(transform.position, Vector2.down, spriteRenderer.bounds.extents.y + 0.1f, LayerMask.GetMask("Enemy")))
        {
            Vector2 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;

            float force = bounceForce;
            if (controls.Player.Jump.ReadValue<float>() > 0.5f)
                force *= bounceForceMultiplier;

            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            enemy.Die();
        }
        else
        {
            enemy.HitPlayer(transform);
        }
    }

    public void DamagePlayer(int damage)
    {
        currentHealth -= damage;
        OnPlayerTakeDamage?.Invoke(currentHealth);
        if (currentHealth <= 0)
            Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (firePointRight != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePointRight.position, 0.1f);
        }

        if (firePointLeft != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(firePointLeft.position, 0.1f);
        }
    }
}
