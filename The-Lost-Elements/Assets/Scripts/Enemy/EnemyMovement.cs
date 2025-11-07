using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class EnemyMovement : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float patrolSpeed = 2.0f;
    [SerializeField] float chaseSpeed  = 3.0f;

    [Header("Sight")]
    [SerializeField] float sightRange = 8f;            // how far
    [SerializeField] float verticalTolerance = 1.2f;   // how different in height we allow
    [SerializeField] LayerMask obstacleMask;           // walls, ground that block sight

    [Header("Ground and walls")]
    [SerializeField] LayerMask groundMask;             // your Ground layer
    [SerializeField] float wallCheckExtra = 0.1f;
    [SerializeField] float ledgeCheckDown = 0.25f;
    [SerializeField] float frontOffsetX = 0.05f;

    [Header("Refs")]
    [SerializeField] string playerTag = "Player";
    [SerializeField] Animator animator;

    enum State { Patrol, Chase }
    State state = State.Patrol;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;
    int dir = 1; // 1 right, −1 left
    float halfWidth;
    float timeSinceSeen = 99f;
    [SerializeField] float loseSightAfter = 0.6f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        if (!animator) animator = GetComponent<Animator>();
    }
    void Start()
    {
        var p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) player = p.transform;
        halfWidth = sr.bounds.extents.x;
    }

    void FixedUpdate()
    {
        if (!player) return;

        bool canSee = CanSeePlayerSideScroll();

        if (canSee) { state = State.Chase; timeSinceSeen = 0f; }
        else { timeSinceSeen += Time.fixedDeltaTime; if (timeSinceSeen > loseSightAfter) state = State.Patrol; }

        if (state == State.Patrol) PatrolTick();
        else                       ChaseTick();

        if (animator) animator.SetBool("isRunning", Mathf.Abs(rb.linearVelocity.x) > 0.05f);
        sr.flipX = dir < 0;
    }

    void PatrolTick()
    {
        if (HitWallAhead() || NoGroundAhead()) dir *= -1;
#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = new Vector2(patrolSpeed * dir, rb.linearVelocityY);
#else
        rb.velocity = new Vector2(patrolSpeed * dir, rb.velocity.y);
#endif
    }

    void ChaseTick()
    {
        dir = (player.position.x > transform.position.x) ? 1 : -1;

        // do not run off a ledge while chasing
        if (HitWallAhead() || NoGroundAhead())
        {
#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = new Vector2(0f, rb.linearVelocityY);
#else
            rb.velocity = new Vector2(0f, rb.velocity.y);
#endif
            return;
        }

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = new Vector2(chaseSpeed * dir, rb.linearVelocityY);
#else
        rb.velocity = new Vector2(chaseSpeed * dir, rb.velocity.y);
#endif
    }

    bool CanSeePlayerSideScroll()
    {
        float dx = Mathf.Abs(player.position.x - transform.position.x);
        if (dx > sightRange) return false;

        float dy = Mathf.Abs(player.position.y - transform.position.y);
        if (dy > verticalTolerance) return false; 

        Vector2 from = new Vector2(transform.position.x, transform.position.y + 0.5f);
        Vector2 to   = new Vector2(player.position.x,    player.position.y + 0.5f);
        Vector2 dirRay = (to - from).normalized;
        float dist = Vector2.Distance(from, to);

        var hit = Physics2D.Raycast(from, dirRay, dist, obstacleMask);
        return hit.collider == null;
    }

    bool HitWallAhead()
    {
        Vector2 origin = transform.position;
        Vector2 castDir = new Vector2(dir, 0f);
        float len = halfWidth + wallCheckExtra;
        return Physics2D.Raycast(origin, castDir, len, groundMask);
    }

    bool NoGroundAhead()
    {
        Vector2 front = (Vector2)transform.position + new Vector2(dir * (halfWidth + frontOffsetX), 0f);
        return !Physics2D.Raycast(front, Vector2.down, ledgeCheckDown, groundMask);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // debug rays
        Gizmos.color = Color.red;
        Vector3 castDir = new Vector3(dir * (halfWidth + wallCheckExtra), 0f, 0f);
        Gizmos.DrawLine(transform.position, transform.position + castDir);

        Gizmos.color = Color.green;
        Vector2 front = (Vector2)transform.position + new Vector2(dir * (halfWidth + frontOffsetX), 0f);
        Gizmos.DrawLine(front, front + Vector2.down * ledgeCheckDown);
    }
#endif
}
