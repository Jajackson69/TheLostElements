using UnityEngine;

public class Fairy : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject chatBubblePrefab;
    [SerializeField] private Transform chatBubblePositionRight;
    [SerializeField] private Transform chatBubblePositionLeft;
    [SerializeField] private float bubbleLifetime = 3f;

    private GameObject currentBubble;
    private SpriteRenderer currentBubbleRenderer;

    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (player == null || spriteRenderer == null) return;

        spriteRenderer.flipX = player.position.x < transform.position.x;

        if (currentBubble != null)
        {
            Transform targetPos = spriteRenderer.flipX ? chatBubblePositionLeft : chatBubblePositionRight;
            currentBubble.transform.position = targetPos.position;

            if (currentBubbleRenderer != null)
                currentBubbleRenderer.flipX = spriteRenderer.flipX;
        }
    }

    public void SpawnBubble()
    {
        if (chatBubblePrefab == null) return;

        Transform targetPos = spriteRenderer.flipX ? chatBubblePositionLeft : chatBubblePositionRight;
        if (targetPos == null) return;

        if (currentBubble != null)
            Destroy(currentBubble);

        currentBubble = Instantiate(chatBubblePrefab, targetPos.position, Quaternion.identity, transform);
        currentBubbleRenderer = currentBubble.GetComponent<SpriteRenderer>();

        if (currentBubbleRenderer != null)
            currentBubbleRenderer.flipX = spriteRenderer.flipX;

        Destroy(currentBubble, bubbleLifetime);
    }
}
