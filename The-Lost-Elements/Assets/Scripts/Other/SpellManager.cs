using UnityEngine;
using UnityEngine.UI;
using System;

public class SpellIconCombiner : MonoBehaviour
{
    [Header("Element Icons")]
    [SerializeField] private Image fireIcon;
    [SerializeField] private Image waterIcon;
    [SerializeField] private Image steamIcon;

    [Header("Spell Prefabs")]
    [SerializeField] private GameObject fireballPrefab;

    [Header("Runtime refs")]
    [SerializeField] private Transform firePoint;                 // will be filled at runtime
    [SerializeField] private string firePointChildName = "FirePoint";

    private bool hasFire = false;
    private bool hasWater = false;
    private bool craftedSteam = false;

    void OnEnable()
    {
        // hook to player spawn
        GameController.OnPlayerSpawned += HookPlayerRefs;

        // if player already exists, hook right away
        if (GameController.CurrentPlayer != null)
            HookPlayerRefs(GameController.CurrentPlayer);
        else
        {
            // fallback, try find by tag
            var existing = GameObject.FindGameObjectWithTag("Player");
            if (existing) HookPlayerRefs(existing);
        }
    }

    void OnDisable()
    {
        GameController.OnPlayerSpawned -= HookPlayerRefs;
    }

    private void HookPlayerRefs(GameObject player)
    {
        if (player == null) return;

        // find the FirePoint child by name
        if (firePoint == null)
        {
            Transform fp = player.transform.Find(firePointChildName);
            if (fp == null)
            {
                // last resort, scan children
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

            if (firePoint == null)
                Debug.LogWarning("SpellIconCombiner could not find FirePoint on the Player. Check the child name.");
        }
    }

    void Start()
    {
        if (fireIcon)  fireIcon.enabled  = false;
        if (waterIcon) waterIcon.enabled = false;
        if (steamIcon) steamIcon.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasFire = true;
            if (fireIcon) fireIcon.enabled = true;
            Debug.Log("🔥 Fire selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasWater = true;
            if (waterIcon) waterIcon.enabled = true;
            Debug.Log("💧 Water selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasFire && hasWater)
            {
                craftedSteam = true;
                if (steamIcon) steamIcon.enabled = true;
                if (fireIcon)  fireIcon.enabled  = false;
                if (waterIcon) waterIcon.enabled = false;

                hasFire = false;
                hasWater = false;

                Debug.Log("🔥 + 💧 = ☁️ Crafted Steam Burst!");
            }
            else
            {
                Debug.Log("⚠️ Need Fire + Water to craft!");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (craftedSteam) CastSteamBurst();
            else Debug.Log("⚠️ No crafted ability!");
        }
    }

    void CastSteamBurst()
    {
        if (!fireballPrefab || !firePoint)
        {
            Debug.LogWarning("Steam cast skipped, missing prefab or FirePoint.");
            return;
        }

        Debug.Log("☁️ Casted Steam Burst!");
        craftedSteam = false;
        if (steamIcon) steamIcon.enabled = false;

        // spawn fireball
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // direction to mouse
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - firePoint.position).normalized;

        // push projectile
        var rb = fireball.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = dir * 10f;   // speed
        }

        fireball.transform.right = dir;  // face direction
        Destroy(fireball, 3f);
    }
}
