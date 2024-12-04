using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    private int dmg = 1;
    private float cooldown = 0.05f;
    private float lastTriggerTime = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Time.time - lastTriggerTime > cooldown)
        {
            //handle kb
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 kbDirection;

                //knock left
                if (collision.transform.position.x > transform.position.x)
                {
                    kbDirection = new Vector2(75f, 10f);
                }
                else //knock right
                {
                    kbDirection = new Vector2(-75f, 10f);
                }

                playerRb.AddForce(kbDirection, ForceMode2D.Impulse);
            }

            lastTriggerTime = Time.time;

            //handle damage
            if (collision.GetComponent<PlayerDoDamage>() != null)
            {
                PlayerHealth h = collision.GetComponent<PlayerHealth>();
                h.TakeDamage(dmg);
            }
        }
    }
}
