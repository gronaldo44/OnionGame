using UnityEngine;

public class health : MonoBehaviour
{
    public float startingHealth;

    private float currentHealth;

    private bool isDead;

    private void Start()
    {
        currentHealth = startingHealth;
        isDead = false;
    }

    public void DamageTaken(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        currentHealth -= damage;

        if (currentHealth > 0)
        {
            // player hurt animation
        }
        else
        {

            // anim.SetTriger("death);
            isDead = true;
            Destroy(gameObject);
            //GetComponent<PlayerMovement>().enabled = false;

        }
    }
}
