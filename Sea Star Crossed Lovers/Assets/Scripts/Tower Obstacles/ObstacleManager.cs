using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleManager : MonoBehaviour
{
    public float windVelocity;
    public float windCooldown;
    public float minWindDuration;
    public float maxWindDuration;
    public bool windEnabled = true;
    public bool windIsActive;
    public int windDirection;

    public float asteroidCooldown;
    public bool asteroidEnabled = true;

    public UnityEvent OnAsteroidSpawn;

    private int[] windDirections = new int[2] { -1, 1 };

    private LevelManager levelManager;
    [SerializeField] private AsteroidSpawner asteroidSpawner;

    public void Construct(LevelManager manager)
    {
        levelManager = manager;
        OnAsteroidSpawn = new UnityEvent();

        asteroidSpawner.Construct(levelManager, this);
    }

    private void Start()
    {
        StartCoroutine(WindLoop(windCooldown));
        StartCoroutine(AsteroidLoop(asteroidCooldown));
    }

    private IEnumerator WindLoop(float cooldown)
    {
        while (windEnabled)
        {
            yield return new WaitForSeconds(cooldown);
            float randomDuration = Random.Range(minWindDuration, maxWindDuration);
            yield return StartCoroutine(WindDuration(randomDuration));
        }
    }

    private IEnumerator WindDuration(float duration)
    {
        int index = Random.Range(0, windDirections.Length);
        windDirection = windDirections[index];
        windIsActive = true;
        yield return new WaitForSeconds(duration);
        windIsActive = false;
    }

    private IEnumerator AsteroidLoop(float cooldown)
    {
        while (asteroidEnabled)
        {
            yield return new WaitForSeconds(cooldown);
            OnAsteroidSpawn.Invoke();
        }
    }
}
