using System.Collections;
using UnityEngine;

/// <summary>
/// Enemy spawns at pointA and patrols between pointA and pointB
/// </summary>
public class EnemyPatrol : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    private Rigidbody2D rb;
    private Transform currentPoint;
    public float speed;
    private SpriteRenderer spriteRenderer;
    #region Bouncing Params
    bool isBouncing = false;
    float bounceForce = 30f;
    float bounceTime = 0.1f;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointA; // Start at point A
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBouncing) { return; }
        // Move towards the current point
        Vector2 direction = (currentPoint.position - transform.position).normalized; // Calculate direction
        rb.velocity = direction * speed; // Set velocity

        // Check if the enemy is close enough to switch points
        if (Vector2.Distance(transform.position, currentPoint.position) < .6f)
        {
            // Switch points
            if (currentPoint == pointA)
            {
                currentPoint = pointB;
                spriteRenderer.flipX = true; // Flip sprite when moving to point B
            }
            else
            {
                currentPoint = pointA;
                spriteRenderer.flipX = false; // Flip sprite when moving to point A
            }
        }
    }

    public void Bounce(Vector2 direction)
    {
        Debug.Log("Enemy Bounce");
        isBouncing = true;
        StartCoroutine(OnBounce(direction));
    }

    private IEnumerator OnBounce(Vector2 direction)
    {
        isBouncing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = direction * bounceForce;
        yield return new WaitForSeconds(bounceTime);

        rb.gravityScale = originalGravity;
        isBouncing = false;
    }
}

