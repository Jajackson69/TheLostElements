using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private Vector2 startPos;
    private float lengthX;

    public Transform cam;
    [Range(0f, 1f)] public float parallaxEffect = 0.5f;
    public float yOffset = 0f;

    void Start()
    {
        startPos = transform.position;
        var size = GetComponent<SpriteRenderer>().bounds.size;
        lengthX = size.x;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        float distX = cam.position.x * parallaxEffect;
        float moveX = cam.position.x * (1 - parallaxEffect);

        float yPos = cam.position.y + yOffset;

        transform.position = new Vector3(
            startPos.x + distX,
            yPos,
            transform.position.z
        );

        if (moveX > startPos.x + lengthX)
        {
            startPos.x += lengthX;
        }
        else if (moveX < startPos.x - lengthX)
        {
            startPos.x -= lengthX;
        }
    }
}
