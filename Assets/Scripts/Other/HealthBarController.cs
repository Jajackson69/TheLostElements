using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    [Header("UI references")]
    [SerializeField] private Image healthBar;          
    [SerializeField] private Image healthBackground;  

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [Header("Fluidity")]
    [SerializeField] private float lerpSpeed = 5f;    

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBarInstant();
    }

    private void Update()
    {
        float fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHealth / maxHealth, Time.deltaTime * lerpSpeed);
        healthBar.fillAmount = fillAmount;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthBarInstant();
    }

    private void UpdateHealthBarInstant()
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
