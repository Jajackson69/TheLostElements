using UnityEngine;
using System.Collections.Generic;

public class EnvironmentReactor : MonoBehaviour
{
    public static EnvironmentReactor Instance { get; private set; }

    private List<ActivatableObject> allObjects = new List<ActivatableObject>();

    void Awake()
    {
        Instance = this;
        RefreshActivatables();
        Debug.Log($"EnvironmentReactor initialized — tracking {allObjects.Count} activatable objects");
    }

    public void RefreshActivatables()
    {
        allObjects.Clear();

        foreach (var root in gameObject.scene.GetRootGameObjects())
        {
            var found = root.GetComponentsInChildren<ActivatableObject>(true);
            allObjects.AddRange(found);
        }
    }

    public void UpdateElementHints(string activeElement)
    {
        Debug.Log(string.IsNullOrEmpty(activeElement)
            ? "No active element — hiding all hints"
            : $"Active Element: {activeElement}");

        foreach (var obj in allObjects)
        {
            if (obj == null) continue;
            bool show = !string.IsNullOrEmpty(activeElement) && obj.requiredSpell == activeElement;
            obj.ShowHint(show);
            Debug.Log($"{obj.name} reacts to {obj.requiredSpell} show={show}");
        }
    }
}
