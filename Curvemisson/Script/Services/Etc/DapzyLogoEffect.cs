using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Etc
{
    public class DapzyLogoEffect : MonoBehaviour
    {
        private string nextSceneName;

        [SerializeField]
        private Color logoEffectBeginColor;

        [SerializeField]
        private float beginDelayTerm;
        [SerializeField]
        private float middleDelayTerm;
        [SerializeField]
        private float endDelayTerm;

        [SerializeField]
        private float effectDelayTerm;

        [SerializeField]
        private float fadeAlphaWeight;

        [SerializeField]
        private Image fadeInOutBox;

        private void Start()
        {
            nextSceneName = Constants.SceneName.Intro;
            StartCoroutine(CoroutineLogoFadeEffect(1.0f, true));
        }

        private IEnumerator CoroutineLogoFadeEffect(float alphaValue, bool isFadeIn)
        {
            this.fadeInOutBox.enabled = true;
            yield return new WaitForSeconds(this.beginDelayTerm);

            WaitForSeconds WFS = new WaitForSeconds(this.effectDelayTerm);
            Color logoEffectBeginColor = this.logoEffectBeginColor;
            while (true)
            {
                if (isFadeIn == true && alphaValue <= 0.0f)
                {
                    yield return new WaitForSeconds(this.middleDelayTerm);

                    isFadeIn = false;
                    alphaValue = 0.0f;
                }
                else if (isFadeIn == false && alphaValue >= 1.0f)
                {
                    yield return new WaitForSeconds(this.endDelayTerm);

                    UnityEngine.SceneManagement.SceneManager.LoadScene(this.nextSceneName);
                    yield break;
                }

                alphaValue += this.fadeAlphaWeight * (isFadeIn == true ? -1 : 1);

                logoEffectBeginColor.a = alphaValue;
                this.fadeInOutBox.color = logoEffectBeginColor;

                yield return WFS;
            }
        }
    }
}