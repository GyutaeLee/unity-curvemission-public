using System.Collections;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Services.Scene.Loading
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private Image progressbarImage;

        private void Start()
        {
            Sound.Bgm.Manager.Instance.Stop();
            StartCoroutine(LoadSceneWithProgressBar());
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
                    this.progressbarImage.fillAmount = Mathf.Lerp(this.progressbarImage.fillAmount, asyncOperation.progress, totalTime);

                    if (this.progressbarImage.fillAmount >= asyncOperation.progress)
                    {
                        totalTime = 0f;
                    }
                }
                else
                {
                    this.progressbarImage.fillAmount = Mathf.Lerp(this.progressbarImage.fillAmount, 1f, totalTime);

                    if (this.progressbarImage.fillAmount == 1.0f)
                    {
                        asyncOperation.allowSceneActivation = true;
                        yield break;
                    }
                }

            }
        }

        private static string nextSceneName;
        public static void LoadScene(string sceneName)
        {
            nextSceneName = sceneName;
            SceneManager.LoadScene(Services.Constants.SceneName.Loading);
        }
    }
}