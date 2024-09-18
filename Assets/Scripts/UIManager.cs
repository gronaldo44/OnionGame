using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI Elements
    public Slider healthBar;

    void Start()
    {
        // Find the PlayerHealth script and subscribe to the OnHealthChanged event
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            playerHealth.OnPlayerDied.AddListener(ShowGameOverScreen);
        } 
        else
        {
            Debug.LogWarning("PlayerHealth component not found.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        PlayerHealth playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        }
    }

    void Update()
    {
    }

    private void UpdateHealthBar(float newHealth)
    {
        healthBar.value = newHealth;
        Debug.Log("Health bar updated: " + newHealth);
    }

    private void ShowGameOverScreen()
    {
        // TODO
    }

}
