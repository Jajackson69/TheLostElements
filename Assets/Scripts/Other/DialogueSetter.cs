using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSystem dialogueSystem;
    [SerializeField] private DialogueLine[] dialogueLines;
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered && triggerOnce) return;
        if (!other.CompareTag("Player")) return;

        if (dialogueSystem == null)
        {
            Debug.LogWarning(" DialogueSystem not assigned to DialogueTrigger.");
            return;
        }

        hasTriggered = true;
        dialogueSystem.gameObject.SetActive(true);
        dialogueSystem.StartCustomDialogue(dialogueLines);

        Debug.Log($" Dialogue triggered: {name}");
    }
}
