using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// The class you use to load the next scene. Attach this to some object and use like a button or something to call the Load function.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    public FadeManager fadeManager;

    /// <summary>
    /// Quick hack for buttons that can't do floats.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void QuickFadeOutLoad(string sceneName) {
        FadeOutLoad(sceneName, 1.0f);
    }

    public void FadeOutLoad(string sceneName, float speed)
    {
        LoadingData.sceneToLoad = sceneName;
        fadeManager.onFadeComplete.AddListener(StartLoadScene);
        fadeManager.StartFade(speed, false);
    }

    /// <summary>
    /// Meant to be used as a callback when the fade function is finished.
    /// </summary>
    void StartLoadScene() {
        SceneManager.LoadScene("Loading");
    }

    public void Load(string sceneName) {
        LoadingData.sceneToLoad = sceneName;
        StartLoadScene(); //Just using this because it's easier than typing out the whole thing.
    }
}
