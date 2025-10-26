using UnityEngine;
using System;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;

    public static GameObject CurrentPlayer { get; private set; }
    public static event Action<GameObject> OnPlayerSpawned;

    private void Awake()
    {
        // Instancie le joueur
        player = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        CurrentPlayer = player;
    }

    private void Start()
    {
        // Appelle l’événement pour avertir les autres systèmes
        OnPlayerSpawned?.Invoke(player);
    }
}
