using UnityEngine;
using UnityEngine.UI;

public class UIArrowPulse : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 1.5f;   // how fast it fades
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float maxAlpha = 1f;

    private Image[] arrowImages;

    private void Awake()
    {
        arrowImages = GetComponentsInChildren<Image>();
    }

    private void Update()
    {
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        foreach (var img in arrowImages)
        {
            if (img != null)
            {
                Color c = img.color;
                c.a = alpha;
                img.color = c;
            }
        }
    }
}
