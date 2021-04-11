using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    private static TextManager _instance = null;
    public static TextManager instance
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

    private const ELanguageType kDefaultLanguageType = ELanguageType.Eng;

    private class TextInformation
    {
        public List<Dictionary<string, string>> text;
    }

    private TextInformation info;

    private void Awake()
    {
        InitInstance();
        this.info = new TextInformation();
    }

    private void Start()
    {
        InitTextManager();
    }

    private void InitInstance()
    {
        if (TextManager.instance == null)
        {
            TextManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitTextManager()
    {
        this.info.text = new List<Dictionary<string, string>>((int)ETextType.Max);

        for (int i = (int)ETextType.None + 1; i < (int)ETextType.Max; i++)
        {
            this.info.text.Add(new Dictionary<string, string>());
            ReadTextCsv(SettingManager.instance.info.eLanguageType, (ETextType)i);
        }
    }

    private string ConvertELanguageTypeToString(ELanguageType eLanguageType)
    {
        string result = "";

        switch (eLanguageType)
        {
            case ELanguageType.Eng:
                result = "Eng";
                break;
            case ELanguageType.Kor:
                result = "Kor";
                break;
            case ELanguageType.Jpa:
                result = "Jap";
                break;
            case ELanguageType.Chi_Hant:
                result = "Chi_Hant";
                break;
            case ELanguageType.Chi_Hans:
                result = "Chi_Hans";
                break;
            default:
                break;
        }

        return result;
    }

    private string ConvertETextTypeToCsvName(ETextType eTextType)
    {
        string name = "";

        switch (eTextType)
        {
            case ETextType.Game:
                name = "security-related";
                break;
            case ETextType.Shop:
                name = "security-related";
                break;
            case ETextType.Car:
                name = "security-related";
                break;
            case ETextType.Paint:
                name = "security-related";
                break;
            case ETextType.Parts:
                name = "security-related";
                break;
            default:
                break;
        }

        return name;
    }

    private void ReadTextCsv(ELanguageType eLanguageType, ETextType eTextType)
    {
        string csvPath = "security-related/" + ConvertETextTypeToCsvName(eTextType);
        CMCsvReader csvReader = new CMCsvReader(csvPath);
        csvReader.ReadCsvFile();

        if (eLanguageType == ELanguageType.None || eLanguageType >= ELanguageType.Max)
        {
            eLanguageType = SettingManager.instance.info.eLanguageType;
        }

        string languageKey = ConvertELanguageTypeToString(eLanguageType);

        for (int i = 0; i < csvReader.GetCsvData().Data.Count; i++)
        {
            string infoID = csvReader.GetCsvData().Data[i]["security-related"];
            this.info.text[ConvertTextTypeToIndex(eTextType)].Add(infoID, csvReader.GetCsvData().Data[i][languageKey]);
        }
    }

    public string GetText(ETextType eTextType, int textKey)
    {
        if (eTextType == ETextType.Max)
        {
            return this.info.text[ConvertTextTypeToIndex(ETextType.Game)][((int)EGameText.Error).ToString()];
        }

        string text = this.info.text[ConvertTextTypeToIndex(eTextType)][textKey.ToString()];

        if (text == null)
        {
            text = this.info.text[ConvertTextTypeToIndex(ETextType.Game)][((int)EGameText.Error).ToString()];
        }

        return text;
    }

    private int ConvertTextTypeToIndex(ETextType eTextType)
    {
        return (int)eTextType - 1;
    }

    public static ELanguageType GetDefaultLanguageType()
    {
        return kDefaultLanguageType;
    }
}
