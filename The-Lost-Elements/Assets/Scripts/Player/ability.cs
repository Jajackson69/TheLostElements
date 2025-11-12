using UnityEngine;

public class SimpleSpellCombiner : MonoBehaviour
{
    private bool hasFire = false;
    private bool hasWater = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            hasFire = true;
            Debug.Log("Fire selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            hasWater = true;
            Debug.Log("Water selected");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (hasFire && hasWater)
            {
                Debug.Log("🔥💧 Combined into Steam Burst!");
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
