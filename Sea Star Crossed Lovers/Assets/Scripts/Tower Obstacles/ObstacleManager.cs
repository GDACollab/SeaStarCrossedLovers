using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ObstacleManager : MonoBehaviour
{
    public Vector2 windVelocity;
    public float windCooldown;
    public float windDuration;
    public bool windEnabled = true;
    public bool windIsActive;

    public float asteroidCooldown;
    public bool asteroidEnabled = true;

    public UnityEvent OnAsteroidSpawn;

    private LevelManager levelManager;
    private AsteroidSpawner asteroidSpawner;

    public void Construct(LevelManager manager)
    {
        levelManager = manager;
        OnAsteroidSpawn = new UnityEvent();

        asteroidSpawner = GameObject.Find("AsteroidSpawnerObj").GetComponent<AsteroidSpawner>();
        asteroidSpawner.Construct(levelManager, this);
    }

    private void Start()
    {
        StartCoroutine(WindLoop(windCooldown, windDuration));
        StartCoroutine(AsteroidLoop(asteroidCooldown));
    }

    private IEnumerator WindLoop(float cooldown, float duration)
    {
        while (windEnabled)
        {
            yield return new WaitForSeconds(cooldown);
            yield return StartCoroutine(WindDuration(duration));
        }
    }

    private IEnumerator WindDuration(float duration)
    {
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
