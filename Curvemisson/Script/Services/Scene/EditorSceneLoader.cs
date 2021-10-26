using UnityEngine;

public class EditorSceneLoader : MonoBehaviour
{
    public void LoadBeforeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(Services.User.User.Instance.BeforeSceneName);
    }

    public void LoadBeforeSceneWithLoading()
    {
        Services.Scene.Loading.Main.LoadScene(Services.User.User.Instance.BeforeSceneName);
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneWithLoading(string sceneName)
    {
        Services.Scene.Loading.Main.LoadScene(sceneName);
    }
}