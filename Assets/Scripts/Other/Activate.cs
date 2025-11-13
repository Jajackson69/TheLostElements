using UnityEngine;

public class ActivatableObject : MonoBehaviour
{
    public string requiredSpell;
    public GameObject objectToActivate;
    public GameObject glowMarker;
    private bool isActive;

    void Awake()
    {
        if (objectToActivate == null)
            objectToActivate = gameObject;

        if (objectToActivate != null)
            objectToActivate.SetActive(isActive);

        if (glowMarker != null)
            glowMarker.SetActive(false);
    }

    public void TryActivate(string spellName)
    {
        if (isActive) return;

        if (spellName == requiredSpell)
        {
            isActive = true;
            objectToActivate.SetActive(true);
            if (glowMarker != null)
                glowMarker.SetActive(false);
        }
    }

    public void ShowHint(bool show)
    {
        if (glowMarker != null)
            glowMarker.SetActive(show && !isActive);
    }
}
