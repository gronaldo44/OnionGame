using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    // UI Elements
    private Image onionHealth_1;
    private Image onionHealth_2;
    private Image onionHealth_3;

    private PlayerHealth playerHealth;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        onionHealth_3.gameObject.SetActive(true);
        onionHealth_2.gameObject.SetActive(false);
        onionHealth_1.gameObject.SetActive(false);
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

    public void setOnions()
    {
        onionHealth_1 = GameObject.Find("OnionHealth_1").GetComponent<Image>();
        onionHealth_2 = GameObject.Find("OnionHealth_2").GetComponent<Image>();
        onionHealth_3 = GameObject.Find("OnionHealth_3").GetComponent<Image>();
        if (onionHealth_1 != null && onionHealth_2 != null && onionHealth_3 != null)
        {
            onionHealth_3.gameObject.SetActive(true);
            onionHealth_2.gameObject.SetActive(false);
            onionHealth_1.gameObject.SetActive(false);

        }
        else
        {
            Debug.LogWarning("OnionHealthImage not assigned");
        }
    }

    private void UpdateHealthBar(float newHealth)
    {
        if (onionHealth_1 == null || onionHealth_2 == null || onionHealth_3 == null)
        {
            Debug.LogError("OnionHealthImage not assigned in prefab");
            return;
        }

        if (newHealth == 3)
        {
            onionHealth_3.gameObject.SetActive(true);
            onionHealth_2.gameObject.SetActive(false);
            onionHealth_1.gameObject.SetActive(false);
        } else if (newHealth == 2)
        {
            onionHealth_3.gameObject.SetActive(false);
            onionHealth_2.gameObject.SetActive(true);
            onionHealth_1.gameObject.SetActive(false);
        } else
        {
            onionHealth_3.gameObject.SetActive(false);
            onionHealth_2.gameObject.SetActive(false);
            onionHealth_1.gameObject.SetActive(true);
        }

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

        // Assign the new player's health and connect the slider
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
