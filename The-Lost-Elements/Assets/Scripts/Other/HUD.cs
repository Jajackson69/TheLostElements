using System;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Slider healthbar;
    private int maxHealth;
    private void SetupHealthbar(GameObject player)
    {
        healthbar.value = healthbar.maxValue;
        maxHealth = player.GetComponent<PlayerHealth>().maxHealth;
    }
    private void UpdateHealthbar(int currentHealth)
    {
        healthbar.value = (float)currentHealth / maxHealth;
        healthbar.value = Mathf.Clamp01(healthbar.value);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        GameController.OnPlayerSpawned += SetupHealthbar;
        PlayerHealth.OnPlayerTakeDamage += UpdateHealthbar;
    }

  

    // Update is called once per frame
    void OnDisable()
    {
        GameController.OnPlayerSpawned -= SetupHealthbar;
        PlayerHealth.OnPlayerTakeDamage -= UpdateHealthbar;
    }
}
