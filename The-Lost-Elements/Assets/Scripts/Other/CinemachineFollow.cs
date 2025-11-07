using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

[RequireComponent(typeof(CinemachineCamera))]
public class CinemachinePlayerFollow : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private CinemachineCamera vcam;

    void Awake()
    {
        vcam = GetComponent<CinemachineCamera>();
    }

    void OnEnable()
    {
        // try both: event and fallback search
        GameController.OnPlayerSpawned += AssignFollowTarget;
        TryAssignExisting();
        // and a tiny delayed retry in case spawn happens slightly later
        StartCoroutine(WaitAndAssign());
    }

    void OnDisable()
    {
        GameController.OnPlayerSpawned -= AssignFollowTarget;
    }

    private void AssignFollowTarget(GameObject player)
    {
        if (!player) return;
        vcam.Follow = player.transform;
        vcam.LookAt = player.transform; // optional
        // Debug.Log("Cinemachine now following: " + player.name);
    }

    private void TryAssignExisting()
    {
        // If player already exists in scene, grab it
        var existing = GameObject.FindWithTag(playerTag);
        if (existing) AssignFollowTarget(existing);
    }

    private IEnumerator WaitAndAssign()
    {
        // safety net, waits a few frames until player appears
        float t = 0f;
        while (vcam.Follow == null && t < 2f)
        {
            var p = GameObject.FindWithTag(playerTag);
            if (p)
            {
                AssignFollowTarget(p);
                yield break;
            }
            t += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
