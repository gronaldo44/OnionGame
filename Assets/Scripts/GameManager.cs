using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton instance

    // Saving/Loading
    private SaveLoadManager saveLoadManager;

    // player setup
    [SerializeField] private GameObject playerPrefab;
    // Swingables setup
    [SerializeField] private GameObject swingablePrefab;
    // Enemy setup
    [SerializeField] private GameObject mousePrefab;
    // camera setup
    [SerializeField] private CameraManager cameraManager;
    // UI setup
    [SerializeField] private UIManager uiManager;
    // DialogueSetup
    [SerializeField] private DialogueManager dialogueManager;

    private GameObject currentPlayer;
    private List<GameObject> swingableInstances = new List<GameObject>();
    private List<GameObject> enemyInstances = new List<GameObject>();

    private void Awake()
    {
        // Ensure that there is only one instance of GameManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return; // Exit if we destroy this instance
        }
        saveLoadManager = new SaveLoadManager();
    }

    void Start()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if there is saved game data for the current scene
        if (saveLoadManager.HasSavedGame(currentSceneName))
        {
            // Load the game data for the current scene
            LoadGame(currentSceneName);
        }
        else
        {
            // Spawn player and set up the environment with default conditions
            SpawnPlayer(new PlayerData());
            SpawnSwingables();
            SpawnEnemies();
        }
    }

    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changing Scene: " + sceneName);
        SaveGame(sceneName); // Save the current scene's data
        StartCoroutine(LoadSceneAndGame(sceneName)); // Start loading new scene
    }

    private IEnumerator LoadSceneAndGame(string sceneName)
    {
        // Load the new scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the new scene is fully loaded
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Once the scene is fully loaded, load the game data
        LoadGame(sceneName);
    }

    #region Saving
    public void SaveGame(string sceneName)
    {
        if (currentPlayer == null)
        {
            // We are in main menu
            saveLoadManager.SaveGame(sceneName, new GameData());
            return;
        }

        // Capture player health and position
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        Vector3 playerPosition = currentPlayer.transform.position;
        Vector3 playerSpawn = currentPlayer.GetComponent<PlayerController>().spawnLocation;

        // Create PlayerData from player stats
        PlayerData playerData = new PlayerData(playerHealth.maxHealth, playerHealth.CurrentHealth, playerPosition, playerSpawn);
        Destroy(currentPlayer);

        // Capture swingables positions
        List<SwingableData> swingableDataList = new List<SwingableData>();
        foreach (GameObject swingable in swingableInstances)
        {
            SwingController swingController = swingable.GetComponent<SwingController>();
            swingableDataList.Add(new SwingableData(swingable.transform.position, swingController.IsRopeSwing));
        }

        // Assuming you have enemy instances stored and managed similarly to swingables
        List<EnemyData> enemyDataList = new List<EnemyData>();
        foreach (GameObject enemy in enemyInstances)
        {
            EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
            enemyDataList.Add(new EnemyData(enemy, true));
        }

        // Create the GameData object with player, swingables, and enemies
        GameData gameData = new GameData(playerData, swingableDataList, enemyDataList);

        // Save game data using SaveLoadManager
        saveLoadManager.SaveGame(sceneName, gameData);
    }

    public void LoadGame(string sceneName)
    {
        Debug.Log("Loading Game: " + sceneName);
        GameData gameData = saveLoadManager.LoadGame(sceneName);

        // Clear previous references
        currentPlayer = null;
        swingableInstances.Clear();
        enemyInstances.Clear();

        if (GameObject.Find(SceneStrings.playerSpawn) == null)
        {
            return; // Handle no player spawn found
        }

        // Spawn player with the loaded game data
        SpawnPlayer(gameData.player);
        LoadSwingables(gameData.swingables);
        LoadEnemies(gameData.enemies);
    }

    private void LoadSwingables(List<SwingableData> swingables)
    {
        swingableInstances.Clear();
        foreach (SwingableData swing in swingables)
        {
            GameObject swingable = Instantiate(swingablePrefab, swing.Location, Quaternion.identity);
            swingableInstances.Add(swingable);
            SwingController swingController = swingable.GetComponent<SwingController>();
            if (swingController != null)
            {
                swingController.IsRopeSwing = swing.IsRopeSwing; // Set correctly based on loaded data
                swingController.SetPlayerReference(currentPlayer);
            }
        }

        SpawnSwingables();
    }

    private void LoadEnemies(List<EnemyData> enemies)
    {
        enemyInstances.Clear();
        foreach (EnemyData enemyData in enemies)
        {
            GameObject enemy = Instantiate(enemyData.enemyPrefab);
            if (!enemyData.isActive)
            {
                health hp = enemy.GetComponent<health>();
                hp.IsDead = true;
            }
            EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();
            enemyInstances.Add(enemy);
        }

        SpawnEnemies();
    }

    #endregion

    #region Spawning
    private void SpawnPlayer(PlayerData playerData)
    {
        // Check if player data has valid health and location
        bool hasPlayerData = playerData.playerHealth > 0;  // just checks if player is alive

        if (hasPlayerData)
        {
            Debug.Log("Spawning player from save");
            // Spawn the player at the saved location
            currentPlayer = Instantiate(playerPrefab, playerData.currLocation, Quaternion.identity);

            PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.CurrentHealth = playerData.playerHealth; // Resftore player health
                playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
            }
        }
        else
        {
            // Default spawn point for the first load
            GameObject defaultSpawnPoint = GameObject.Find(SceneStrings.playerSpawn);
            if (defaultSpawnPoint != null)
            {
                Debug.Log("Spawning player in default spawn");
                currentPlayer = Instantiate(playerPrefab, defaultSpawnPoint.transform.position, Quaternion.identity);

                PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.CurrentHealth = playerHealth.maxHealth; // Set to max health on first load
                    playerHealth.OnPlayerDied.AddListener(HandlePlayerDeath);
                }
            } else
            {
                Debug.Log("No Player found in scene: " + SceneManager.GetActiveScene().name);
                return;
            }
        }

        UpdateRopeSwingPlayerRefs();

        // Ensure CameraManager is set at runtime
        if (cameraManager == null)
        {
            cameraManager = GameObject.Find(SceneStrings.mainCamera).GetComponent<CameraManager>();
            if (cameraManager == null)
            {
                Debug.LogError("CameraManager not found in the scene!");
            }
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


    private void SpawnSwingables()
    {
        // Find the parent GameObject containing all swingable prefabs
        GameObject swingables = GameObject.Find(SceneStrings.swingables);
        if (swingables == null)
        {
            Debug.Log("No swingables found in scene: " + SceneManager.GetActiveScene().name);
            return;
        }

        // Iterate through each child of the swingables GameObject
        Debug.Log("Spawning Swingables");
        foreach (Transform swing in swingables.transform)
        {
            // Instantiate the swingable prefab at the position of the child
            swing.gameObject.SetActive(true);
            swingableInstances.Add(swing.gameObject); // Keep track of swingables for future reference

            // Set the player reference in the swingable
            if (swing.gameObject.TryGetComponent<SwingController>(out SwingController newSwingController))
            {
                newSwingController.SetPlayerReference(currentPlayer);
            }
        }
    }

    // Only spawns Mice right now
    private void SpawnEnemies()
    {
        // Find where to grab mice enemies from
        bool isFirstLoad = enemyInstances.Count == 0;

        if (isFirstLoad)
        {
            GameObject miceEnemies = GameObject.Find(SceneStrings.miceEnemies);
            if (miceEnemies == null)
            {
                Debug.Log("No mouse enemies found in scene: " + SceneManager.GetActiveScene().name);
                return;
            }

            Debug.Log("Spawning mice enemies");
            foreach (Transform enemy in miceEnemies.transform)
            {
                if (isFirstLoad)
                {
                    // Activate all enemies if this is the first load
                    enemy.gameObject.SetActive(true);
                    enemyInstances.Add(enemy.gameObject);
                }
            }
        }
        else
        {
            // Set active state based on loaded data for previously loaded enemies
            foreach (GameObject enemy in enemyInstances)
            {
                health enemyHealth = enemy.GetComponent<health>();
                if (enemyHealth != null)
                {
                    // Activate enemy if it is not dead
                    enemy.SetActive(!enemyHealth.IsDead);
                }
            }
        }
    }
    #endregion

    private void UpdateRopeSwingPlayerRefs()
    {
        foreach (GameObject swingable in swingableInstances)
        {
            SwingController swingCollider = swingable.GetComponent<SwingController>();
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

        SpawnPlayer(new PlayerData());
    }

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}

