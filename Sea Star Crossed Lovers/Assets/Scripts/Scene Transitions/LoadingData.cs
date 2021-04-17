/// <summary>
/// This is the class that transfers data in between scenes. Right now, it just is for SceneLoader to pass over the next scene to LoadingScreenLoader. Feel free to add more stuff.
/// In whatever the next scene is, you should be able just to access LoadingData.whateverDataYouNeed to get the data that was passed from the previous scene.
/// </summary>
public static class LoadingData
{
    /// <summary>
    /// The scene to load in the loading scene.
    /// </summary>
    public static string sceneToLoad;
}
