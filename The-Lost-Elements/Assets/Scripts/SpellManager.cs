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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasFire = true;
            fireIcon.enabled = true;
            Debug.Log("🔥 Fire selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasWater = true;
            waterIcon.enabled = true;
            Debug.Log("💧 Water selected");
        }

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

        if (fireballPrefab != null && firePoint != null)
        {
            // Spawn fireball
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

            // Calculate direction towards mouse
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f; // keep in 2D
            Vector2 direction = (mouseWorld - firePoint.position).normalized;

            // Apply velocity
            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                rb.linearVelocity = direction * 10f; // speed 10
            }

            // Rotate fireball to face direction (optional)
            fireball.transform.right = direction;

            Destroy(fireball, 3f); // clean up after 3 seconds
        }
    }
}
