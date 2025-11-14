using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathUIManager : MonoBehaviour
{
    [SerializeField] private GameObject deathUI;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;

    private const string GAME_SCENE = "Gamemap1";
    private const string MAIN_MENU_SCENE = "Windscene";

    void Awake()
    {
        if (deathUI != null)
            deathUI.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    public void ShowDeathUI()
    {
        if (deathUI != null)
            deathUI.SetActive(true);
    }

    void RestartGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }
}
