using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathUIManager : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private Button respawnButton;
    [SerializeField] private Button mainMenuButton;

    void Awake()
    {
        if (deathUI != null)
            deathUI.SetActive(false);

        respawnButton.onClick.AddListener(RestartScene);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void ShowDeathUI()
    {
        if (deathUI != null)
            deathUI.SetActive(true);
    }

    public void HideDeathUI()
    {
        if (deathUI != null)
            deathUI.SetActive(false);
    }

    private void RestartScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene("Windscene");
    }
}
