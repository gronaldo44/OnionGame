using UnityEngine;

public class health : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    private float currentHealth;

    private bool isDead;

    private void Start()
    {
        currentHealth = startingHealth;
        isDead = false;
    }

    public void DamageTaken(float damage)
    {
        //Debug.Log("Took Damage: " + damage);
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);
        currentHealth -= damage;
        //Debug.Log("Health: " + currentHealth + "/" + startingHealth);

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
