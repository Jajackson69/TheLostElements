using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour

{
    [SerializeField] private int health = 10;
    public int currentHealth { get; private set; }
    public int maxHealth { get; private set; }
    public static Action<int> OnPlayerTakeDamage;
    // Start is called once before the firdt execution of Update after the MonoBehaviour is created
    void Awake()
    {
        currentHealth = health;
        maxHealth = health;
    }

    public void DamagePlayer(int damageAmount)
    {
        currentHealth -= damageAmount;
        OnPlayerTakeDamage?.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
