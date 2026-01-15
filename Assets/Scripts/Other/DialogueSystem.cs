using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System;

[Serializable]
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

    private PlayerControls controls;

    public static Action OnDialogueFinished;


    // --- LIFECYCLE ---
    private void Awake()
    {
        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Dialogue.Enable();
        controls.Dialogue.Next.performed += OnAdvancePressed;
    }

    private void OnDisable()
    {
        controls.Dialogue.Next.performed -= OnAdvancePressed;
        controls.Dialogue.Disable();
    }

    private void Start()
    {
        StartCoroutine(InitDialogue());
    }


    // --- INIT ---
    IEnumerator InitDialogue()
    {
        yield return null;
        yield return null;

        if (dialogueText != null)
            dialogueText.text = "";

        StartDialogue();
    }


    // --- FLOW ---
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


    // --- INPUT ---
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


    // --- TEXT ADVANCE ---
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


    // --- DISPLAY ---
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


    // --- END ---
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

        OnDialogueFinished?.Invoke();
    }


    // --- UI ---
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
}
