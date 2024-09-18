using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth = 50f;
    [SerializeField] private float _currHealth;
    public float CurrentHealth
    {
        get { return _currHealth; }
        set { _currHealth = value; }
    }
    private bool isDead;

    // Event Controllers for UIManager
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnPlayerDied;

    void Start()
    {
        _currHealth = startingHealth;
        isDead = false;

        // Initialize the health bar UI
        OnHealthChanged?.Invoke(_currHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // No damage if the player is already dead

        _currHealth = Mathf.Clamp(_currHealth - damage, 0, startingHealth);

        // Notify listeners of health change
        OnHealthChanged?.Invoke(_currHealth);
        Debug.Log("Player took damage: " + damage);

        if (_currHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        _currHealth = Mathf.Clamp(_currHealth + healAmount, 0, startingHealth);

        // Notify listeners of health change
        OnHealthChanged?.Invoke(_currHealth);
        Debug.Log("Player healed hp: " + healAmount);
    }

    private void Die()
    {
        isDead = true;

        // Notify listeners of player death
        Debug.Log("Player has died!");
        OnPlayerDied?.Invoke();
        // Add any additional death logic here
    }

    void Update()
    {
        // For testing
        if (Input.GetKeyDown(KeyCode.K)) // Press K to simulate damage
        {
            TakeDamage(10f); // Deal 10 damage for testing
        }
        if (Input.GetKeyDown(KeyCode.L)) // Press L to simulate healing
        {
            Heal(10f); // Heal 10 damage for testing
        }
    }
}
