using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        DeathUIManager deathUI = FindFirstObjectByType<DeathUIManager>();
        if (deathUI != null)
            deathUI.ShowDeathUI();
    }
}
