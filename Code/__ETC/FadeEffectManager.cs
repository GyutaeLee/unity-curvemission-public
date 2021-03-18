using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeEffectManager : MonoBehaviour
{
    public class FadeEffectInformation
    {        
        public Color fadeBeginColor;

        public bool isFadeIn;
        public float beginDelayTerm;
        public float fadeDelayTerm;
        public float fadeEffectTotalTime;
        public float fadeAlphaWeight;
    }

    private GameObject CANVAS_Fade;
    private GameObject fadeObject;
    private Image IMG_Fade;

    public void InitFadeEffectInformation(ref FadeEffectInformation fadeEffectInformation, bool isFadeIn)
    {
        if (this.CANVAS_Fade == null)
        {
            this.CANVAS_Fade = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("MainCanvas"), "CANVAS_Fade", true);
            this.CANVAS_Fade.SetActive(true);
        }

        if (this.fadeObject == null)
        {
            this.fadeObject = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Fade, "FadeObject", true);
        }

        if (this.IMG_Fade == null)
        {
            this.IMG_Fade = this.fadeObject.GetComponent<Image>();
        }

        fadeEffectInformation.fadeBeginColor = Color.black;
        fadeEffectInformation.fadeBeginColor.a = isFadeIn == true ? 1.0f : 0.0f;
        fadeEffectInformation.isFadeIn = isFadeIn;
        fadeEffectInformation.beginDelayTerm = 0.5f;
        fadeEffectInformation.fadeDelayTerm = 0.2f;
        fadeEffectInformation.fadeAlphaWeight = 0.2f;

        fadeEffectInformation.fadeEffectTotalTime = fadeEffectInformation.beginDelayTerm;
        fadeEffectInformation.fadeEffectTotalTime += 1.0f / (fadeEffectInformation.fadeAlphaWeight / fadeEffectInformation.fadeDelayTerm);
    }

    public void StartCoroutineFadeEffect(FadeEffectInformation fadeEffectInformation)
    {
        this.fadeObject.SetActive(true);
        this.IMG_Fade.enabled = true;
        this.IMG_Fade.color = fadeEffectInformation.fadeBeginColor;

        StartCoroutine(CoroutineFadeEffect(fadeEffectInformation));
    }

    public void StartCoroutineFadeEffectWithLoadScene(FadeEffectInformation fadeEffectInformation, delegateLoadScene delegateLS, string sceneName)
    {
        this.fadeObject.SetActive(true);
        this.IMG_Fade.enabled = true;
        this.IMG_Fade.color = fadeEffectInformation.fadeBeginColor;

        StartCoroutine(CoroutineFadeEffectWithLoadScene(fadeEffectInformation, delegateLS, sceneName));
    }

    private IEnumerator CoroutineFadeEffect(FadeEffectInformation fadeEffectInformation)
    {
        WaitForSeconds WFS = new WaitForSeconds(fadeEffectInformation.fadeDelayTerm);
        Color fadeColor = this.IMG_Fade.color;
        float fadeAlpha = fadeEffectInformation.fadeBeginColor.a;
        float fadeAlphaWeight = fadeEffectInformation.isFadeIn == true ? -1.0f : 1.0f;
        
        yield return new WaitForSeconds(fadeEffectInformation.beginDelayTerm);        

        while (fadeAlpha >= 0.0f && fadeAlpha <= 1.0f)
        {
            fadeColor.a = fadeAlpha;
            this.IMG_Fade.color = fadeColor;

            fadeAlpha += fadeEffectInformation.fadeAlphaWeight * fadeAlphaWeight;
            
            yield return WFS;
        }

        if (fadeAlphaWeight > 0)
        {
            fadeColor.a = 1.0f;
        }
        else
        {
            fadeColor.a = 0.0f;
        }

        this.IMG_Fade.color = fadeColor;

        if (fadeEffectInformation.isFadeIn == true)
        {
            this.fadeObject.SetActive(false);
            this.IMG_Fade.enabled = false;
        }
    }

    private IEnumerator CoroutineFadeEffectWithLoadScene(FadeEffectInformation fadeEffectInformation, delegateLoadScene delegateLS, string sceneName)
    {
        WaitForSeconds WFS = new WaitForSeconds(fadeEffectInformation.fadeDelayTerm);
        Color fadeColor = this.IMG_Fade.color;
        float fadeAlpha = fadeEffectInformation.fadeBeginColor.a;
        float fadeAlphaWeight = fadeEffectInformation.isFadeIn == true ? -1.0f : 1.0f;

        yield return new WaitForSeconds(fadeEffectInformation.beginDelayTerm);

        while (fadeAlpha >= 0.0f && fadeAlpha <= 1.0f)
        {
            fadeColor.a = fadeAlpha;
            this.IMG_Fade.color = fadeColor;

            fadeAlpha += fadeEffectInformation.fadeAlphaWeight * fadeAlphaWeight;

            yield return WFS;
        }

        if (fadeAlphaWeight > 0)
        {
            fadeColor.a = 1.0f;
        }
        else
        {
            fadeColor.a = 0.0f;
        }

        this.IMG_Fade.color = fadeColor;

        if (fadeEffectInformation.isFadeIn == true)
        {
            this.fadeObject.SetActive(false);
            this.IMG_Fade.enabled = false;
        }

        delegateLS(sceneName);
    }
}
