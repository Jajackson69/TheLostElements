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
    [SerializeField] private UnityEngine.UI.Text dotsText;

    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private float typingSpeed = 0.03f;
    [SerializeField] private float dotsInterval = 0.4f;

    private int currentLineIndex;
    private bool isTyping;
    private bool isComplete;
    private Coroutine typingCoroutine;
    private Coroutine dotsCoroutine;
    private PlayerControls controls;
    private bool initialized;

    void Awake()
    {
        controls = null;
    }

    void Start()
{
    StartCoroutine(InitDialogue());
}

IEnumerator InitDialogue()
{
    yield return null;
    yield return null;
    controls = new PlayerControls();
    if (dotsText != null) dotsText.text = "";
    if (dialogueText != null) dialogueText.text = "";
    controls.Dialogue.Enable();
    controls.Dialogue.Next.performed += OnAdvancePressed;
    StartDialogue();
}


    void OnDisable()
    {
        if (controls != null)
        {
            controls.Dialogue.Next.performed -= OnAdvancePressed;
            controls.Dialogue.Disable();
        }
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
            StopDots();
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
        StopDots();

        foreach (char c in lineText)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
        isComplete = true;

        if (currentLineIndex < dialogueLines.Length - 1)
            StartDots();
        else
            StartCoroutine(AutoEndAfterDelay());
    }

    IEnumerator AutoEndAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        EndDialogue();
    }

    void StartDots()
    {
        if (dotsText == null) return;
        if (dotsCoroutine != null)
            StopCoroutine(dotsCoroutine);
        dotsCoroutine = StartCoroutine(DotsLoop());
    }

    void StopDots()
    {
        if (dotsCoroutine != null)
            StopCoroutine(dotsCoroutine);
        dotsCoroutine = null;
        if (dotsText != null)
            dotsText.text = "";
    }

    IEnumerator DotsLoop()
    {
        int count = 0;
        while (!isTyping && isComplete)
        {
            if (dotsText != null)
                dotsText.text = new string('.', count);
            count = (count + 1) % 4;
            yield return new WaitForSeconds(dotsInterval);
        }
        if (dotsText != null)
            dotsText.text = "";
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
            StartDots();
        else
            StartCoroutine(AutoEndAfterDelay());
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
        StopDots();
        gameObject.SetActive(false);
    }
}
