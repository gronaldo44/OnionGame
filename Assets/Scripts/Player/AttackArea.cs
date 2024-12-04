using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int dmg = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<health>(out var enemy))
        {
            Debug.Log("PlayerAttackArea collided with Enemy");

            EnemyPatrol enemyPatrol = enemy.GetComponent<EnemyPatrol>();
            Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
            PlayerController player = GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerController>();
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            if (enemyPatrol != null)
            {
                // bounce player away from enemy
                float xDirection = playerRb.position.x - enemyRb.position.x;
                if (xDirection < 0)
                {
                    player.Bounce(Vector2.left);
                    enemyPatrol.Bounce(Vector2.right);
                }
                else
                {
                    // bounce right
                    player.Bounce(Vector2.right);
                    enemyPatrol.Bounce(Vector2.left);
                }
            }

            // handle damage
            enemy.DamageTaken(dmg);
        }
    }
}
