using UnityEngine;

public class EnemyAttackArea : MonoBehaviour
{
    [SerializeField] public int dmg;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerDoDamage>() != null)
        {
            PlayerHealth h = collision.GetComponent<PlayerHealth>();
            h.TakeDamage(dmg);
        }
    }
}
