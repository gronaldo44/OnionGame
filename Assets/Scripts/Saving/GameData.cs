using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public struct GameData
{
    public PlayerData player;
    public List<SwingableData> swingables;
    public List<EnemyData> enemies;

    /// <summary>
    /// Constructs save data for the game with player, swingables, and enemies.
    /// </summary>
    /// <param name="player">Player data to be saved</param>
    /// <param name="swingables">Swingable objects</param>
    /// <param name="enemies">Enemies</param>
    public GameData(PlayerData player, List<SwingableData> swingables, List<EnemyData> enemies)
    {
        this.player = player;
        this.swingables = swingables ?? new List<SwingableData>();
        this.enemies = enemies ?? new List<EnemyData>();
    }
}

[Serializable]
public struct SwingableData
{
    public Vector3 Location;
    public bool IsRopeSwing;

    public SwingableData(Vector3 loc, bool isRopeSwing)
    {
        Location = loc;
        IsRopeSwing = isRopeSwing;
    }
}

[Serializable]
public struct PlayerData
{
    public float maxHealth;
    public float playerHealth;
    public Vector3 currLocation;
    public Vector3 spawnLocation;

    public PlayerData(float maxHealth, float? hp, Vector3 loc, Vector3 spawn)
    {
        this.maxHealth = maxHealth;
        playerHealth = hp ?? maxHealth;
        currLocation = loc;
        spawnLocation = spawn;
    }
}

[Serializable]
public struct EnemyData
{
    public GameObject enemyPrefab;
    public Vector3 enemyPatrolPositionA;
    public Vector3 enemyPatrolPositionB;
    public bool isActive;

    /// <summary>
    /// Save data for an enmy that spawns on posA then patrols between posA and posB
    /// </summary>
    /// <param name="type">enemy prefab</param>
    /// <param name="patrolPosA">spawn location</param>
    /// <param name="patrolPosB">patrol location</param>
    /// <param name="isActive"></param>
    public EnemyData(GameObject type, Vector3 patrolPosA, Vector3 patrolPosB, bool isActive)
    {
        enemyPrefab = type;
        enemyPatrolPositionA = patrolPosA;
        enemyPatrolPositionB = patrolPosB;
        this.isActive = isActive;
    }
}

