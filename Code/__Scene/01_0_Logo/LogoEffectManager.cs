using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LogoEffectManager : MonoBehaviour, ICMInterface
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

    public void PrepareBaseObjects()
    {
        if (this.IMG_LogoEffect == null)
        {
            this.IMG_LogoEffect = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("MainCanvas"), "IMG_LogoEffect", true).GetComponent<Image>();
        }
    }

    private void InitLogoEffectManager()
    {
        // 1. setting data
        this.info.logoEffectBeginColor = Color.white;

        this.info.beginDelayTerm = 0.75f;
        this.info.middleDelayTerm = 1.5f;
        this.info.endDelayTerm = 0.75f;

        this.info.effectDelayTerm = 0.125f;
        this.info.fadeAlphaWeight = 0.15f;

        // 2. effect start
        StartCoroutine(CoroutineLogoFadeEffect(1.0f, true));
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
            // 1. FadeIn
            if (isFadeIn == true && alphaValue <= 0.0f)
            {
                yield return new WaitForSeconds(this.info.middleDelayTerm);

                isFadeIn = false;
                alphaValue = 0.0f;
            }
            // 2. FadeOut
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