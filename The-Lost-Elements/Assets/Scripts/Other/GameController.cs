using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    public static GameObject CurrentPlayer { get; private set; }

    public static Action<GameObject> OnPlayerSpawned;
    private void Awake()
    {
        player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        
    }

    private void Start()
    {
        OnPlayerSpawned?.Invoke(player);
    }
}
