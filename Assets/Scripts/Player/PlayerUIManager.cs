using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    [Header("UI-Hover Arrows")]
    [SerializeField] private GameObject uiHoverArrows;

    [Header("UI - Credits Panel")]
    [SerializeField] private GameObject creditsUIPrefab;
    
    [Header("UI - Controls Panel")]
    [SerializeField] private GameObject controlsUIPrefab;

    private GameObject spawnedCreditsUI;
    private GameObject spawnedControlsUI;


    [Header("Y positions for each buttons")]
    [SerializeField] private float playY = 1.329f;
    [SerializeField] private float offsetY = -1.279f;

    private void Awake()
    {
        if (playButton == null) playButton = GameObject.Find("Play").GetComponent<Button>();
        if (controlsButton == null) controlsButton = GameObject.Find("Controls").GetComponent<Button>();
        if (creditsButton == null) creditsButton = GameObject.Find("Credits").GetComponent<Button>();
        if (quitButton == null) quitButton = GameObject.Find("Quit").GetComponent<Button>();
        if (uiHoverArrows == null) uiHoverArrows = GameObject.Find("UI-Hover Arrows");
    }

    private void OnEnable()
    {
        playButton.onClick.AddListener(OnPlayClicked);
        controlsButton.onClick.AddListener(OnControlsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        AddHoverListener(playButton, playY);
        AddHoverListener(controlsButton, playY + offsetY);
        AddHoverListener(creditsButton, playY + offsetY * 2);
        AddHoverListener(quitButton, playY + offsetY * 3);
    }

    private void OnDisable()
    {
        playButton.onClick.RemoveListener(OnPlayClicked);
        controlsButton.onClick.RemoveListener(OnControlsClicked);
        creditsButton.onClick.RemoveListener(OnCreditsClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    private void AddHoverListener(Button button, float yPosition)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null) trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryEnter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
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
        SceneManager.LoadScene("Gamemap1");
    }

    private void OnControlsClicked()
{
    if (controlsUIPrefab == null)
    {
        Debug.LogWarning("No UI Controls found!");
        return;
    }

    if (spawnedControlsUI != null)
    {
        Debug.Log("UI Controls already on.");
        return;
    }

    Canvas canvas = FindFirstObjectByType<Canvas>();
    if (canvas == null)
    {
        Debug.LogError("No Canvas found in the scene!");
        return;
    }

    spawnedControlsUI = Instantiate(controlsUIPrefab, canvas.transform);

    RectTransform rect = spawnedControlsUI.GetComponent<RectTransform>();
    if (rect != null)
        rect.anchoredPosition = new Vector2(-960.0067f, -540.0221f);
    else
        spawnedControlsUI.transform.localPosition = new Vector3(-960.0067f, -540.0221f, 0);

    Button closeButton = spawnedControlsUI.GetComponentInChildren<Button>();
    if (closeButton != null)
        closeButton.onClick.AddListener(CloseControls);
}


    private void OnCreditsClicked()
    {
        if (creditsUIPrefab == null)
        {
            Debug.LogWarning("No UI Credits found!");
            return;
        }

        if (spawnedCreditsUI != null)
        {
            Debug.Log("UI Credits already on.");
            return;
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }

        spawnedCreditsUI = Instantiate(creditsUIPrefab, canvas.transform);

        RectTransform rect = spawnedCreditsUI.GetComponent<RectTransform>();
        if (rect != null)
            rect.anchoredPosition = new Vector2(-960.0067f, -540.0221f);
        else
            spawnedCreditsUI.transform.localPosition = new Vector3(-960.0067f, -540.0221f, 0);

        Button closeButton = spawnedCreditsUI.GetComponentInChildren<Button>();
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseCredits);
    }

    private void CloseControls()
    {
        if (spawnedControlsUI != null)
        {
            spawnedControlsUI.SetActive(false);
            Destroy(spawnedControlsUI);
            spawnedControlsUI = null;
        }
    }

    private void CloseCredits()
    {
        if (spawnedCreditsUI != null)
        {
            spawnedCreditsUI.SetActive(false);
            Destroy(spawnedCreditsUI);
            spawnedCreditsUI = null;
        }
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
