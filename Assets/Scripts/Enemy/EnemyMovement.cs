using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private SpriteRenderer spriteRender;
    [SerializeField] private Animator animator;
    [SerializeField] private float speed = 3f;
    [SerializeField] private int startDirection = 1;

    private int currentDirection;
    private float halfWidth;
    private Vector2 movement;

    void Awake()
    {
        if (!rigidBody) rigidBody = GetComponent<Rigidbody2D>();
        if (!spriteRender) spriteRender = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }

    void Start()
    {
        halfWidth = spriteRender.bounds.extents.x;
        currentDirection = startDirection;
        spriteRender.flipX = startDirection == 1 ? false : true;

        animator.SetBool("isRunning", true);
    }

    void FixedUpdate()
    {
        movement.x = speed * currentDirection;
        movement.y = rigidBody.linearVelocityY;
        rigidBody.linearVelocity = movement;

        bool isMoving = Mathf.Abs(rigidBody.linearVelocityX) > 0.05f;
        animator.SetBool("isRunning", isMoving);

        CheckDirectionChange();
    }

    public void KnockbackEnemy(Vector2 knockbackForce, int direction)
    {
        knockbackForce.x *= direction;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
        rigidBody.AddForce(knockbackForce, ForceMode2D.Impulse);
    }

    private void CheckDirectionChange()
    {
        if (Physics2D.Raycast(transform.position, Vector2.right, halfWidth + 0.1f, LayerMask.GetMask("Ground")) &&
            rigidBody.linearVelocityX > 0)
        {
            currentDirection *= -1;
            spriteRender.flipX = true;
        }
        else if (Physics2D.Raycast(transform.position, Vector2.left, halfWidth + 0.1f, LayerMask.GetMask("Ground")) &&
                 rigidBody.linearVelocityX < 0)
        {
            currentDirection *= -1;
            spriteRender.flipX = false;
        }

        Debug.DrawRay(transform.position, Vector2.right * (halfWidth + 0.1f), Color.red);
        Debug.DrawRay(transform.position, Vector2.left * (halfWidth + 0.1f), Color.red);
    }
}
