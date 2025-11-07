using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int damage = 3;

    [Header("Knockback")]
    [SerializeField] private Vector2 knockbackToPlayer = new Vector2(3f, 5f);
    [SerializeField] private Vector2 knockbackToSelf   = new Vector2(3f, 5f);

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void HitPlayer(Transform playerTransform)
    {
        if (playerTransform == null) return;

        int direction = GetDirection(playerTransform); // 1 if player is right, -1 if left

        // knock the player using their own movement script
        var playerMove = playerTransform.GetComponent<PlayerMovement>();
        if (playerMove != null)
        {
            playerMove.KnockbackPlayer(knockbackToPlayer, direction);
        }

        // apply damage to the exact player we collided with
        var playerHealth = playerTransform.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.DamagePlayer(damage);
        }

        // self knockback using our rigidbody
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector2.zero;
#else
            rb.velocity = Vector2.zero;
#endif
            Vector2 force = new Vector2(-direction * Mathf.Abs(knockbackToSelf.x), knockbackToSelf.y);
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        // optional pause on your new AI if you add such a method
        // var ai = GetComponent<EnemyChaseSide2D>();
        // if (ai != null) ai.PauseFor(0.2f);
    }

    private int GetDirection(Transform playerTransform)
    {
        return (playerTransform.position.x > transform.position.x) ? 1 : -1;
    }

    // call HitPlayer automatically on collision with the player
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
        {
            HitPlayer(col.collider.transform);
        }
    }
}
