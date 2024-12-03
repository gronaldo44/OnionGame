using UnityEngine;

public class AttackArea : MonoBehaviour
{
    public int dmg = 5;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<health>(out var health))
        {
            health.DamageTaken(dmg);
            GameManager.Instance.GetCurrentPlayer().GetComponent<PlayerController>().DamageBounce();
        }
    }
}
