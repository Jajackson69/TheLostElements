using UnityEngine;

public class SpellDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private string targetTag = "Enemy"; // Set in Inspector
    [SerializeField] private bool destroyOnHit = true;

    public void Init(int dmg) { damage = dmg; }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(targetTag)) return;

        var enemyHP = other.GetComponent<EnemyHealth>();
        if (enemyHP != null)
        {
            enemyHP.TakeDamage(damage);
        }

        if (destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
