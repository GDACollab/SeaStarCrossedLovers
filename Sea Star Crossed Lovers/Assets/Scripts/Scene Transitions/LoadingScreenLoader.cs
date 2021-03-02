using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// The object in the loading screen that accesses LoadingData to load the next scene.
/// </summary>
public class LoadingScreenLoader : MonoBehaviour
{
    AsyncOperation loadNextScene;

    /// <summary>
    /// The image to fill up with the level's loading progress.
    /// </summary>
    [Tooltip("The image to fill up with the level's loading progress.")]
    public Image progressBar;

    // Start is called before the first frame update
    void Awake()
    {
        loadNextScene = SceneManager.LoadSceneAsync(LoadingData.sceneToLoad);
    }

    private void Update()
    {
        progressBar.fillAmount = loadNextScene.progress;
    }
}
