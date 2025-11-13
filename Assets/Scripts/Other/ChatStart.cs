using UnityEngine;
using System.Collections;

public class ChatBootstrap : MonoBehaviour
{
    [SerializeField] private GameObject chatObject;

    IEnumerator Start()
    {
        yield return null;
        if (chatObject != null)
            chatObject.SetActive(true);
    }
}
