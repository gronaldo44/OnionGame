using UnityEngine;

public class enemyAttackArea : MonoBehaviour
{
    [SerializeField] private int dmg;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<playerDoDamage>() != null)
        {
            PlayerHealth h = collision.GetComponent<PlayerHealth>();
            h.TakeDamage(dmg);
        }
    }
}
