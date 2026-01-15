using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DeathUIManager : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private const string MAIN_MENU_SCENE = "Windscene";

    void Awake()
    {
        if (deathUI != null)
            deathUI.SetActive(false);

        restartButton.onClick.AddListener(RestartScene);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void ShowDeathUI()
    {
        if (deathUI != null)
            deathUI.SetActive(true);
    }

    private void RestartScene()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
