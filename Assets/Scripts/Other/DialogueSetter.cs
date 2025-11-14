using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private bool triggerOnce = true;

    [Header("NPC Movement Before Dialogue")]
    [SerializeField] private Transform npcToMove;
    [SerializeField] private Transform startTarget;
    [SerializeField] private float startMoveSpeed = 3f;
    [SerializeField] private bool moveNpcBeforeDialogue = false;

    [Header("NPC Movement After Dialogue")]
    [SerializeField] private Transform endTarget;
    [SerializeField] private float endMoveSpeed = 3f;
    [SerializeField] private bool moveNpcAfterDialogue = false;

    private bool hasTriggered;
    private bool movingToStart;
    private bool movingToEnd;
    private bool waitingForDialogueEnd = false;


    void Awake()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnEnable()
    {
        DialogueSystem.OnDialogueFinished += OnDialogueFinished;
    }


void OnTriggerEnter2D(Collider2D other)
{
    if (hasTriggered && triggerOnce) return;
    if (!other.CompareTag("Player")) return;

    hasTriggered = true;
    waitingForDialogueEnd = true;

    DialogueSystem.OnDialogueFinished += HandleDialogueEnd;

    if (moveNpcBeforeDialogue && npcToMove != null && startTarget != null)
        movingToStart = true;
    else
        StartDialogue();
}

void HandleDialogueEnd()
{
    if (!waitingForDialogueEnd) return;

    waitingForDialogueEnd = false;
    DialogueSystem.OnDialogueFinished -= HandleDialogueEnd;

    if (moveNpcAfterDialogue && npcToMove != null && endTarget != null)
        movingToEnd = true;
}

void OnDisable()
{
    DialogueSystem.OnDialogueFinished -= HandleDialogueEnd;
}

    void Update()
    {
        if (movingToStart)
            MoveNPC(npcToMove, startTarget, startMoveSpeed, () => StartDialogue());

        if (movingToEnd)
            MoveNPC(npcToMove, endTarget, endMoveSpeed, () => movingToEnd = false);
    }

    void StartDialogue()
    {
        movingToStart = false;

        if (dialogueSystem != null)
        {
            dialogueSystem.gameObject.SetActive(true);
            dialogueSystem.StartCustomDialogue(dialogueLines);
        }
    }

    void OnDialogueFinished()
    {
        if (moveNpcAfterDialogue && npcToMove != null && endTarget != null)
            movingToEnd = true;
    }

    void MoveNPC(Transform npc, Transform target, float speed, System.Action onArrive)
    {
        npc.position = Vector3.MoveTowards(
            npc.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(npc.position, target.position) < 0.05f)
            onArrive?.Invoke();
    }
}
