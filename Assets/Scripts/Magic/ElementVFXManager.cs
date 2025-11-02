using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ElementVFXManager : MonoBehaviour
{
    [System.Serializable]
    public class ElementVFX
    {
        public string elementName;
        public GameObject prefab;
        public VFXAnchorType anchorType = VFXAnchorType.Auto;

        [HideInInspector] public GameObject instance;
        [HideInInspector] public Transform currentParent;
        [HideInInspector] public int tapCount = 0;
        [HideInInspector] public float lastTapTime = 0f;
        [HideInInspector] public string activeTag = null;
        [HideInInspector] public bool canCrossFuse = true;
        [HideInInspector] public VFXAnchorType activeAnchorType = VFXAnchorType.Auto;
    }

    [System.Serializable]
    public class FusionRule
    {
        public string tagA;
        public string tagB;
        public GameObject fusionPrefab;
        public VFXAnchorType anchorType = VFXAnchorType.Auto;
    }

    public enum VFXAnchorType
    {
        Auto,
        Middle
    }

    [Header("References")]
    public PlayerController player;

    [Header("VFX Elements (keys 1–4)")]
    public List<ElementVFX> elementVFXs = new List<ElementVFX>();

    [Header("Fusion Rules")]
    public List<FusionRule> fusionRules = new List<FusionRule>();

    [Header("Settings")]
    public Vector3 offset = Vector3.zero;
    public float doubleTapDelay = 0.4f;

    private Dictionary<Key, int> keyToIndex;

    private void Awake()
    {
        keyToIndex = new Dictionary<Key, int>()
        {
            { Key.Digit1, 0 },
            { Key.Digit2, 1 },
            { Key.Digit3, 2 },
            { Key.Digit4, 3 }
        };
    }

    private void Update()
    {
        if (player == null) return;

        foreach (var kvp in keyToIndex)
            if (Keyboard.current[kvp.Key].wasPressedThisFrame)
                HandleTap(kvp.Value);

        UpdateVFXPositions();
        CheckForFusion();
    }

    private void HandleTap(int index)
    {
        if (index < 0 || index >= elementVFXs.Count) return;
        ElementVFX e = elementVFXs[index];
        float now = Time.time;

        if (now - e.lastTapTime <= doubleTapDelay)
        {
            e.tapCount++;
            if (e.tapCount == 2)
            {
                TrySelfFusion(e);
                e.tapCount = 0;
                e.lastTapTime = 0f;
                return;
            }
        }
        else
        {
            e.tapCount = 1;
        }
        e.lastTapTime = now;

        if (e.instance == null) ActivateVFX(e);
        else DeactivateVFX(e);
    }

    private void ActivateVFX(ElementVFX e)
    {
        Transform anchor = GetAnchor(e.anchorType);
        if (anchor == null) return;

        e.currentParent = anchor;
        e.activeAnchorType = e.anchorType;
        Vector3 spawnPos = anchor.position + offset;

        e.instance = Instantiate(e.prefab, spawnPos, anchor.rotation, anchor);
        SetLocalParticleMode(e.instance);

        e.instance.transform.localPosition = offset;
        e.instance.transform.localRotation = Quaternion.identity;

        e.activeTag = e.elementName;
        e.canCrossFuse = true;

        Debug.Log($"🟢 Activated {e.elementName} at {anchor.name}");
    }

    private void DeactivateVFX(ElementVFX e)
    {
        if (e.instance != null) Destroy(e.instance);
        e.instance = null;
        e.currentParent = null;
        e.activeTag = null;
        e.canCrossFuse = true;
        e.activeAnchorType = VFXAnchorType.Auto;
        Debug.Log($"🔴 Deactivated {e.elementName}");
    }

    private void TrySelfFusion(ElementVFX e)
    {
        string selfTag = e.elementName + "++";
        FusionRule rule = fusionRules.Find(f => f.tagA == e.elementName && f.tagB == e.elementName);
        GameObject prefab = rule != null ? rule.fusionPrefab : e.prefab;

        VFXAnchorType finalAnchorType = rule != null ? rule.anchorType : e.anchorType;
        Transform anchor = GetAnchor(finalAnchorType);
        if (anchor == null)
        {
            Debug.LogWarning($"⚠️ No anchor found for {e.elementName} self-fusion!");
            return;
        }

        if (e.instance != null) Destroy(e.instance);

        Vector3 pos = anchor.position + offset;
        e.instance = Instantiate(prefab, pos, anchor.rotation, anchor);
        SetLocalParticleMode(e.instance);

        e.instance.transform.localPosition = offset;
        e.instance.transform.localRotation = Quaternion.identity;
        e.currentParent = anchor;
        e.activeAnchorType = finalAnchorType;

        e.activeTag = selfTag;
        e.canCrossFuse = false;

        Debug.Log($"💧 Self-Fusion: {e.elementName}+{e.elementName} → {prefab.name} on {anchor.name}");
    }

    private void CheckForFusion()
    {
        var active = elementVFXs.FindAll(x => x.instance != null && x.canCrossFuse && !string.IsNullOrEmpty(x.activeTag));
        if (active.Count != 2) return;

        string tagA = active[0].activeTag;
        string tagB = active[1].activeTag;

        FusionRule rule = fusionRules.Find(f =>
            (f.tagA == tagA && f.tagB == tagB) ||
            (f.tagA == tagB && f.tagB == tagA));

        if (rule == null) return;

        foreach (var e in active)
        {
            Destroy(e.instance);
            e.instance = null;
            e.currentParent = null;
            e.activeTag = null;
        }

        Transform anchor = GetAnchor(rule.anchorType);
        if (anchor == null) return;

        Vector3 pos = anchor.position + offset;
        GameObject fusionInstance = Instantiate(rule.fusionPrefab, pos, anchor.rotation, anchor);
        SetLocalParticleMode(fusionInstance);

        fusionInstance.transform.localPosition = offset;
        fusionInstance.transform.localRotation = Quaternion.identity;

        Debug.Log($"⚡ Fusion {tagA}+{tagB} → {rule.fusionPrefab.name} on {anchor.name}");
    }

    private void UpdateVFXPositions()
    {
        foreach (var e in elementVFXs)
        {
            if (e.instance == null) continue;

            Transform targetParent = GetAnchor(e.activeAnchorType);
            if (targetParent == null) continue;

            if (e.currentParent != targetParent)
            {
                e.currentParent = targetParent;
                e.instance.transform.SetParent(targetParent);
                e.instance.transform.localPosition = offset;
                e.instance.transform.localRotation = Quaternion.identity;
                Debug.Log($"🔄 Reattached {e.elementName} to {targetParent.name}");
            }
        }
    }

    private Transform GetAnchor(VFXAnchorType type)
    {
        return type switch
        {
            VFXAnchorType.Middle => player.firePointMiddle != null ? player.firePointMiddle : player.currentFirePoint,
            _ => player.currentFirePoint
        };
    }

    private void SetLocalParticleMode(GameObject obj)
    {
        foreach (var ps in obj.GetComponentsInChildren<ParticleSystem>())
        {
            var main = ps.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
        }
    }
}
