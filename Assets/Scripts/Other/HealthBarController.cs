using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private PlayerController player;

    private int maxHealth;

    void Start()
{
    Debug.Log("HealthUI Init: healthFill = " + healthFill);
    Debug.Log("HealthUI Init: player = " + player);

    maxHealth = player != null ? player.MaxHealth : 100;
    PlayerController.OnPlayerTakeDamage += UpdateHealth;
}


    void OnDestroy()
    {
        PlayerController.OnPlayerTakeDamage -= UpdateHealth;
    }

    private void UpdateHealth(int current)
    {
        float value = Mathf.Clamp01((float)current / maxHealth);
        healthFill.fillAmount = value;
    }
}
