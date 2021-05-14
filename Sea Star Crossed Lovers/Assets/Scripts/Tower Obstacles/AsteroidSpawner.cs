using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid asteroidPrefab;
    public Vector2 asteroidVelocity;
    public Vector2 asteroidForce;
    [Tooltip("Lower distance asteroid can spawn; Based on y position of highest block minus this number")]
    public float minSpawnYOffset = 10;
    [Tooltip("Lowest posible distance asteroid can spawn")]
    public float lowestSpawn = 2;

    private float constXPos;

    private LevelManager levelManager;
    private ObstacleManager obstacleManager;
    private BlockManager blockManager;

    public void Construct(LevelManager manager, ObstacleManager obstacleManager)
    {
        levelManager = manager;
        this.obstacleManager = obstacleManager;
        blockManager = manager.blockManager;

        obstacleManager.OnAsteroidSpawn.AddListener(SpawnAsteroid);

        constXPos = gameObject.transform.position.x;
    }

    private void SpawnAsteroid()
    {
        ChooseSpawnLocation();
        Vector2 spawnPosition = gameObject.transform.position;
        Asteroid asteriod = Instantiate(asteroidPrefab, spawnPosition, Quaternion.identity);
        asteriod.Construct(blockManager, asteroidVelocity, asteroidForce);
    }

    private void ChooseSpawnLocation()
    {
        float maxSpawnY = blockManager.highestBlockHeight;
        float minSpawnY = blockManager.highestBlockHeight - minSpawnYOffset;
        float finalSpawnY = Mathf.Max(lowestSpawn, Random.Range(minSpawnY, maxSpawnY));

        gameObject.transform.position = new Vector2(constXPos, finalSpawnY);
    }
}
