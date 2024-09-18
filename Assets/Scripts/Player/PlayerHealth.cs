using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public float startingHealth = 50f;
    [SerializeField] private float currentHealth;
    private bool isDead;

    // Event Controllers for UIManager
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent OnPlayerDied;

    void Start()
    {
        currentHealth = startingHealth;
        isDead = false;

        // Initialize the health bar UI
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return; // No damage if the player is already dead

        currentHealth = Mathf.Clamp(currentHealth - damage, 0, startingHealth);

        // Notify listeners of health change
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log("Player took damage: " + damage);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth = Mathf.Clamp(currentHealth + healAmount, 0, startingHealth);

        // Notify listeners of health change
        OnHealthChanged?.Invoke(currentHealth);
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
