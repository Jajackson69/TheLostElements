using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] private Transform firePoint; 
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifetime = 3f;

    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
            Launch();
    }

    void Launch()
    {
        if (firePoint == null || projectilePrefab == null)
            return;

        // Spawn projectile
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Mouse world position
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0f));
        mouseWorld.z = 0f;

        // Direction firePoint → mouse
        Vector2 dir = ((Vector2)(mouseWorld - firePoint.position)).normalized;

        // Apply velocity
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = dir * speed;
        }

        // Rotate projectile to face direction (optional)
        proj.transform.right = dir;

        // Auto destroy
        Destroy(proj, lifetime);
    }
}
