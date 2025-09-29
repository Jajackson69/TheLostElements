using UnityEngine;
using UnityEngine.UI;

public class SpellIconCombiner : MonoBehaviour
{
    [Header("Element Icons")]
    [SerializeField] private Image fireIcon;
    [SerializeField] private Image waterIcon;
    [SerializeField] private Image steamIcon;

    [Header("Spell Prefabs")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private Transform firePoint; // Fireball spawn point
    [SerializeField] private SpriteRenderer playerSprite; // Player's SpriteRenderer

    private bool hasFire = false;
    private bool hasWater = false;
    private bool craftedSteam = false;

    void Start()
    {
        fireIcon.enabled = false;
        waterIcon.enabled = false;
        steamIcon.enabled = false;
    }

    void Update()
    {
        // Select Fire
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasFire = true;
            fireIcon.enabled = true;
            Debug.Log("🔥 Fire selected");
        }

        // Select Water
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasWater = true;
            waterIcon.enabled = true;
            Debug.Log("💧 Water selected");
        }

        // Craft ability
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasFire && hasWater)
            {
                craftedSteam = true;
                steamIcon.enabled = true;
                fireIcon.enabled = false;
                waterIcon.enabled = false;

                hasFire = false;
                hasWater = false;

                Debug.Log("🔥 + 💧 = ☁️ Crafted Steam Burst!");
            }
            else
            {
                Debug.Log("⚠️ Need Fire + Water to craft!");
            }
        }

        // Cast ability
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (craftedSteam)
            {
                CastSteamBurst();
            }
            else
            {
                Debug.Log("⚠️ No crafted ability!");
            }
        }
    }

    void CastSteamBurst()
    {
        Debug.Log("☁️ Casted Steam Burst!");
        craftedSteam = false;
        steamIcon.enabled = false;

        // Spawn Fireball prefab
        if (fireballPrefab != null && firePoint != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            // Decide shooting direction
            float direction = playerSprite.flipX ? -1f : 1f;

            // Move fireball
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = new Vector2(10f * direction, 0f); // shoot left or right
            }

            // Flip fireball sprite if going left
            SpriteRenderer fbSprite = fireball.GetComponent<SpriteRenderer>();
            if (fbSprite != null && direction < 0)
            {
                fbSprite.flipX = true;
            }
        }
    }
}
