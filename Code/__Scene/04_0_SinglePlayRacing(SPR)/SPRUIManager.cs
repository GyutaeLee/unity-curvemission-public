using UnityEngine;
using UnityEngine.UI;

public class SPRUIManager : MonoBehaviour, ICMInterface
{
    public class SPRUIInformation
    {

    }

    public SPRUIInformation info;
    
    private GameObject CANVAS_Pause;

    private GameObject CANVAS_Playing;
    private Text TXT_CurrentLapTime;
    private Text TXT_CurrentCoin;
    private Text TXT_CurrentLapCount;

    private GameObject CANVAS_Finish;
    private Text TXT_FinishLapTime;
    private Text TXT_FinishCoin;

    private GameObject CANVAS_Death;
    private Text TXT_DeathLapTime;
    private Text TXT_DeathCoin;

    private SPRGameManager gameManager;

#if (DEBUG_MODE)
    public float deltaTime;
    public float worstFPS;
    public float currentFPS;

    public GameObject CANVAS_Debug;

    public Text TXT_CurrentFrame;
    public Text TXT_WorstFrame;
#endif

    private void Awake()
    {
        this.info = new SPRUIInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRUIManager();
    }

    private void Update()
    {
        UpdateUILapTime();

#if (DEBUG_MODE)
        UpdateDebugUI();
#endif
    }

    public void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        if (this.CANVAS_Pause == null)
        {
            this.CANVAS_Pause = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Pause", true);
        }

        if (this.CANVAS_Playing == null)
        {
            this.CANVAS_Playing = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Playing", true);
        }

        if (this.TXT_CurrentLapTime == null)
        {
            this.TXT_CurrentLapTime = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Playing, "TXT_CurrentLapTime", true).GetComponent<Text>();
        }

        if (this.TXT_CurrentCoin == null)
        {
            this.TXT_CurrentCoin = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Playing, "TXT_CurrentCoin", true).GetComponent<Text>();
        }

        if (this.TXT_CurrentLapCount == null)
        {
            this.TXT_CurrentLapCount = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Playing, "TXT_CurrentLapCount", true).GetComponent<Text>();
        }

        if (this.CANVAS_Finish == null)
        {
            this.CANVAS_Finish = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Finish", true);
        }

        if (this.TXT_FinishLapTime == null)
        {
            this.TXT_FinishLapTime = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Finish, "TXT_FinishLapTime", true).GetComponent<Text>();
        }

        if (this.TXT_FinishCoin == null)
        {
            this.TXT_FinishCoin = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Finish, "TXT_FinishCoin", true).GetComponent<Text>();
        }

        if (this.CANVAS_Death == null)
        {
            this.CANVAS_Death = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Death", true);
        }

        if (this.TXT_DeathLapTime == null)
        {
            this.TXT_DeathLapTime = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Death, "TXT_DeathLapTime", true).GetComponent<Text>();
        }

        if (this.TXT_DeathCoin == null)
        {
            this.TXT_DeathCoin = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Death, "TXT_DeathCoin", true).GetComponent<Text>();
        }

        this.CANVAS_Playing.SetActive(true);
        this.CANVAS_Playing.SetActive(false);

        this.gameManager = SPRGameManager.instance;
        this.TXT_CurrentLapCount.text = string.Format("{0:d} / {1:d}", this.gameManager.sprLapManager.GetCurrentLapCount(), this.gameManager.sprLapManager.GetFinishLapCount());

#if (DEBUG_MODE)
        if (this.CANVAS_Debug == null)
        {
            this.CANVAS_Debug = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Debug", true);
        }

        if (this.TXT_CurrentFrame == null)
        {
            this.TXT_CurrentFrame = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Debug, "TXT_CurrentFrame", true).GetComponent<Text>();
        }

        if (this.TXT_WorstFrame == null)
        {
            this.TXT_WorstFrame = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Debug, "TXT_WorstFrame", true).GetComponent<Text>();
        }

        this.CANVAS_Debug.SetActive(true);
        this.worstFPS = 999;
#endif
    }

    private void InitSPRUIManager()
    {

    }
      

    public void StartGame()
    {
        this.CANVAS_Playing.SetActive(true);
    }

    public void PauseGame()
    {
        this.CANVAS_Pause.SetActive(true);
        this.CANVAS_Playing.SetActive(false);
    }

    public void ResumeGame()
    {
        this.CANVAS_Pause.SetActive(false);
        this.CANVAS_Playing.SetActive(true);
    }

    public void FinishGame()
    {
        this.CANVAS_Playing.SetActive(false);

        // TO DO : 관련 UI 띄우기
        FailGame();
    }

    public void FailGame()
    {
        this.CANVAS_Playing.SetActive(false);

        this.CANVAS_Death.SetActive(true);

        this.TXT_DeathLapTime.text = string.Format("{0:N3}", this.gameManager.sprLapManager.GetCurrentLapTime());
    }

    public void UpdateUILapTime()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentLapTime.text = string.Format("{0:N3}", this.gameManager.sprLapManager.GetCurrentLapTime());
    }

    public void UpdateUICoin()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentCoin.text = this.gameManager.GetCoinQuantity().ToString();
    }

    public void UpdateUILapCount()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentLapCount.text = string.Format("{0:d} / {1:d}", this.gameManager.sprLapManager.GetCurrentLapCount(), this.gameManager.sprLapManager.GetFinishLapCount());
    }

    public void PlayUISound(int soundIndex)
    {
        SoundManager.instance.PlaySound(ESoundType.UI, soundIndex);
    }

#if (DEBUG_MODE)
    public void UpdateDebugUI()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        currentFPS = 1.0f / deltaTime;
        worstFPS = (worstFPS < currentFPS) ? worstFPS : currentFPS;

        TXT_CurrentFrame.text = string.Format("NOW   : {0:N2}", currentFPS);
        TXT_WorstFrame.text = string.Format("WORST : {0:N2}", worstFPS);
    }
#endif
}
