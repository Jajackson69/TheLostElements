using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rigidBody;

    private Vector2 movement;
    private Vector2 screenBounds;
    private float playerHalfWidth;
    private float xPosLastFrame;

    private void Awake()
    {
        if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    private void Start()
    {
        // Convert screen size to world units
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        playerHalfWidth = spriteRenderer.bounds.extents.x;
        xPosLastFrame = transform.position.x;
    }

    private void Update()
    {
        HandleMovement();
        ClampMovement();
        FlipCharacter();
    }

    private void HandleMovement()
    {
        // -1, 0, 1 from A/D or Left/Right
        float input = Input.GetAxisRaw("Horizontal");
        movement.Set(input * speed * Time.deltaTime, 0f);
        transform.Translate(movement);

        animator.SetBool("isRunning", input != 0f);
    }

    public void KnockbackPlayer(Vector2 knockbackForce, int direction)
    {
        knockbackForce.x *= direction;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    private void ClampMovement()
    {
        float clampedX = Mathf.Clamp(
            transform.position.x,
            -screenBounds.x + playerHalfWidth,
            screenBounds.x - playerHalfWidth
        );

        Vector3 pos = transform.position;
        pos.x = clampedX;
        transform.position = pos;
    }

    private void FlipCharacter()
    {
        float dx = transform.position.x - xPosLastFrame;

        if (dx > 0.001f)
            spriteRenderer.flipX = false; // moving right
        else if (dx < -0.001f)
            spriteRenderer.flipX = true;  // moving left

        xPosLastFrame = transform.position.x;
    }
}
