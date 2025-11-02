using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Références des boutons dans la scène")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("Référence du prefab UI-Hover Arrows")]
    [SerializeField] private GameObject uiHoverArrows;

    [Header("UI - Credits Panel")]
    [SerializeField] private GameObject creditsUIPrefab;
    private GameObject spawnedCreditsUI;



    [Header("Positions Y pour chaque bouton")]
    [SerializeField] private float playY = 1.329f;
    [SerializeField] private float offsetY = -1.279f; // Différence de hauteur entre chaque bouton

    private void Awake()
    {
        if (playButton == null) playButton = GameObject.Find("Play").GetComponent<Button>();
        if (settingsButton == null) settingsButton = GameObject.Find("Settings").GetComponent<Button>();
        if (creditsButton == null) creditsButton = GameObject.Find("Credits").GetComponent<Button>();
        if (quitButton == null) quitButton = GameObject.Find("Quit").GetComponent<Button>();

        if (uiHoverArrows == null)
        {
            uiHoverArrows = GameObject.Find("UI-Hover Arrows");
        }
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        settingsButton.onClick.AddListener(OnSettingsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        // Ajout des listeners de hover
        AddHoverListener(playButton, playY);
        AddHoverListener(settingsButton, playY + offsetY);
        AddHoverListener(creditsButton, playY + offsetY * 2);
        AddHoverListener(quitButton, playY + offsetY * 3);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);
        settingsButton.onClick.RemoveListener(OnSettingsClicked);
        creditsButton.onClick.RemoveListener(OnCreditsClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void AddHoverListener(Button button, float yPosition)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        // Quand on entre dans le bouton
        EventTrigger.Entry entryEnter = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entryEnter.callback.AddListener((_) => MoveHoverArrows(yPosition));
        trigger.triggers.Add(entryEnter);
    }

    private void MoveHoverArrows(float yPos)
    {
        if (uiHoverArrows != null)
        {
            Vector3 currentPos = uiHoverArrows.transform.localPosition;
            uiHoverArrows.transform.localPosition = new Vector3(currentPos.x, yPos, currentPos.z);
        }
    }

    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked");
        SceneManager.LoadScene("Gamemap1");
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings button clicked");
    }

   private void OnCreditsClicked()
{
    Debug.Log("Credits button clicked");

    if (creditsUIPrefab == null)
    {
        Debug.LogWarning("Aucun prefab UI Credits assigné !");
        return;
    }

    // Si déjà affiché, on évite de le recréer
    if (spawnedCreditsUI != null)
    {
        Debug.Log("UI Credits déjà affiché.");
        return;
    }

    // Trouver le Canvas dans la scène
    Canvas canvas = FindObjectOfType<Canvas>();
    if (canvas == null)
    {
        Debug.LogError("Aucun Canvas trouvé dans la scène !");
        return;
    }

    // Créer le UI sous le Canvas
    spawnedCreditsUI = Instantiate(creditsUIPrefab, canvas.transform);

    // Placer le panneau à la position souhaitée
    RectTransform rect = spawnedCreditsUI.GetComponent<RectTransform>();
    if (rect != null)
    {
        rect.anchoredPosition = new Vector2(-960.0067f, -540.0221f);
    }
    else
    {
        spawnedCreditsUI.transform.localPosition = new Vector3(-960.0067f, -540.0221f, 0);
    }
}



    private void OnQuitClicked()
    {
        Debug.Log("Quit button clicked");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
