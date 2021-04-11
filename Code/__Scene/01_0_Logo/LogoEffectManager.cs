using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogoEffectManager : MonoBehaviour
{
    private const string nextSceneName = "security-related";
    
    public class LogoEffectInformation
    {
        public Color logoEffectBeginColor;

        public float beginDelayTerm;
        public float middleDelayTerm;
        public float endDelayTerm;

        public float effectDelayTerm;

        public float fadeAlphaWeight;
    }

    private LogoEffectInformation info;
    private Image IMG_LogoEffect;

    private void Awake()
    {
        this.info = new LogoEffectInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitLogoEffectManager();
    }

    private void PrepareBaseObjects()
    {
        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.IMG_LogoEffect, GameObject.Find("MainCanvas"), "IMG_LogoEffect", true);
    }

    private void InitLogoEffectManager()
    {
        InitLogoEffectInformation();
        StartCoroutine(CoroutineLogoFadeEffect(1.0f, true));
    }

    private void InitLogoEffectInformation()
    {
        this.info.logoEffectBeginColor = Color.white;

        this.info.beginDelayTerm = 0.75f;
        this.info.middleDelayTerm = 1.5f;
        this.info.endDelayTerm = 0.75f;

        this.info.effectDelayTerm = 0.125f;
        this.info.fadeAlphaWeight = 0.15f;
    }

    private IEnumerator CoroutineLogoFadeEffect(float alphaValue, bool isFadeIn)
    {
        WaitForSeconds WFS;
        Color logoEffectBeginColor = this.info.logoEffectBeginColor;

        this.IMG_LogoEffect.enabled = true;

        yield return new WaitForSeconds(this.info.beginDelayTerm);

        WFS = new WaitForSeconds(this.info.effectDelayTerm);

        while (true)
        {
            if (isFadeIn == true && alphaValue <= 0.0f)
            {
                yield return new WaitForSeconds(this.info.middleDelayTerm);

                isFadeIn = false;
                alphaValue = 0.0f;
            }
            else if (isFadeIn == false && alphaValue >= 1.0f)
            {
                yield return new WaitForSeconds(this.info.endDelayTerm);

                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
                yield break;
            }

            alphaValue += this.info.fadeAlphaWeight * (isFadeIn == true ? -1 : 1);

            logoEffectBeginColor.a = alphaValue;
            this.IMG_LogoEffect.color = logoEffectBeginColor;

            yield return WFS;
        }
    }
}