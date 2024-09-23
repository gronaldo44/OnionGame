using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // UI Elements
    [SerializeField] private Slider healthBar;

    private PlayerHealth playerHealth;

    void Start()
    {
        // Set the healthBar dynamically at runtime, if not already set
        if (healthBar == null)
        {
            healthBar = GameObject.Find(SceneStrings.mainCamera).GetComponent<Slider>();
            if (healthBar == null)
            {
                Debug.LogError("HealthBar slider not found in the scene!");
                return; // Prevent further execution if healthBar is missing
            }
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid memory leaks
        playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.RemoveListener(UpdateHealthBar);
        } else
        {
            Debug.LogError("Could not find playerhealth");
        }
    }

    void Update()
    {
    }

    private void UpdateHealthBar(float newHealth)
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not assigned in the UIManager.");
            return;
        }

        healthBar.value = newHealth;
        Debug.Log("Health bar updated: " + newHealth);
    }

    private void HandlePlayerDeath()
    {
        // TODO figure what should happen when the player dies

    }

    public void UpdatePlayerReference(GameObject newPlayer)
    {
        Debug.Log("Update UI Player Reference: " + newPlayer.gameObject.name);
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
            UpdateHealthBar(playerHealth.CurrentHealth);
        }
        else
        {
            Debug.LogError("PlayerHealth component not found");
        }
    }
}
