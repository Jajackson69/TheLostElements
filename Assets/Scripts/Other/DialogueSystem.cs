using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

[System.Serializable]
public class DialogueLine
{
    public string text;
    public bool requiresAction;
    public string requiredActionId;
}

public class DialogueSystem : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Text dialogueText;
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private Animator mouseAnimator;

    private int currentLineIndex;
    private bool isTyping;
    private bool isComplete;
    private Coroutine typingCoroutine;

    private static PlayerControls sharedControls;
    private static bool inputHooked;

    void Start()
    {
        StartCoroutine(InitDialogue());
    }

    IEnumerator InitDialogue()
    {
        yield return null;
        yield return null;

        if (sharedControls == null)
            sharedControls = new PlayerControls();

        if (!inputHooked)
        {
            sharedControls.Dialogue.Next.performed += OnAdvancePressed;
            inputHooked = true;
        }

        sharedControls.Dialogue.Enable();

        if (dialogueText != null)
            dialogueText.text = "";

        StartDialogue();
    }

    

    public void StartDialogue()
    {
        StartCoroutine(SafeStartDialogue());
    }

    IEnumerator SafeStartDialogue()
    {
        yield return null;
        currentLineIndex = 0;
        DisplayLine();
    }

    DialogueLine GetCurrentLine()
    {
        if (dialogueLines == null || currentLineIndex < 0 || currentLineIndex >= dialogueLines.Length)
            return null;
        return dialogueLines[currentLineIndex];
    }

    void OnAdvancePressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!gameObject.activeInHierarchy) return;

        DialogueLine line = GetCurrentLine();
        if (line == null) return;
        if (line.requiresAction) return;
        InternalAdvance();
    }

    public void AdvanceFromAction(string actionId)
    {
        DialogueLine line = GetCurrentLine();
        if (line == null) return;
        if (!line.requiresAction) return;
        if (!string.IsNullOrEmpty(line.requiredActionId) && line.requiredActionId != actionId)
            return;
        InternalAdvance();
    }

    void InternalAdvance()
    {
        if (isTyping)
            CompleteLineInstantly();
        else if (isComplete)
        {
            HideMouseHint();
            NextLine();
        }
    }

    void DisplayLine()
    {
        DialogueLine line = GetCurrentLine();
        if (line == null)
        {
            EndDialogue();
            return;
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string lineText)
    {
        isTyping = true;
        isComplete = false;
        dialogueText.text = "";
        HideMouseHint();

        foreach (char c in lineText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isComplete = true;

        if (currentLineIndex < dialogueLines.Length - 1)
            ShowMouseHint();
        else
            StartCoroutine(AutoEndAfterDelay());
    }

    void CompleteLineInstantly()
    {
        DialogueLine line = GetCurrentLine();
        if (line == null) return;
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);
        dialogueText.text = line.text;
        isTyping = false;
        isComplete = true;

        if (currentLineIndex < dialogueLines.Length - 1)
            ShowMouseHint();
        else
            StartCoroutine(AutoEndAfterDelay());
    }

    IEnumerator AutoEndAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        EndDialogue();
    }

    void ShowMouseHint()
    {
        if (mouseAnimator != null)
            mouseAnimator.SetBool("ClickHint", true);
    }

    void HideMouseHint()
    {
        if (mouseAnimator != null)
            mouseAnimator.SetBool("ClickHint", false);
    }

    void NextLine()
    {
        currentLineIndex++;
        DisplayLine();
    }

    public void StartCustomDialogue(DialogueLine[] newLines)
    {
        dialogueLines = newLines;
        currentLineIndex = 0;
        gameObject.SetActive(true);
        StartCoroutine(SafeStartDialogue());
    }

    void EndDialogue()
    {
        isTyping = false;
        isComplete = false;
        if (dialogueText != null)
            dialogueText.text = "";
        HideMouseHint();
        if (mouseAnimator != null)
            mouseAnimator.SetTrigger("Hide");
        gameObject.SetActive(false);
    }
}
