using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using Services.Delegate;

namespace Services.Gui
{
    public class FadeEffect : MonoBehaviour
    {
        public static FadeEffect Instance { get; private set; }

        private Color beginFadeColor;
        private float beginWaitTerm;

        private float fadeDelayTerm;
        private float fadeAlphaWeight;

        [SerializeField]
        private Image fadeEffectImage;

        private void Awake()
        {
            if (FadeEffect.Instance == null)
            {
                FadeEffect.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            this.beginFadeColor = Color.black;
            this.beginWaitTerm = 0.5f;

            this.fadeDelayTerm = 0.2f;
            this.fadeAlphaWeight = 0.2f;
        }

        public void InActiveFadeEffectObject()
        {
            this.fadeEffectImage.enabled = false;
            this.fadeEffectImage.gameObject.SetActive(false);
        }

        public void StartCoroutineFadeEffect(bool isFadeIn)
        {
            this.fadeEffectImage.gameObject.SetActive(true);
            this.fadeEffectImage.enabled = true;

            this.beginFadeColor.a = isFadeIn == true ? 1.0f : 0.0f;
            this.fadeEffectImage.color = this.beginFadeColor;

            StartCoroutine(CoroutineFadeEffect(isFadeIn));
        }

        private IEnumerator CoroutineFadeEffect(bool isFadeIn)
        {
            WaitForSeconds WFS = new WaitForSeconds(this.fadeDelayTerm);
            Color fadeColor = this.fadeEffectImage.color;
            float fadeAlpha = this.beginFadeColor.a;
            float fadeInOutAlphaWeight = isFadeIn == true ? -1.0f : 1.0f;

            yield return new WaitForSeconds(this.beginWaitTerm);

            while (fadeAlpha >= 0.0f && fadeAlpha <= 1.0f)
            {
                fadeColor.a = fadeAlpha;
                this.fadeEffectImage.color = fadeColor;

                fadeAlpha += this.fadeAlphaWeight * fadeInOutAlphaWeight;

                yield return WFS;
            }

            if (fadeInOutAlphaWeight > 0)
            {
                fadeColor.a = 1.0f;
            }
            else
            {
                fadeColor.a = 0.0f;
            }

            this.fadeEffectImage.color = fadeColor;

            if (isFadeIn == true)
            {
                this.fadeEffectImage.enabled = false;
                this.fadeEffectImage.gameObject.SetActive(false);
            }
        }

        public void StartCoroutineFadeEffectWithLoadScene(delegateLoadScene delegateLoadScene, string sceneName, bool isFadeIn)
        {
            this.fadeEffectImage.gameObject.SetActive(true);
            this.fadeEffectImage.enabled = true;

            this.beginFadeColor.a = isFadeIn == true ? 1.0f : 0.0f;
            this.fadeEffectImage.color = this.beginFadeColor;

            StartCoroutine(CoroutineFadeEffectWithLoadScene(delegateLoadScene, sceneName, isFadeIn));
        }

        private IEnumerator CoroutineFadeEffectWithLoadScene(delegateLoadScene delegateLoadScene, string sceneName, bool isFadeIn)
        {
            WaitForSeconds WFS = new WaitForSeconds(this.fadeDelayTerm);
            Color fadeColor = this.fadeEffectImage.color;
            float fadeAlpha = this.beginFadeColor.a;
            float fadeInOutAlphaWeight = isFadeIn == true ? -1.0f : 1.0f;

            yield return new WaitForSeconds(this.beginWaitTerm);

            while (fadeAlpha >= 0.0f && fadeAlpha <= 1.0f)
            {
                fadeColor.a = fadeAlpha;
                this.fadeEffectImage.color = fadeColor;

                fadeAlpha += this.fadeAlphaWeight * fadeInOutAlphaWeight;

                yield return WFS;
            }

            if (fadeInOutAlphaWeight > 0)
            {
                fadeColor.a = 1.0f;
            }
            else
            {
                fadeColor.a = 0.0f;
            }

            this.fadeEffectImage.color = fadeColor;

            delegateLoadScene(sceneName);
        }
    }
}