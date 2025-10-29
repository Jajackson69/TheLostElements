using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Element { Fire, Water, Earth }

[Serializable]
public struct ComboKey : IEquatable<ComboKey>
{
    public int fire, water, earth;
    public ComboKey(int f, int w, int e) { fire = f; water = w; earth = e; }
    public bool Equals(ComboKey other) => fire == other.fire && water == other.water && earth == other.earth;
    public override bool Equals(object obj) => obj is ComboKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(fire, water, earth);
    public override string ToString() => $"F{fire}W{water}E{earth}";
}

public class SpellManager : MonoBehaviour
{
    [Header("UI (optional)")]
    [SerializeField] private Image fireIcon;
    [SerializeField] private Image waterIcon;
    [SerializeField] private Image earthIcon;
    [SerializeField] private Image resultIcon;

    [Header("FirePoint (auto)")]
    [SerializeField] private Transform firePoint;              // leave empty, auto filled
    [SerializeField] private string firePointChildName = "FirePoint";

    [Header("Ability assets (assign in Inspector)")]
    // 10 total spells
    [SerializeField] private AbilitySO infernoBurst;    // F3 W0 E0
    [SerializeField] private AbilitySO tidalSurge;      // F0 W3 E0
    [SerializeField] private AbilitySO quakePillar;     // F0 W0 E3
    [SerializeField] private AbilitySO scaldingJet;     // F2 W1 E0
    [SerializeField] private AbilitySO magmaBoulder;    // F2 W0 E1
    [SerializeField] private AbilitySO boilingGeyser;   // F1 W2 E0
    [SerializeField] private AbilitySO mudWave;         // F0 W2 E1
    [SerializeField] private AbilitySO moltenRock;      // F1 W0 E2
    [SerializeField] private AbilitySO quicksandTrap;   // F0 W1 E2
    [SerializeField] private AbilitySO elementalNova;   // F1 W1 E1

    private readonly List<Element> picks = new List<Element>(3);
    private Dictionary<ComboKey, AbilitySO> recipes;
    private AbilitySO currentAbility;
    private bool crafted;

    void OnEnable()
    {
        // hook to player spawn, supports runtime spawned player
        GameController.OnPlayerSpawned += HookPlayerRefs;
        if (GameController.CurrentPlayer != null) HookPlayerRefs(GameController.CurrentPlayer);
        else
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) HookPlayerRefs(p);
        }
    }

    void OnDisable()
    {
        GameController.OnPlayerSpawned -= HookPlayerRefs;
    }

    void HookPlayerRefs(GameObject player)
    {
        if (firePoint) return;
        Transform fp = player.transform.Find(firePointChildName);
        if (!fp)
        {
            foreach (var t in player.GetComponentsInChildren<Transform>(true))
            {
                if (string.Equals(t.name, firePointChildName, StringComparison.OrdinalIgnoreCase))
                {
                    fp = t;
                    break;
                }
            }
        }
        firePoint = fp;
        if (!firePoint) Debug.LogWarning("SpellManager: FirePoint not found on Player.");
    }

    void Start()
    {
        // build recipe table, order insensitive, duplicates supported
        recipes = new Dictionary<ComboKey, AbilitySO>
        {
            { new ComboKey(3,0,0), infernoBurst },   // F F F
            { new ComboKey(0,3,0), tidalSurge },     // W W W
            { new ComboKey(0,0,3), quakePillar },    // E E E
            { new ComboKey(2,1,0), scaldingJet },    // F F W
            { new ComboKey(2,0,1), magmaBoulder },   // F F E
            { new ComboKey(1,2,0), boilingGeyser },  // W W F
            { new ComboKey(0,2,1), mudWave },        // W W E
            { new ComboKey(1,0,2), moltenRock },     // E E F
            { new ComboKey(0,1,2), quicksandTrap },  // E E W
            { new ComboKey(1,1,1), elementalNova },  // F W E
        };

        Toggle(fireIcon, false);
        Toggle(waterIcon, false);
        Toggle(earthIcon, false);
        Toggle(resultIcon, false);
    }

    void Update()
    {
        // picks, allow duplicates, max 3
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryAdd(Element.Fire, fireIcon);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryAdd(Element.Water, waterIcon);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryAdd(Element.Earth, earthIcon);

        // craft
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Alpha5)) TryCraft();

        // cast
        if (crafted && Input.GetKeyDown(KeyCode.Mouse0)) CastCurrent();

        // reset
        if (Input.GetKeyDown(KeyCode.Alpha0)) ResetSelection();
    }

    void TryAdd(Element e, Image icon)
    {
        if (crafted) return;
        if (picks.Count >= 3) return;
        picks.Add(e);
        Toggle(icon, true); // simple feedback
    }

    void TryCraft()
    {
        if (crafted) return;
        if (picks.Count != 3)
        {
            Debug.Log("Pick exactly 3 elements before crafting.");
            return;
        }

        int f = 0, w = 0, e = 0;
        foreach (var p in picks)
        {
            if (p == Element.Fire) f++;
            else if (p == Element.Water) w++;
            else e++;
        }

        var key = new ComboKey(f, w, e);
        if (recipes.TryGetValue(key, out var ability) && ability != null)
        {
            currentAbility = ability;
            crafted = true;
            Toggle(resultIcon, true);
            Debug.Log("Crafted: " + ability.abilityId + "  Key " + key);
        }
        else
        {
            Debug.LogWarning("No recipe for combo " + key);
            crafted = false;
            currentAbility = null;
        }

        // prepare for next craft input
        picks.Clear();
        Toggle(fireIcon, false);
        Toggle(waterIcon, false);
        Toggle(earthIcon, false);
    }

    void CastCurrent()
    {
        if (currentAbility == null || currentAbility.prefab == null)
        {
            Debug.LogWarning("No ability to cast.");
            return;
        }

        // spawn pos
        Vector3 spawnPos;
        if (currentAbility.spawnAtFirePoint)
        {
            if (!firePoint) { Debug.LogWarning("No FirePoint."); return; }
            spawnPos = firePoint.position;
        }
        else
        {
            var m = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m.z = 0f;
            spawnPos = m;
        }

        // aim dir
        var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouse.z = 0f;
        Vector2 dir = (mouse - spawnPos).normalized;

        var go = Instantiate(currentAbility.prefab, spawnPos, Quaternion.identity);

        if (currentAbility.faceMouse) go.transform.right = dir;

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb && currentAbility.speed > 0f)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            // Unity 6 uses linearVelocity, older uses velocity
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = dir * currentAbility.speed;
#else
            rb.velocity = dir * currentAbility.speed;
#endif
        }

        // damage component, use your own class name here
        var dmg = go.GetComponent<SpellDamage>(); // or ProjectileDamage2D
        if (dmg) dmg.Init(currentAbility.damage);

        if (currentAbility.lifetime > 0f) Destroy(go, currentAbility.lifetime);

        crafted = false;
        currentAbility = null;
        Toggle(resultIcon, false);
    }

    void ResetSelection()
    {
        picks.Clear();
        crafted = false;
        currentAbility = null;
        Toggle(fireIcon, false);
        Toggle(waterIcon, false);
        Toggle(earthIcon, false);
        Toggle(resultIcon, false);
        Debug.Log("Selection cleared.");
    }

    void Toggle(Image img, bool on)
    {
        if (img) img.enabled = on;
    }
}
