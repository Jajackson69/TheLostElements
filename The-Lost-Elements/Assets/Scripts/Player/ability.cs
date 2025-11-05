using UnityEngine;

public class SimpleSpellCombiner : MonoBehaviour
{
    private bool hasFire = false;
    private bool hasWater = false;

    void Update()
    {
        // Press 1 to collect Fire
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasFire = true;
            Debug.Log("Fire selected");
        }

        // Press 2 to collect Water
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasWater = true;
            Debug.Log("Water selected");
        }

        // Press 3 to combine
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasFire && hasWater)
            {
                Debug.Log("🔥💧 Combined into Steam Burst!");
                // Spawn your ability prefab here later
                hasFire = false;
                hasWater = false;
            }
            else
            {
                Debug.Log("You need both Fire and Water to craft!");
            }
        }
    }
}
