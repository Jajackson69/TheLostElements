using UnityEngine;
using TMPro;

public class TMPBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void InitTMP()
    {
        TMP_Settings settings = TMP_Settings.instance;
        if (settings != null)
            Debug.Log("✅ TMP initialized early");
    }
}
