using UnityEngine;
using UnityEngine.UI;

public class SPRUIManager : MonoBehaviour
{
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

    private SPRGameManager sprGameManager;
    private SPRLapManager sprLapManager;

#if (DEBUG_MODE)
    public float deltaTime;
    public float worstFPS;
    public float currentFPS;

    public GameObject CANVAS_Debug;

    public Text TXT_CurrentFrame;
    public Text TXT_WorstFrame;
#endif

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

    private void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Pause, mainCanvas, "CANVAS_Pause", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Playing, mainCanvas, "CANVAS_Playing", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Finish, mainCanvas, "CANVAS_Finish", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Death, mainCanvas, "CANVAS_Death", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_CurrentLapTime, this.CANVAS_Playing, "TXT_CurrentLapTime", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_CurrentCoin, this.CANVAS_Playing, "TXT_CurrentCoin", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_CurrentLapCount, this.CANVAS_Playing, "TXT_CurrentLapCount", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_FinishLapTime, this.CANVAS_Finish, "TXT_FinishLapTime", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_FinishCoin, this.CANVAS_Finish, "TXT_FinishCoin", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_DeathLapTime, this.CANVAS_Death, "TXT_DeathLapTime", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_DeathCoin, this.CANVAS_Death, "TXT_DeathCoin", true);

        this.sprGameManager = SPRGameManager.instance;

        if (this.sprLapManager == null)
        {
            this.sprLapManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "SPRLapManager", true).GetComponent<SPRLapManager>();
        }

        this.CANVAS_Playing.SetActive(true);
        this.CANVAS_Playing.SetActive(false);

#if (DEBUG_MODE)
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Debug, mainCanvas, "CANVAS_Debug", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_CurrentFrame, this.CANVAS_Playing, "TXT_CurrentFrame", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_WorstFrame, this.CANVAS_Playing, "TXT_WorstFrame", true);

        this.CANVAS_Debug.SetActive(true);
        this.worstFPS = 999;
#endif
    }

    private void InitSPRUIManager()
    {
        this.TXT_CurrentLapCount.text = string.Format("{0:d} / {1:d}", this.sprLapManager.GetCurrentLapCount(), this.sprLapManager.GetFinishLapCount());
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
    }

    public void FailGame()
    {
        this.CANVAS_Playing.SetActive(false);

        this.CANVAS_Death.SetActive(true);

        this.TXT_DeathLapTime.text = string.Format("{0:N3}", this.sprLapManager.GetCurrentLapTime());
    }

    public void UpdateUILapTime()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentLapTime.text = string.Format("{0:N3}", this.sprLapManager.GetCurrentLapTime());
    }

    public void RefreshUICoin()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentCoin.text = this.sprGameManager.GetCoinQuantity().ToString();
    }

    public void UpdateUILapCount()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.TXT_CurrentLapCount.text = string.Format("{0:d} / {1:d}", this.sprLapManager.GetCurrentLapCount(), this.sprLapManager.GetFinishLapCount());
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
