using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Vector2 knockbackToSelf = new Vector2(3f, 5f);
    [SerializeField] private Vector2 knockbackToPlayer = new Vector2(3f, 5f);
    [SerializeField] private int damage = 3;

    public void Die()
    {
        Destroy(gameObject);
    }

    public void HitPlayer(Transform playerTransform)
    {
        int direction = GetDirection(playerTransform);

        var playerMove = FindAnyObjectByType<PlayerMovement>();
        if (playerMove != null)
            playerMove.KnockbackPlayer(knockbackToPlayer, direction);

        var playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
            playerHealth.DamagePlayer(damage);

        var enemyMove = GetComponent<EnemyMovement>();
        if (enemyMove != null)
            enemyMove.KnockbackEnemy(knockbackToSelf, -direction);
    }

    private int GetDirection(Transform playerTransform)
    {
        return (transform.position.x > playerTransform.position.x) ? -1 : 1;
    }
}
