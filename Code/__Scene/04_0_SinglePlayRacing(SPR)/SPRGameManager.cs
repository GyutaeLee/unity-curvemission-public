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

public class SPRGameManager : MonoBehaviour, ICMInterface
{
    public static SPRGameManager instance = null;

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

    public SPRCarManager sprCarManager;
    public SPRUIManager sprUIManager;
    public SPRMapManager sprMapManager;
    public SPRLapManager sprLapManager;
    public SPRItemManager sprItemManager;

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

    public void PrepareBaseObjects()
    {
        GameObject manager = GameObject.Find("Manager");

        this.info.eCurrentSPRGameState = ESPRGameState.None;
        this.info.currentStageID = SecurityPlayerPrefs.GetInt("security-related", 0);

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

        if (this.sprMapManager == null)
        {
            this.sprMapManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRMapManager", true).GetComponent<SPRMapManager>();
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

    }

    private void UpdateGame()
    {
        if (IsGameStatePlaying() == false)
        {
            return;
        }

        this.sprLapManager.UpdateCurrentLapTime();
    }

    public void StartGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.Playing;

        // Play Bgm
        BgmManager.instance.LoadBgmResources(EBgmType.RacingStage, this.info.currentStageID);
        BgmManager.instance.PlayGameBgm(true);

        // Active Car
        this.sprCarManager.SetCarEnable(true);

        // UI
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

        // UI 코드
        this.sprUIManager.PauseGame();

        // Sound
        BgmManager.instance.PauseGameBgm();
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.Playing;

        // UI 코드
        this.sprUIManager.ResumeGame();

        // Sound
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

        this.sprCarManager.SetCarFinish();
        this.sprUIManager.FinishGame();

        // Post user coin to Firebase DB
        ServerManager.instance.PostUserCoinToFirebaseDB(this.info.coinQuantity);

        // Update record
        float srRecordTime = UserManager.instance.GetSRRecords(this.sprMapManager.GetCurrentStageID(), "security-related");

        if (srRecordTime > this.sprLapManager.GetCurrentLapTime() || srRecordTime == SPRLapManager.kNoneLapTime)
        {
            // Post user SPR record to Firebase DB
            ServerManager.instance.PostUserSPRRecordToFirebaseDB(this.sprMapManager.GetCurrentStageID(),
                                                                this.sprLapManager.GetCurrentLapTime(),
                                                                this.sprCarManager.GetCarInfoID(),
                                                                this.sprCarManager.GetCarPaintID());

            // Check user SPR record and Post SPR Ranking To Firebase DB
            ServerManager.instance.CheckAndPostUserSPRRankingToFirebaseDB(this.sprMapManager.GetCurrentStageID());
        }

        // Sound
        BgmManager.instance.StopGameBgm();
        SoundManager.instance.PlaySound(ESoundType.ETC, (int)ESoundETC.Finish);
    }

    public void FailGame()
    {
        this.info.eCurrentSPRGameState = ESPRGameState.End;

        this.sprCarManager.SetCarDeath();
        this.sprUIManager.FailGame();

        // update coin
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
