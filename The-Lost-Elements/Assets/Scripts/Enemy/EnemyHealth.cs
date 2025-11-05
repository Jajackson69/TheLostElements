using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    private Enemy enemy; // reference to Enemy script (optional for death logic)

    void Awake()
    {
        currentHealth = maxHealth;
        enemy = GetComponent<Enemy>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage, HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        if (enemy != null)
            enemy.Die(); // uses your existing Enemy.Die()
        else
            Destroy(gameObject);
    }
}
