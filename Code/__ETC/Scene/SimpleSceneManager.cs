using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSceneManager : MonoBehaviour
{
    public string sceneName;
    public bool isLoadingScene;

    public void OnMouseUpAsButton()
    {
        LoadScene(this.sceneName);
    }

    public void LoadScene(string sceneName)
    {
        if (this.isLoadingScene == false)
        { 
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            LoadingSceneManager.LoadScene(sceneName);
        }
    }

    public void PlayUISound(int soundIndex)
    {
        SoundManager.instance.PlaySound(ESoundType.UI, soundIndex);
    }
}
