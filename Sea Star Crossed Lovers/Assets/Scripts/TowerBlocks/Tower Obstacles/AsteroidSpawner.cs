using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    private LevelManager levelManager;
    private ObstacleManager obstacleManager;
    private BlockManager blockManager;

    public Asteroid asteroidPrefab;

    public void Construct(LevelManager manager, ObstacleManager obstacleManager)
    {
        levelManager = manager;
        this.obstacleManager = obstacleManager;
        blockManager = manager.blockManager;

        obstacleManager.OnAsteroidSpawn.AddListener(SpawnAsteroid);
    }

    private void SpawnAsteroid()
    {
        Vector2 spawnPosition = gameObject.transform.position;
        Asteroid asteriod = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
        asteriod.Construct(blockManager);
    }
}
