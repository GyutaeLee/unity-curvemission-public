using UnityEngine;

public enum EBgmType
{
    None = 0,

    Etc         = 1,
    Village     = 2,
    RacingStage = 3,

    Max,
}

public enum EBgmEtc
{
    None = 0,

    Intro       = 1,

    Max,
}

public enum EBgmVillage
{
    None = 0,

    Village     = 1,
    Shop        = 2,
    Garage      = 3,
    Ranking     = 4,
    Tutorial    = 5,

    Max,
}

public enum EBgmRacingStage
{
    None = 0,

    Stage_1     = 1,
    Stage_2     = 2,
    Stage_3     = 3,
    Stage_4     = 4,

    Max,
}

public class BgmManager : MonoBehaviour, ICMInterface
{
    public static BgmManager instance = null;

    public class GameBgmInformation
    {
        public bool isBgmChanged;

        public bool isBgmOff;
        public float bgmVolume;
    }

    private GameBgmInformation info;

    private AudioSource AS_Bgm;
    private AudioClip AC_Bgm;

    private void Awake()
    {
        InitInstance();
        this.info = new GameBgmInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitBgmManager();
    }

    private void InitInstance()
    {
        if (BgmManager.instance == null)
        {
            BgmManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareBaseObjects()
    {
        if (this.AS_Bgm == null)
        {
            if (this.GetComponent<AudioSource>() == null)
            {
                this.gameObject.AddComponent<AudioSource>();
            }

            this.AS_Bgm = this.GetComponent<AudioSource>();
        }

        this.info.isBgmOff = SecurityPlayerPrefs.GetInt("security-related", 0) == 1 ? true : false;
        this.info.bgmVolume = SecurityPlayerPrefs.GetFloat("security-related", 1.0f);

        this.AS_Bgm.playOnAwake = false;
        this.AS_Bgm.loop = false;
        this.AS_Bgm.volume = this.info.bgmVolume;
    }

    private void InitBgmManager()
    {

    }

    public void LoadBgmResources(EBgmType eBgmType, int bgmID)
    {
        string bgmPathName;
        string bgmResourceName;

        switch (eBgmType)
        {
            case EBgmType.Etc:
                bgmPathName = "security-related";
                bgmResourceName = "security-related";
                break;
            case EBgmType.Village:
                bgmPathName = "security-related";
                bgmResourceName = "security-related";
                break;
            case EBgmType.RacingStage:
                bgmPathName = "security-related";
                bgmResourceName = "security-related";
                break;
            default:
                bgmPathName = "";
                bgmResourceName = "";
                break;
        }

        int bgmIndex = GetBgmIndex(eBgmType, bgmID);
        bgmResourceName += bgmIndex;

        if (IsSameBgmResource(bgmResourceName) == true)
        {
            return;
        }

        string bgmResourcePathName = string.Format("security-related{0}{1:d}", bgmPathName, bgmIndex);

        this.AC_Bgm = Resources.Load(bgmResourcePathName) as AudioClip;
        this.info.isBgmChanged = true;
    }

    private bool IsSameBgmResource(string bgmResourceName)
    {
        bool isSameBgmResource = ((this.AC_Bgm != null) && (this.AC_Bgm.name == bgmResourceName));

        return isSameBgmResource;
    }

    public void PlayGameBgm(bool bLoop)
    {
        if (this.info.isBgmOff == true)
        {
            return;
        }

        if (this.info.isBgmChanged == false)
        {
            return;
        }

        if (this.AC_Bgm == null)
        {
            return;
        }

        StopGameBgm();

        this.AS_Bgm.clip = this.AC_Bgm;
        this.AS_Bgm.loop = bLoop;
        this.AS_Bgm.Play();

        this.info.isBgmChanged = false;
    }

    public void ResumeGameBgm()
    {
        this.AS_Bgm.Play();
    }

    public void StopGameBgm()
    {
        this.AS_Bgm.clip = null;
        this.AS_Bgm.loop = false;
        this.AS_Bgm.Stop();
    }

    public void PauseGameBgm()
    {
        this.AS_Bgm.Pause();
    }

    public void ClearGameBgm()
    {
        this.AC_Bgm = null;

        StopGameBgm();
    }

    private int GetBgmIndex(EBgmType eBgmType, int bgmID)
    {
        int bgmIndex = 0;

        switch (eBgmType)
        {
            case EBgmType.Etc:
                bgmIndex = bgmID;
                break;
            case EBgmType.Village:
                bgmIndex = bgmID;
                break;
            case EBgmType.RacingStage:
                bgmIndex = bgmID - 1000;
                break;
            default:
                bgmIndex = 0;
                break;
        }

        return bgmIndex;
    }
}
