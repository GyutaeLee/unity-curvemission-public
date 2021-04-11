using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SPRIntroManager : MonoBehaviour
{
    public class GameIntroInformation
    {        
        public float beginDelayTerm;
        public float countDelayTerm;

        public int countDownSptCount;
        public int goSptCount;
    }

    private GameIntroInformation info;

    private FadeEffectManager fadeEffectManager;
    private FadeEffectManager.FadeEffectInformation fadeEffectInfo;

    private GameObject CANVAS_Intro;
    private Image IMG_CountDown;
    private Sprite[] SPT_CountDownArray;

    private void Awake()
    {
        this.info = new GameIntroInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRIntroManager();

        StartSPRIntro();
    }

    private void PrepareBaseObjects()
    {
        PrepareGameObject();
        PrepareFadeEffectObject();
    }

    private void PrepareGameObject()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Intro, mainCanvas, "CANVAS_Intro", true);
        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.IMG_CountDown, this.CANVAS_Intro, "IMG_CountDown", true);

        if (this.SPT_CountDownArray == null)
        {
            this.SPT_CountDownArray = Resources.LoadAll<Sprite>("security-related");
        }
    }

    private void PrepareFadeEffectObject()
    {
        GameObject fadeEffectObject = null;
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref fadeEffectObject, GameObject.Find("Manager"), "FadeEffectManager", true);

        this.fadeEffectManager = fadeEffectObject.GetComponent<FadeEffectManager>();
        this.fadeEffectInfo = new FadeEffectManager.FadeEffectInformation();
        this.fadeEffectManager.InitFadeEffectInformation(ref this.fadeEffectInfo, true);
    }

    private void InitSPRIntroManager()
    {
        this.info.beginDelayTerm = 2.0f;
        this.info.countDelayTerm = 0.1f;
        this.info.countDownSptCount = 27;
        this.info.goSptCount = 12;
    }

    private void StartSPRIntro()
    {
#if (UNITY_EDITOR)
        SPRGameManager.instance.StartGame();        
        CMObjectManager.FindGameObjectInAllChild(GameObject.Find("CANVAS_Fade"), "FadeObject", true).SetActive(false);
        Destroy(this.gameObject);
#else
        this.fadeEffectManager.StartCoroutineFadeEffect(this.fadeEffectInfo);
        StartCoroutine(CoroutineGameIntroCountDown(this.info.beginDelayTerm, this.info.countDelayTerm, this.info.countDownSptCount, this.info.goSptCount));
#endif
    }

    private IEnumerator CoroutineGameIntroCountDown(float beginDelayTerm, float countDelayTerm, int countDownSptCount, int goSptCount)
    {
        SoundManager.instance.PlaySound(ESoundType.Car, (int)ESoundCar.Engine_1);
        yield return new WaitForSeconds(beginDelayTerm);
        SPRGameManager.instance.SetCurrentSPRGameState(ESPRGameState.Intro);

        yield return StartCoroutine(CoroutineCountDownAnimation(countDelayTerm, countDownSptCount));
        SPRGameManager.instance.StartGame();
        yield return StartCoroutine(CoroutineGoAnimation(countDelayTerm, countDownSptCount, goSptCount));

        this.IMG_CountDown.enabled = false;
        Destroy(this.gameObject);
    }

    private IEnumerator CoroutineCountDownAnimation(float countDelayTerm, int countDownSptCount)
    {
        this.IMG_CountDown.enabled = true;
        SoundManager.instance.PlaySound(ESoundType.ETC, (int)ESoundETC.CountDown);

        WaitForSeconds WFS = new WaitForSeconds(countDelayTerm);
        for (int sptIndex = 0; sptIndex < countDownSptCount; sptIndex++)
        {
            if (sptIndex < this.SPT_CountDownArray.Length)
            {
                this.IMG_CountDown.sprite = this.SPT_CountDownArray[sptIndex];
                this.IMG_CountDown.SetNativeSize();
            }

            yield return WFS;
        }
    }

    private IEnumerator CoroutineGoAnimation(float countDelayTerm, int countDownSptCount, int goSptCount)
    {
        WaitForSeconds WFS = new WaitForSeconds(countDelayTerm);

        int totalSptCount = countDownSptCount + goSptCount;
        for (int sptIndex = countDownSptCount; sptIndex < totalSptCount; sptIndex++)
        {
            if (sptIndex < this.SPT_CountDownArray.Length)
            {
                this.IMG_CountDown.sprite = this.SPT_CountDownArray[sptIndex];
                this.IMG_CountDown.SetNativeSize();
            }

            yield return WFS;
        }
    }
}
