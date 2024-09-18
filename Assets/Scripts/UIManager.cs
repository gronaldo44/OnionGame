using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI Elements
    public Slider healthBar;

    private PlayerHealth playerHealth;

    void Start()
    {
        ConnectHealthBarToPlayer();
    }

    private void ConnectHealthBarToPlayer()
    {
        // Find the PlayerHealth script and subscribe to the OnHealthChanged event
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
        }
        else
        {
            Debug.LogWarning("PlayerHealth component not found.");
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        playerHealth = FindAnyObjectByType<PlayerHealth>();
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

    private void HandlePlayerDeath()
    {
        // TODO figure what should happen when the player dies

    }

    public void UpdatePlayerReference(GameObject newPlayer)
    {
        // Unsubscribe from the previous player if necessary
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
            playerHealth.OnPlayerDied.RemoveListener(HandlePlayerDeath);
        }

        // Assign the new player's health and reconnect the slider
        playerHealth = newPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
        }
        else
        {
            Debug.LogWarning("New player does not have a PlayerHealth component.");
        }
    }
}
