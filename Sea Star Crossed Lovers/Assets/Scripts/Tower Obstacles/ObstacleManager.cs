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
    public float warningIndicatorInterval = 0.33f;
    public bool flashWarning;

    public float asteroidCooldown;
    public bool asteroidEnabled = true;

    public UnityEvent OnAsteroidSpawn;

    private int[] windDirections = new int[2] { -1, 1 };

    private LevelManager levelManager;
    private GameObject warningIndicator;
    [SerializeField] private AsteroidSpawner asteroidSpawner;

    public void Construct(LevelManager manager)
    {
        levelManager = manager;
        OnAsteroidSpawn = new UnityEvent();

        asteroidSpawner.Construct(levelManager, this);
    }

    private void Start()
    {
        flashWarning = false;
        warningIndicator = GameObject.Find("TowerObjects").transform.Find("Warning Indicator").gameObject;
        warningIndicator.SetActive(false);
        StartCoroutine(WindLoop(windCooldown));
        StartCoroutine(AsteroidLoop(asteroidCooldown));
    }

    private IEnumerator WindLoop(float cooldown)
    {
        while (windEnabled)
        {
            yield return new WaitForSeconds(cooldown - 1f);
            warningIndicator.transform.position = new Vector2(10, 18);
            StartCoroutine(Blink(warningIndicatorInterval));
            flashWarning = true;
            yield return new WaitForSeconds(1f);
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
        flashWarning = false;
    }

    private IEnumerator AsteroidLoop(float cooldown)
    {
        while (asteroidEnabled)
        {
            yield return new WaitForSeconds(cooldown - 3f);
            asteroidSpawner.ChooseSpawnLocation();
            warningIndicator.transform.position = new Vector2(-10, asteroidSpawner.transform.position.y);
            StartCoroutine(Blink(warningIndicatorInterval));
            flashWarning = true;
            yield return new WaitForSeconds(3f);
            OnAsteroidSpawn.Invoke();
            flashWarning = false;
        }
    }

    private IEnumerator Blink(float interval)
    {
            warningIndicator.SetActive(!warningIndicator.activeSelf);
            yield return new WaitForSeconds(interval);
            if(flashWarning)
                yield return StartCoroutine(Blink(warningIndicatorInterval));
            else
                warningIndicator.SetActive(false);
    }
}
