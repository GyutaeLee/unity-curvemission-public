using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ESPRGameState
{
    None = 0,

    Intro = 1,
    Playing = 2,

    Pause = 3,
    End = 4,

    Max,
}

public class SPRGameManager : MonoBehaviour
{
    private static SPRGameManager _instance = null;
    public static SPRGameManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public class GameInformation
    {
        private ESPRGameState _eCurrentSPRGameState;
        public ESPRGameState eCurrentSPRGameState
        {
            get
            {
                return _eCurrentSPRGameState;
            }
            set
            {
                if (value <= ESPRGameState.None || value >= ESPRGameState.Max)
                {
                    _eCurrentSPRGameState = ESPRGameState.None;
                }
                else
                {
                    _eCurrentSPRGameState = value;
                }
            }
        }

        private int _coinQuantity;
        public int coinQuantity
        {
            get
            {
                return _coinQuantity;
            }
            set
            {
                if (value < 0)
                {
                    _coinQuantity = 0;
                }
                else
                {
                    _coinQuantity = value;
                }
            }
        }

        public int currentStageID;
    }

    private GameInformation info;

    private SPRCameraManager sprCameraManager;

    private SPRCarManager sprCarManager;
    private SPRUIManager sprUIManager;
    private SPRStageManager sprStageManager;
    private SPRLapManager sprLapManager;
    private SPRItemManager sprItemManager;

    private bool thread_wait_moveToFinish;

    private void Awake()
    {
        InitInstance();
        this.info = new GameInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitGameManager();
    }

    private void Update()
    {
        UpdateGame();
    }

    private void InitInstance()
    {
        if (SPRGameManager.instance == null)
        {
            SPRGameManager.instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void PrepareBaseObjects()
    {
        GameObject manager = GameObject.Find("Manager");

        if (this.sprCarManager == null)
        {
            this.sprCarManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Game"), "SPRCar", true).GetComponent<SPRCarManager>();
        }

        if (this.sprCameraManager == null)
        {
            this.sprCameraManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRCameraManager", true).GetComponent<SPRCameraManager>();
        }

        if (this.sprUIManager == null)
        {
            this.sprUIManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRUIManager", true).GetComponent<SPRUIManager>();
        }

        if (this.sprStageManager == null)
        {
            this.sprStageManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRStageManager", true).GetComponent<SPRStageManager>();
        }

        if (this.sprLapManager == null)
        {
            this.sprLapManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRLapManager", true).GetComponent<SPRLapManager>();
        }

        if (this.sprItemManager == null)
        {
            this.sprItemManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRItemManager", true).GetComponent<SPRItemManager>();
        }
    }

    private void InitGameManager()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.None;
        this.info.currentStageID = SecurityPlayerPrefs.GetInt("security-related", SPRStageManager.GetDefaultStageID());
    }

    private void UpdateGame()
    {
        if (IsGameStatePlaying() == false)
        {
            return;
        }

        this.sprLapManager.UpdateCurrentLapTime();
    }

    public void RefreshUICoin()
    {
        this.sprUIManager.RefreshUICoin();
    }

    public void StartGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.Playing;

        BgmManager.instance.LoadBgmResources(EBgmType.RacingStage, this.info.currentStageID);
        BgmManager.instance.PlayGameBgm(true);

        this.sprCarManager.SetCarEnable(true);
        this.sprUIManager.StartGame();
    }

    public void ReStartGame()
    {
        const string SPRSceneName = "security-related";

        Time.timeScale = 1.0f;
        LoadingSceneManager.LoadScene(SPRSceneName);
    }

    public void PauseGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.Pause;

        this.sprUIManager.PauseGame();

        BgmManager.instance.PauseGameBgm();
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.Playing;

        this.sprUIManager.ResumeGame();

