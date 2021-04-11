using UnityEngine;

public class SettingManager : MonoBehaviour
{
    private static SettingManager _instance = null;
    public static SettingManager instance
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

    public class SettingInformation
    {
        public ELanguageType eLanguageType;
    }

    public SettingInformation info;

    private void Awake()
    {
        InitInstance();
        this.info = new SettingInformation();
    }

    private void Start()
    {
        InitSettingManager();
    }

    private void InitInstance()
    {
        if (SettingManager.instance == null)
        {
            SettingManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitSettingManager()
    {
        this.info.eLanguageType = (ELanguageType)SecurityPlayerPrefs.GetInt("security-related", (int)TextManager.GetDefaultLanguageType());
    }
}
