using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [SerializeField] private float knockbackDuration = 0.2f;
    private bool isKnockedback = false;

    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [SerializeField] private int maxHealth = 100;
    public int MaxHealth => maxHealth;

    private int currentHealth;
    public static Action<int> OnPlayerTakeDamage;

    public float bounceForce = 6f;
    public float bounceForceMultiplier = 1.3f;

    public Transform firePointRight;
    public Transform firePointLeft;
    public Transform firePointMiddle;
    [HideInInspector] public Transform currentFirePoint;

    [SerializeField] private Fairy fairyController;
    [SerializeField] private DialogueSystem dialogueSystem;

    [SerializeField] private ElementVFXManager vfxManager;

    [SerializeField] private GameObject deathUI;

    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    public PlayerControls Controls => controls;

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

        bool isRunning = Mathf.Abs(moveInput.x) > 0.1f && isGrounded && !isKnockedback;

        if (isRunning && !playingFootsteps)
            StartFootsteps();
        else if (!isRunning && playingFootsteps)
            StopFootsteps();

        UpdateJumpAndFallAnimations();
        FlipSprite();
    }

    private void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    private void PlayFootstep()
    {
        float pitch = 2f + UnityEngine.Random.Range(0f, 0.5f);
        SoundEffectManager.Play("footsteps", pitch);
    }

    private void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    public void ApplyKnockback(Vector2 knockDir)
    {
        StartCoroutine(KnockbackCoroutine(knockDir));
    }

    private IEnumerator KnockbackCoroutine(Vector2 knockDir)
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

    public void DamagePlayer(int damage)
    {
        currentHealth -= damage;
        OnPlayerTakeDamage?.Invoke(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        moveInput = Vector2.zero;

        if (controls != null)
            controls.Disable();

        animator.ResetTrigger("death");
        animator.SetTrigger("death");

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(1.2f);
        deathUI.SetActive(true);
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
