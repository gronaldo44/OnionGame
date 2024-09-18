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
    [SerializeField] private CameraManager cameraManager;
    // UI setup
    [SerializeField] private UIManager uiManager;

    private GameObject currentPlayer;
    private List<GameObject> swingableInstances = new List<GameObject>();

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
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, Quaternion.identity);
        UpdateRopeSwingPlayerRefs();
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
        }

        if (cameraManager != null)
        {
            cameraManager.UpdateCameraFollow(currentPlayer.transform);
        }

        // Update the UIManager with the new player instance
        if (uiManager != null)
        {
            uiManager.UpdatePlayerReference(currentPlayer);
        }
    }

    private void SpawnRopeSwings()
    {
        foreach (Transform spawnPoint in swingableSpawnPoints)
        {
            GameObject swingable = Instantiate(swingablePrefab, spawnPoint.position, Quaternion.identity);
            swingableInstances.Add(swingable); // Keep track of swingables for future reference
            SwingCollider swingCollider = swingable.GetComponent<SwingCollider>();
            if (swingCollider != null)
            {
                swingCollider.SetPlayerReference(currentPlayer);
            }
        }
    }

    private void UpdateRopeSwingPlayerRefs()
    {
        foreach (GameObject swingable in swingableInstances)
        {
            SwingCollider swingCollider = swingable.GetComponent<SwingCollider>();
            if (swingCollider != null)
            {
                swingCollider.SetPlayerReference(currentPlayer);
            }
        }
    }

    // Just respawns the player at the beginning right now
    private void HandlePlayerDeath()
    {
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }

        SpawnPlayer();
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}

