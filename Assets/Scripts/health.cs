using System.Collections;
using UnityEngine;

public class health : MonoBehaviour
{
    [SerializeField] private float startingHealth;

    private Animator animator;

    private float _currentHealth;

    private string damageTaken = "isDamaged";

    public float CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            if (value <= 0)
            {
                _currentHealth = 0;
                IsDead = true;
            }
            else
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
        CurrentHealth = startingHealth;
        animator = GetComponent<Animator>();
    }

    public void DamageTaken(float damage)
    {
        Debug.Log(gameObject.name + " Took Damage: " + damage);
        if (animator != null)
        {
            animator.SetTrigger(damageTaken);
        }
        CurrentHealth -= damage;
        Debug.Log("Health: " + CurrentHealth + "/" + startingHealth);
    }


    // TODO
    private void HandleDeath()
    {
        Debug.Log(gameObject.name + " is dead");
        gameObject.SetActive(false);
    }
}
