using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneManager : MonoBehaviour
{
    private static string nextSceneName;
    private const string kLoadSceneName = "security-related";

    public Image IMG_ProgressBar;

    private void Start()
    {
        PrepareBaseObjects();
        InitLoadingSceneManager();

        StartCoroutine(LoadSceneWithProgressBar());   
    }

    private void PrepareBaseObjects()
    {
        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.IMG_ProgressBar, GameObject.Find("MainCanvas"), "IMG_ProgressBar", true);
    }

    private void InitLoadingSceneManager()
    {
        BgmManager.instance.ClearGameBgm();
    }

    public static void LoadScene(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene(kLoadSceneName);
    }

    private IEnumerator LoadSceneWithProgressBar()
    {
        yield return null;

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);

        asyncOperation.allowSceneActivation = false;

        float totalTime = 0.0f;

        while (asyncOperation.isDone == false)
        {
            yield return null;

            totalTime += Time.deltaTime;

            if (asyncOperation.progress < 0.9f)
            {
                this.IMG_ProgressBar.fillAmount = Mathf.Lerp(this.IMG_ProgressBar.fillAmount, asyncOperation.progress, totalTime);

                if (this.IMG_ProgressBar.fillAmount >= asyncOperation.progress)
                {
                    totalTime = 0f;
                }
            }
            else
            {
                this.IMG_ProgressBar.fillAmount = Mathf.Lerp(this.IMG_ProgressBar.fillAmount, 1f, totalTime);

                if (this.IMG_ProgressBar.fillAmount == 1.0f)
                {
                    asyncOperation.allowSceneActivation = true;
                    yield break;
                }
            }

        }
    }
}
