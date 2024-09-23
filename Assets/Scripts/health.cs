using UnityEngine;

public class health : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    private float _currentHealth;
    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value <= 0)
            {
                _currentHealth = 0;
                IsDead = true;
            } else
            {
                // TODO HurtAnimation()
                _currentHealth = value;
            }
        }
    }
    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            if (value && !_isDead)
            {
                _isDead = true;
                HandleDeath();
            }
            else
            {
                _isDead = value;
            }
        }
    }

    private void Start()
    {
        _currentHealth = startingHealth;
    }

    public void DamageTaken(float damage)
    {
        //Debug.Log("Took Damage: " + damage);
        _currentHealth -= damage;
        //Debug.Log("Health: " + currentHealth + "/" + startingHealth);
    }

    // TODO
    private void HandleDeath()
    {
        Debug.Log(gameObject.name + " is dead");
        gameObject.SetActive(false);
    }
}
