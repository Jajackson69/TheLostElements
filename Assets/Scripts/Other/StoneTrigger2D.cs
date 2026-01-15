using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneTrigger2D : MonoBehaviour
{
    public GameObject replacementObject;
    public GameObject teleportEffect;
    public string sceneToLoad;
    public SpriteRenderer stoneSprite;
    bool triggered;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (other.CompareTag("Player"))
        {
            triggered = true;
            replacementObject.SetActive(true);
            teleportEffect.SetActive(true);
            stoneSprite.enabled = false;
            StartCoroutine(TeleportPlayer());
        }
    }

    System.Collections.IEnumerator TeleportPlayer()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneToLoad);
    }
}
