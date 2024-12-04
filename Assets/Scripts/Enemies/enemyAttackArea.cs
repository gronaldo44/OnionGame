using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    [SerializeField] public int dmg = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerController>(out var player))
        {
            Debug.Log("EnemyAttackArea collided with Player");

            Rigidbody2D enemyRb = transform.parent.gameObject.GetComponent<Rigidbody2D>();
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            // bounce player away from enemy
            float xDirection = playerRb.position.x - enemyRb.position.x;
            if (xDirection < 0)
            {
                player.Bounce(Vector2.left);
            }
            else
            {
                // bounce right
                player.Bounce(Vector2.right);
            }

            //handle damage
            if (collision.GetComponent<PlayerDoDamage>() != null)
            {
                PlayerHealth h = collision.GetComponent<PlayerHealth>();
                h.TakeDamage(dmg);
            }
        }
    }
}
