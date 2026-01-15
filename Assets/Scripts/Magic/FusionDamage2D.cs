using UnityEngine;

public class FusionDamage2D : MonoBehaviour
{
    public int damage;

    void OnTriggerEnter2D(Collider2D other)
    {
        var hp = other.GetComponent<EnemyHealth>();
        if (hp != null)
            hp.TakeDamage(damage);
    }
}
