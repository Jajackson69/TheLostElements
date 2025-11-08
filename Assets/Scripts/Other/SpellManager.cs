using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class SpellIconCombiner : MonoBehaviour
{
    [Header("Element Icons")]
    [SerializeField] private Image fireIcon;
    [SerializeField] private Image waterIcon;
    [SerializeField] private Image steamIcon;

    [Header("Spell Prefabs")]
    [SerializeField] private GameObject fireballPrefab;

    [Header("Runtime Refs")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private string firePointChildName = "FirePoint";

    private bool hasFire = false;
    private bool hasWater = false;
    private bool craftedSteam = false;

    private PlayerControls controls;

    private void OnEnable()
    {
        controls = new PlayerControls();
        controls.Spells.Enable();

        controls.Spells.Fire.performed += OnFireSelect;
        controls.Spells.Water.performed += OnWaterSelect;
        controls.Spells.Combine.performed += OnCombine;
        controls.Spells.Cast.performed += OnCast;
    }

    private void OnDisable()
    {
        controls.Spells.Fire.performed -= OnFireSelect;
        controls.Spells.Water.performed -= OnWaterSelect;
        controls.Spells.Combine.performed -= OnCombine;
        controls.Spells.Cast.performed -= OnCast;
        controls.Spells.Disable();
    }

    private void Start()
    {
        if (firePoint == null)
        {
            Transform fp = transform.Find(firePointChildName);
            if (fp == null)
            {
                foreach (var t in GetComponentsInChildren<Transform>(true))
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
                Debug.LogWarning("SpellIconCombiner could not find FirePoint. Check the child name.");
        }
    }

    private void OnFireSelect(InputAction.CallbackContext context)
    {
        hasFire = true;
        if (fireIcon) fireIcon.enabled = true;
        Debug.Log("🔥 Fire selected");
    }

    private void OnWaterSelect(InputAction.CallbackContext context)
    {
        hasWater = true;
        if (waterIcon) waterIcon.enabled = true;
        Debug.Log("💧 Water selected");
    }

    private void OnCombine(InputAction.CallbackContext context)
    {
        if (hasFire && hasWater)
        {
            craftedSteam = true;
            if (steamIcon) steamIcon.enabled = true;
            if (fireIcon) fireIcon.enabled = false;
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

    private void OnCast(InputAction.CallbackContext context)
    {
        if (craftedSteam)
            CastSteamBurst();
        else
            Debug.Log("⚠️ No crafted ability!");
    }

    private void CastSteamBurst()
    {
        if (!fireballPrefab || !firePoint)
        {
            Debug.LogWarning("Steam cast skipped, missing prefab or FirePoint.");
            return;
        }

        Debug.Log("☁️ Casted Steam Burst!");
        craftedSteam = false;
        if (steamIcon) steamIcon.enabled = false;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorld.z = 0f;
        Vector2 dir = (mouseWorld - firePoint.position).normalized;

        var rb = fireball.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.linearVelocity = dir * 10f;
        }

        fireball.transform.right = dir;
        Destroy(fireball, 3f);
    }
}
