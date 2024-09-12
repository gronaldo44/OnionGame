using UnityEngine;

public class enemyAttackArea : MonoBehaviour
{
    public int dmg = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<playerDoDamage>() != null)
        {
            health h = collision.GetComponent<health>();
            h.DamageTaken(dmg);
        }
    }
}
