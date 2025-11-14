using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private int damage = 3;
    [SerializeField] private float hitInterval = 0.5f;

    [Header("Knockback")]
    [SerializeField] private Vector2 knockbackToPlayer = new Vector2(3f, 5f);
    [SerializeField] private Vector2 knockbackToSelf = new Vector2(3f, 5f);

    private Rigidbody2D rb;
    private bool canDamage = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            TryHitPlayer(col.collider.transform);
    }

    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            TryHitPlayer(col.collider.transform);
    }

    private void TryHitPlayer(Transform playerTransform)
    {
        if (!canDamage) return;

        canDamage = false;
        HitPlayer(playerTransform);
        StartCoroutine(ResetDamageCooldown());
    }

    private System.Collections.IEnumerator ResetDamageCooldown()
    {
        yield return new WaitForSeconds(hitInterval);
        canDamage = true;
    }

    public void HitPlayer(Transform playerTransform)
{
    if (playerTransform == null) return;

    int direction = playerTransform.position.x > transform.position.x ? 1 : -1;

    var playerController = playerTransform.GetComponent<PlayerController>();
    if (playerController != null)
    {
        Vector2 knockDir = new Vector2(direction * Mathf.Abs(knockbackToPlayer.x), knockbackToPlayer.y);
        playerController.ApplyKnockback(knockDir);
        playerController.DamagePlayer(damage);
    }
}

}
