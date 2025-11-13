using UnityEngine;
using UnityEngine.InputSystem;

public class SpellActivator : MonoBehaviour
{
    [SerializeField] private ElementVFXManager vfxManager;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private string glowTag = "GlowMarker";
    [SerializeField] private DialogueSystem dialogueSystem;


    private PlayerControls controls;

   private void Awake()
{
    if (controls == null)
        controls = new PlayerControls();
}

private void OnEnable()
{
    if (controls == null) return;

    controls.Player.Enable();
    controls.Player.ActivateSpell.performed += OnActivateSpell;
}

private void OnDisable()
{
    if (controls == null) return;

    controls.Player.ActivateSpell.performed -= OnActivateSpell;
    controls.Player.Disable();
}


    void OnActivateSpell(InputAction.CallbackContext ctx)
    {
        Debug.Log(" Right-click detected, attempting activation...");
        TryActivateUnderMouse();
    }

    void TryActivateUnderMouse()
    {
        if (vfxManager == null)
        {
            Debug.LogWarning(" VFX Manager missing.");
            return;
        }

        string activeElement = null;
        foreach (var e in vfxManager.elementVFXs)
        {
            if (e.instance != null && !string.IsNullOrEmpty(e.activeTag))
            {
                activeElement = e.elementName;
                break;
            }
        }

        if (string.IsNullOrEmpty(activeElement))
        {
            Debug.Log(" No active element — no spell selected.");
            return;
        }

        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(mouseScreen);
        Vector2 point = new Vector2(mouseWorld.x, mouseWorld.y);

        Debug.Log($" Mouse world position: {point}");

        Collider2D hit = Physics2D.OverlapPoint(point);

        if (hit == null)
        {
            Debug.Log(" No collider detected under mouse.");
            return;
        }

        Debug.Log($" Hit {hit.name} (tag: {hit.tag})");

        if (!hit.CompareTag(glowTag))
        {
            Debug.Log(" Object hit is NOT a GlowMarker.");
            return;
        }

ActivatableObject activatable = null;
GlowReference refScript = hit.GetComponent<GlowReference>();
if (refScript != null)
    activatable = refScript.target;
else
    activatable = hit.GetComponentInParent<ActivatableObject>();

        if (activatable == null)
        {
            Debug.Log(" GlowMarker has no parent ActivatableObject.");
            return;
        }

        Debug.Log($" Attempting to activate {activatable.name} with spell {activeElement}");

        activatable.TryActivate(activeElement);
        vfxManager.DeactivateElementByName(activeElement);

        if (dialogueSystem != null)
        {
            dialogueSystem.AdvanceFromAction("SpellCraft");
        }


        Debug.Log($" Activation complete for {activatable.name}");
    }
}
