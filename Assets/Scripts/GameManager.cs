using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    // player setup
    [SerializeField] private GameObject playerPrefab; 
    [SerializeField] private Transform spawnPoint;
    // Swingables setup
    [SerializeField] private GameObject swingablePrefab;
    [SerializeField] private Transform[] swingableSpawnPoints;
    // camera setup
    public CameraManager cameraManager;

    private GameObject currentPlayer;

    private void Awake()
    {
        // Ensure that there is only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this instance persistent across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    void Start()
    {
        SpawnPlayer();
        SpawnRopeSwings();
    }

    private void SpawnPlayer()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
        }

        if (cameraManager != null)
        {
            cameraManager.UpdateCameraFollow(currentPlayer.transform);
        }
    }

    private void SpawnRopeSwings()
    {
        foreach (Transform spawnPoint in swingableSpawnPoints)
        {
            GameObject swingable = Instantiate(swingablePrefab, spawnPoint.position, Quaternion.identity);
            // Set the reference to the player or any other setup needed
            SwingCollider swingCollider = swingable.GetComponent<SwingCollider>();
            if (swingCollider != null)
            {
                swingCollider.SetPlayerReference(currentPlayer); // Add a method to set the player reference in SwingCollider
            }
        }
    }

    // Just respawns the player at the beginning right now
    private void HandlePlayerDeath()
    {
        SpawnPlayer();
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}

