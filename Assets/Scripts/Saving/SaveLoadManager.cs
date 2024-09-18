using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveLoadManager
{
    public void SaveGame(GameData data)
    {
        PlayerPrefs.SetFloat(PlayerPrefStrings.PlayerHealth, data.playerHealth);
        PlayerPrefs.SetFloat(PlayerPrefStrings.PlayerPosX, data.playerPosX);
        PlayerPrefs.SetFloat(PlayerPrefStrings.PlayerPosY, data.playerPosY);
        PlayerPrefs.SetFloat(PlayerPrefStrings.PlayerPosZ, data.playerPosZ);
        PlayerPrefs.Save();
    }

    public GameData LoadGame()
    {
        GameData data = new GameData
        {
            playerHealth = PlayerPrefs.GetFloat(PlayerPrefStrings.PlayerHealth, GameData.startingHealth),
            playerPosX = PlayerPrefs.GetFloat(PlayerPrefStrings.PlayerPosX, 0f),
            playerPosY = PlayerPrefs.GetFloat(PlayerPrefStrings.PlayerPosY, 0f),
            playerPosZ = PlayerPrefs.GetFloat(PlayerPrefStrings.PlayerPosZ, 0f)
        };
        return data;
    }
}
