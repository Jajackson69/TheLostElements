using System;
using UnityEngine;
using System.Collections;
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

    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedback = false;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;

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

    [Header("Chat Settings")]
    [SerializeField] private Fairy fairyController;
    [SerializeField] private DialogueSystem dialogueSystem;

    [Header("Magic Settings")]
    [SerializeField] private ElementVFXManager vfxManager;
    [SerializeField] private GameObject deathScreen;


    public PlayerControls Controls => controls;

private Vector3 respawnPoint;

private void Start()
{
    respawnPoint = transform.position; // first spawn point
}

public void Respawn()
{
    if (deathScreen != null)
    deathScreen.SetActive(false);
    
    currentHealth = maxHealth;
    OnPlayerTakeDamage?.Invoke(currentHealth);

    transform.position = respawnPoint;

    GetComponent<SpriteRenderer>().enabled = true;
    GetComponent<Collider2D>().enabled = true;

#if UNITY_6000_0_OR_NEWER
    rb.linearVelocity = Vector2.zero;
#else
    rb.velocity = Vector2.zero;
#endif
}

public void SetRespawnPoint(Vector3 point)
{
    respawnPoint = point;
}

    private void Awake()
    {
        if (controls == null)
            controls = new PlayerControls();

        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (firePointRight != null)
            currentFirePoint = firePointRight;
    }

    private void OnEnable()
    {
        if (controls == null) return;

        controls.Player.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
        controls.Player.Jump.performed += OnJump;
        controls.Player.Attack.performed += OnAttack;
        controls.Player.SpawnChat.performed += OnSpawnChat;
    }

    private void OnDisable()
    {
        if (controls == null) return;

        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Player.Jump.performed -= OnJump;
        controls.Player.Attack.performed -= OnAttack;
        controls.Player.SpawnChat.performed -= OnSpawnChat;
        controls.Player.Disable();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

#if UNITY_6000_0_OR_NEWER
if (!isKnockedback)
    rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
#else
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
#endif

        animator.SetBool("isRunning", Mathf.Abs(moveInput.x) > 0.1f);
        UpdateJumpAndFallAnimations();
        FlipSprite();
    }


public void ApplyKnockback(Vector2 knockDir)
{
    StartCoroutine(KnockbackCoroutine(knockDir));
}

private System.Collections.IEnumerator KnockbackCoroutine(Vector2 knockDir)
{
    isKnockedback = true;

#if UNITY_6000_0_OR_NEWER
    rb.linearVelocity = Vector2.zero;
    rb.AddForce(knockDir, ForceMode2D.Impulse);
#else
    rb.velocity = Vector2.zero;
    rb.AddForce(knockDir, ForceMode2D.Impulse);
#endif

    yield return new WaitForSeconds(knockbackDuration);
    isKnockedback = false;
}

    private void UpdateJumpAndFallAnimations()
    {
#if UNITY_6000_0_OR_NEWER
        float velY = rb.linearVelocity.y;
#else
        float velY = rb.velocity.y;
#endif

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
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
#else
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
#endif

            animator.SetBool("isJumping", true);

            if (dialogueSystem != null)
                dialogueSystem.AdvanceFromAction("Jump");
        }
    }

    private void OnAttack(InputAction.CallbackContext context)
    {
        animator.SetTrigger("attack");
    }

    private void OnSpawnChat(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (fairyController == null) return;
        fairyController.SpawnBubble();
    }

    private void OnElement1(InputAction.CallbackContext context)
    {
        if (!context.performed || vfxManager == null) return;
        if (dialogueSystem != null) dialogueSystem.AdvanceFromAction("Crimsonova");
    }

    private void OnElement2(InputAction.CallbackContext context)
    {
        if (!context.performed || vfxManager == null) return;
        if (dialogueSystem != null) dialogueSystem.AdvanceFromAction("SkyLume");
    }

    private void OnElement3(InputAction.CallbackContext context)
    {
        if (!context.performed || vfxManager == null) return;
        if (dialogueSystem != null) dialogueSystem.AdvanceFromAction("Mysthaze");
    }

    private void OnElement4(InputAction.CallbackContext context)
    {
        if (!context.performed || vfxManager == null) return;
        if (dialogueSystem != null) dialogueSystem.AdvanceFromAction("BlushBloom");
    }

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
        if (!other.gameObject.CompareTag("Enemy")) return;

        Enemy enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy == null) return;

        bool stomp = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            spriteRenderer.bounds.extents.y + 0.1f,
            LayerMask.GetMask("Enemy")
        );

        if (stomp)
        {
#if UNITY_6000_0_OR_NEWER
            Vector2 velocity = rb.linearVelocity;
#else
            Vector2 velocity = rb.velocity;
#endif
            velocity.y = 0f;

#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = velocity;
#else
            rb.velocity = velocity;
#endif

            float force = bounceForce;
            if (controls.Player.Jump.ReadValue<float>() > 0.5f)
                force *= bounceForceMultiplier;

            rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
            enemy.Die();
        }
    }

   public void DamagePlayer(int damage)
{
    currentHealth -= damage;
    OnPlayerTakeDamage?.Invoke(currentHealth);

    if (currentHealth <= 0)
        Die();
}

void Die()
{
    rb.bodyType = RigidbodyType2D.Static;

    moveInput = Vector2.zero;

    if (controls != null)
        controls.Disable();

    animator.ResetTrigger("death");
    animator.SetTrigger("death");

    StartCoroutine(ResetDeathTriggerNextFrame());
    StartCoroutine(DeathSequence());
}

private IEnumerator ResetDeathTriggerNextFrame()
{
    yield return null;
    animator.ResetTrigger("death");
}

private IEnumerator DeathSequence()
{
    yield return new WaitForSeconds(0.84f);

    if (deathScreen != null)
        deathScreen.SetActive(true);
    else
        Debug.LogError("deathScreen reference missing!");
}

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