        BgmManager.instance.ResumeGameBgm();
        Time.timeScale = 1.0f;
    }

    public void GoToVillage()
    {
        const string villageSceneName = "security-related";

        Time.timeScale = 1.0f;
        LoadingSceneManager.LoadScene(villageSceneName);
    }

    public void FinishGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.End;

        this.sprUIManager.FinishGame();
        this.sprCarManager.SetCarFinish();
        this.sprLapManager.CalculateResultLapTime();

        // Post user coin to Firebase DB
        ServerManager.instance.PostUserCoinToFirebaseDB(this.info.coinQuantity);

        // Update record & Set Finish Information
        bool isBestRecord = false;
        SPRFinishManager.SPRFinishInformation finishInfo = new SPRFinishManager.SPRFinishInformation();
        float srRecordTime = UserManager.instance.GetSRRecords(this.sprStageManager.GetCurrentStageID(), "security-related");

        if (srRecordTime > this.sprLapManager.GetCurrentLapTime() || srRecordTime == SPRLapManager.GetNoneLapTime())
        {
            isBestRecord = true;

            // Post user SPR record to Firebase DB
            ServerManager.instance.PostUserSPRRecordToFirebaseDB(this.sprStageManager.GetCurrentStageID(),
                                                                this.sprLapManager.GetCurrentLapTime(),
                                                                this.sprCarManager.GetCarInfoID(),
                                                                this.sprCarManager.GetCarPaintID());

            // Check user SPR record and Post SPR Ranking To Firebase DB
            delegateActiveFlag delegateF = new delegateActiveFlag(ActiveThreadMoveToFinish);
            InActiveThreadMoveToFinish();
            ServerManager.instance.CheckAndPostUserSPRRankingToFirebaseDB(this.sprStageManager.GetCurrentStageID(), delegateF);
        }
        else
        {
            isBestRecord = false;
            ActiveThreadMoveToFinish();
        }
        
        BgmManager.instance.StopGameBgm();
        SoundManager.instance.PlaySound(ESoundType.ETC, (int)ESoundETC.Finish);
        StartCoroutine(MoveToFinishScene(isBestRecord));
    }

    private void ActiveThreadMoveToFinish()
    {
        this.thread_wait_moveToFinish = true;
    }

    private void InActiveThreadMoveToFinish()
    {
        this.thread_wait_moveToFinish = false;
    }

    private bool GetThreadMoveToFinish()
    {
        return this.thread_wait_moveToFinish;
    }

    private IEnumerator MoveToFinishScene(bool isBestRecord)
    {
        delegateGetFlag delegateGetFlag = new delegateGetFlag(GetThreadMoveToFinish);
        yield return StartCoroutine(CMDelegate.CoroutineThreadWait(delegateGetFlag));

        if (GetThreadMoveToFinish() == false)
        {
            string errorText = string.Format(TextManager.instance.GetText(ETextType.Game, (int)EGameText.Error), EnumError.GetEGameErrorCodeString(EGameError.ThreadWaitTimeOver));
            PopupManager.instance.OpenCheckPopup(errorText);
            SceneManager.LoadScene(UserManager.instance.GetBeforeSceneName());
            yield break;
        }

        InstantiateAndSetFinishManager(isBestRecord);
        FadeOutAndLoadFinishScene();
    }

    private void InstantiateAndSetFinishManager(bool isBestRecord)
    {
        GameObject finsihManagerPrefab = Resources.Load("security-related") as GameObject;
        GameObject finishManagerObject = Instantiate(finsihManagerPrefab);
        DontDestroyOnLoad(finishManagerObject);

        SPRFinishManager finishManager = finishManagerObject.GetComponent<SPRFinishManager>();
        SPRFinishManager.SPRFinishInformation info = new SPRFinishManager.SPRFinishInformation();

        info.isBestRecord = isBestRecord;
        info.currentStageID = this.sprStageManager.GetCurrentStageID();
        info.currentUserRanking = 999; // TO DO : 수정 필요
        info.resultCoin = this.info.coinQuantity;
        info.resultLapTime = this.sprLapManager.GetResultLapTime();

        finishManager.SetSPRFinishInformation(info);
    }

    private void FadeOutAndLoadFinishScene()
    {
        FadeEffectManager fadeEffectManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "FadeEffectManager", true).GetComponent<FadeEffectManager>();
        FadeEffectManager.FadeEffectInformation fadeEffectInfo = new FadeEffectManager.FadeEffectInformation();

        delegateLoadScene delegateLS = new delegateLoadScene(SceneManager.LoadScene);

        fadeEffectManager.InitFadeEffectInformation(ref fadeEffectInfo, false);
        fadeEffectManager.StartCoroutineFadeEffectWithLoadScene(fadeEffectInfo, delegateLS, SPRFinishManager.kFinishSceneName);
    }

    public void FailGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.End;

        this.sprCarManager.SetCarDeath();
        this.sprUIManager.FailGame();

        ServerManager.instance.PostUserCoinToFirebaseDB(this.info.coinQuantity);

        // Sound
        BgmManager.instance.StopGameBgm();
        //SoundManager.instance.PlaySound(ESoundType.ETC, (int)ESoundETC.SOUND_ETC_DEATH);
    }

    private IEnumerator CoroutineSceneMove(float coroutineTerm, string sceneName)
    {
        yield return new WaitForSeconds(coroutineTerm);

        SceneManager.LoadScene(sceneName);
    }

    /* coin */
    public void AddCoinQuantity(int count)
    {
        this.info.coinQuantity += count;
    }

    public void ModifyCoinQuantity(int quantity)
    {
        this.info.coinQuantity = quantity;
    }

    /* etc */

    public bool IsGameStatePlaying()
    {
        if (this.info.eCurrentSPRGameState != ESPRGameState.Playing)
        {
            return false;
        }

        return true;
    }

    public void SetCurrentSPRGameState(ESPRGameState eCurrentSPRGameState)
    {
        this.info.eCurrentSPRGameState = eCurrentSPRGameState;
    }

    public ESPRGameState GetCurrentSPRGameState()
    {
        return this.info.eCurrentSPRGameState;
    }

    public void SetCoinQuantity(int coinQuantity)
    {
        this.info.coinQuantity = coinQuantity;
    }

    public int GetCoinQuantity()
    {
        return this.info.coinQuantity;
    }
}
