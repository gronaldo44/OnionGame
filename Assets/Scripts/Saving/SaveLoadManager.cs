using System.IO;
using UnityEngine;

public class SaveLoadManager
{
    private string saveFilePath;

    public SaveLoadManager()
    {
        // Set the path for the save file
        saveFilePath = Path.Combine(Application.persistentDataPath, "savefile.json");
    }

    public void SaveGame(string sceneName, GameData data)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{sceneName}Save.json");
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(filePath, json);
    }

    public GameData LoadGame(string sceneName)
    {
        string filePath = Path.Combine(Application.persistentDataPath, $"{sceneName}Save.json");

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(json);
        }

        return new GameData(); // Return default data if file doesn't exist
    }

    public bool HasSavedGame(string sceneName)
    {
        // Check if there is a saved file for the current scene
        return PlayerPrefs.HasKey(sceneName);
    }

}