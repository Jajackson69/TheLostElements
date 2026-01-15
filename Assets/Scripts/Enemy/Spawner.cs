using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Camera mainCamera; // optional, not used yet but can be kept
    public GameObject enemyPrefab;

    [Header("Spawn Area")]
    public float spawnDistance = 18f;        // how far from player enemies spawn (outside screen)
    public float groundCheckDistance = 8f;   // how far down we raycast to find ground
    public LayerMask groundLayer;

    [Header("Enemy Limit")]
    public int maxEnemies = 10;              // max enemies allowed in scene
    public string enemyTag = "Enemy";        // make sure your enemy prefab uses this tag

    [Header("Avoid Overlap")]
    public float minDistanceBetweenEnemies = 2f; // no spawn if another enemy is too close
    public LayerMask enemyLayer;                 // set to your Enemy layer in inspector

    [Header("Spawn Timing")]
    public float spawnInterval = 3f;

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        yield return new WaitForSeconds(5f);
        while (true)
        {
            TrySpawnEnemy();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void TrySpawnEnemy()
    {
        if (!player) return;

        // 1. Check limit
        if (CurrentEnemyCount() >= maxEnemies)
            return;

        // 2. Try to find a valid position a few times
        for (int i = 0; i < 10; i++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            Vector2 spawnPos = (Vector2)player.position + randomDir * spawnDistance;

            // raycast down to find a platform
            RaycastHit2D hit = Physics2D.Raycast(spawnPos, Vector2.down, groundCheckDistance, groundLayer);

            if (hit.collider != null)
            {
                Vector3 finalPos = new Vector3(spawnPos.x, hit.point.y + 1f, 0f);

                // 3. Check if there is already an enemy too close
                if (Physics2D.OverlapCircle(finalPos, minDistanceBetweenEnemies, enemyLayer) != null)
                {
                    // too close to another enemy, try another position
                    continue;
                }

                // 4. Spawn enemy
                Instantiate(enemyPrefab, finalPos, enemyPrefab.transform.rotation);
                return;
            }
        }

        Debug.Log("EnemySpawner: No valid spawn position found after several tries.");
    }

    int CurrentEnemyCount()
    {
        // simplest way: count tagged enemies in scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        return enemies.Length;
    }

    // just to visualize spawn radius in editor
    private void OnDrawGizmosSelected()
    {
        if (!player) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(player.position, spawnDistance);
    }
}
