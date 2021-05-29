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
    private GameObject meteorWarning;
    private GameObject windWarning;
    public GameObject Canvas;
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
        meteorWarning = Canvas.transform.Find("Meteor Warning").gameObject;
        windWarning = Canvas.transform.Find("Wind Warning").gameObject;
        windWarning.SetActive(false);
        meteorWarning.SetActive(false);
        StartCoroutine(WindLoop(windCooldown));
        StartCoroutine(AsteroidLoop(asteroidCooldown));
    }

    private IEnumerator WindLoop(float cooldown)
    {
        while (windEnabled)
        {
            yield return new WaitForSeconds(cooldown - 1f);
            windWarning.transform.localPosition = new Vector2(300f, 400f);
            StartCoroutine(Blink(warningIndicatorInterval, windWarning));
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
            //need to map from world coords to local coords relative to the canvas
            float canvasY = (asteroidSpawner.transform.position.y * 45f) - 390f;
            meteorWarning.transform.localPosition = new Vector2(-300f, canvasY);
            StartCoroutine(Blink(warningIndicatorInterval, meteorWarning));
            flashWarning = true;
            yield return new WaitForSeconds(3f);
            OnAsteroidSpawn.Invoke();
            flashWarning = false;
        }
    }

    private IEnumerator Blink(float interval, GameObject warningIndicator)
    {
            warningIndicator.SetActive(!warningIndicator.activeSelf);
            yield return new WaitForSeconds(interval);
            if(flashWarning)
                yield return StartCoroutine(Blink(warningIndicatorInterval, warningIndicator));
            else
                warningIndicator.SetActive(false);
    }
}
