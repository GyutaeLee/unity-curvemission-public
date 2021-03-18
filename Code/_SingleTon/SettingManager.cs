using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public static SettingManager instance = null;

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
        this.info.eLanguageType = (ELanguageType)SecurityPlayerPrefs.GetInt("security-related", 1);
    }
}
