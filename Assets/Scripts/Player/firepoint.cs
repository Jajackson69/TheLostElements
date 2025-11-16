using UnityEngine;

public class FirePoint : MonoBehaviour
{
    [SerializeField] private Transform player;      // reference to the player
    [SerializeField] private Transform firePoint;   // the FirePoint child
    [SerializeField] private float radius = 0.6f;   // distance from player

    void LateUpdate()
    {
        // Get mouse position in world
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;

        // Direction from player to mouse
        Vector3 dir = (mouseWorld - player.position).normalized;

        // Place firePoint on a circle around player
        firePoint.position = player.position + dir * radius;

        // Rotate firePoint so firePoint.right points to mouse
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }
}
