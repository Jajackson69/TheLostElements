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
    public Transform firePoint;
    public float firePointRadius = 0.6f;

    private float lastXPos;

    public PlayerControls Controls => controls;

    private void Awake()
    {
        controls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
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
        // Vérifie si le joueur touche le sol
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Applique le mouvement horizontal
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Animation "isRunning"
        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > 0.1f);

        // Flip le sprite selon la direction
        FlipSprite();

        // Positionne et oriente le FirePoint vers la souris
        UpdateFirePoint();
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
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack!");
        animator.SetTrigger("attack");
    }

    private void FlipSprite()
    {
        if (moveInput.x > 0.05f && !facingRight)
        {
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < -0.05f && facingRight)
        {
            facingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    private void UpdateFirePoint()
    {
        if (firePoint == null) return;

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;

        Vector3 dir = (mouseWorld - transform.position).normalized;
        firePoint.position = transform.position + dir * firePointRadius;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
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

        // Si on tombe sur l'ennemi
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
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(firePoint.position, 0.1f);
        }
    }
} 

