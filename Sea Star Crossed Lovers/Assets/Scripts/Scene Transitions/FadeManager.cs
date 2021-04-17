using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The script used to fade in and out of scenes.
/// </summary>
public class FadeManager : MonoBehaviour
{
    /// <summary>
    /// The UI we want to use to set the fade.
    /// </summary>
    [Tooltip("The UI we want to use to set the fade.")]
    public GameObject ActiveUI;
    
    /// <summary>
    /// The prefab we're using to fade (See: fadeImage in assets).
    /// </summary>
    [Tooltip("The prefab we're using to fade (See: fadeImage in assets).")]
    public GameObject fadeImagePrefab;
    
    /// <summary>
    /// The actual image we're fading with.
    /// </summary>
    GameObject fadeImage;

    /// <summary>
    /// Should we fade in as soon as the scene starts?
    /// </summary>
    [Tooltip("Should we fade in as soon as the scene starts?")]
    public bool fadeInOnAwake = true;
    
    /// <summary>
    /// Used to keep track during Fade() if we need to fade in or out.
    /// </summary>
    bool fadeIn = true;

    /// <summary>
    /// The speed at which we should Fade().
    /// </summary>
    float fadeSpeed;

    /// <summary>
    /// A list of actions to be done when a fade is complete. All of these actions will be removed once Fade() is complete (so that one event doesn't happen both on a fade in or fade out).
    /// You can set this ahead of a time to start something as soon as a fade in is done.
    /// </summary>
    [Tooltip("A list of actions to be done when a fade is complete. All of these actions will be removed once Fade() is complete (so that one event doesn't happen both on a fade in or fade out). You can set this ahead of a time to start something as soon as a fade in is done.")]
    public UnityEngine.Events.UnityEvent onFadeComplete;

    /// <summary>
    /// The current opacity of the Fade().
    /// </summary>
    float fadeOpacity;

    // Start is called before the first frame update
    private void Awake()
    {
        fadeImage = Instantiate(fadeImagePrefab, ActiveUI.transform);
        if (fadeInOnAwake == false)
        {
            fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            fadeOpacity = 0;
            //So we don't mess with existing buttons.
            fadeImage.SetActive(false);
        }
        else {
            fadeImage.SetActive(true);
            fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, 1);
            fadeOpacity = 1;
            StartFade(1, true);
        }
    }

    /// <summary>
    /// Starts the Fade(). Meant to be used by classes that can't start the fade coroutine.
    /// </summary>
    /// <param name="speed">The speed you want the fade to fade at.</param>
    /// <param name="shouldFadeIn">Are we fading in? true for fade in, false for fade out.</param>
    public void StartFade(float speed, bool shouldFadeIn) {
        fadeImage.SetActive(true);
        fadeIn = shouldFadeIn;
        fadeSpeed = speed;
        StartCoroutine("Fade");
    }

    IEnumerator Fade() {
        if (!fadeIn)
        {
            while (fadeImage.GetComponent<Image>().color.a < 1)
            {
                fadeOpacity += fadeSpeed * Time.deltaTime;
                fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, fadeOpacity);
                yield return new WaitForEndOfFrame();
            }
            onFadeComplete.Invoke();
            //Just so nothing gets called again... just in case.
            onFadeComplete.RemoveAllListeners();
        }
        else {
            while (fadeImage.GetComponent<Image>().color.a > 0)
            {
                fadeOpacity -= fadeSpeed * Time.deltaTime;
                fadeImage.GetComponent<Image>().color = new Color(0, 0, 0, fadeOpacity);
                yield return new WaitForEndOfFrame();
            }
            fadeImage.SetActive(false);
            onFadeComplete.Invoke();
            //Just so the callbacks don't get called again if !fadeIn.
            onFadeComplete.RemoveAllListeners();
        }
    }
}
