using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int dmg = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<health>() != null)
        {
            health h = collision.GetComponent<health>();
            h.DamageTaken(dmg);
        }
    }
}
