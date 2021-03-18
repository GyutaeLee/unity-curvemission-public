using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SPRIntroManager : MonoBehaviour, ICMInterface
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
    }

    public void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        if (this.CANVAS_Intro == null)
        {
            this.CANVAS_Intro = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Intro", true);
        }

        if (this.IMG_CountDown == null)
        {
            this.IMG_CountDown = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Intro, "IMG_CountDown", true).GetComponent<Image>();
        }

        if (this.SPT_CountDownArray == null)
        {
            this.SPT_CountDownArray = Resources.LoadAll<Sprite>("security-related");
        }

        this.info.beginDelayTerm = 2.0f;
        this.info.countDelayTerm = 0.1f;
        this.info.countDownSptCount = 27;
        this.info.goSptCount = 12;

        /* Fade Effect */
        if (this.fadeEffectManager == null)
        {
            this.fadeEffectManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "FadeEffectManager", true).GetComponent<FadeEffectManager>();
        }

        this.fadeEffectInfo = new FadeEffectManager.FadeEffectInformation();
        this.fadeEffectManager.InitFadeEffectInformation(ref this.fadeEffectInfo, true);
    }

    private void InitSPRIntroManager()
    {
#if (UNITY_EDITOR)
        SPRGameManager.instance.StartGame();
#else
        this.m_fadeEffectManager.StartCoroutineFadeEffect(this.fadeEffectInfo);
        StartCoroutine(CoroutineGameIntroCountDown(this.info.beginDelayTerm, this.info.countDelayTerm, this.info.countDownSptCount, this.info.goSptCount));
#endif
    }

    private IEnumerator CoroutineGameIntroCountDown(float beginDelayTerm, float countDelayTerm, int countDownSptCount, int goSptCount)
    {
        SoundManager.instance.PlaySound(ESoundType.Car, (int)ESoundCar.Engine_1);
        yield return new WaitForSeconds(beginDelayTerm);

        SPRGameManager.instance.SetCurrentSPRGameState(ESPRGameState.Intro);

        WaitForSeconds WFS = new WaitForSeconds(countDelayTerm);

        this.IMG_CountDown.enabled = true;
        SoundManager.instance.PlaySound(ESoundType.ETC, (int)ESoundETC.CountDown);

        // count down (3..2..1..)
        int sptIndex = 0;
        for (sptIndex = 0; sptIndex < countDownSptCount; sptIndex++)
        {
            if (sptIndex < this.SPT_CountDownArray.Length)
            {
                this.IMG_CountDown.sprite = this.SPT_CountDownArray[sptIndex];
                this.IMG_CountDown.SetNativeSize();
            }

            yield return WFS;
        }

        // Start game
        SPRGameManager.instance.StartGame();

        // "GO"
        int totalSptCount = countDownSptCount + goSptCount;
        for (; sptIndex < totalSptCount; sptIndex++)
        {
            if (sptIndex < this.SPT_CountDownArray.Length)
            {
                this.IMG_CountDown.sprite = this.SPT_CountDownArray[sptIndex];
                this.IMG_CountDown.SetNativeSize();
            }

            yield return WFS;
        }

        this.IMG_CountDown.enabled = false;
        Destroy(this.gameObject);
    }
}
