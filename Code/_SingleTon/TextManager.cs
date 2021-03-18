using System.Collections.Generic;
using UnityEngine;

public enum ELanguageType
{
    None = 0,

    Eng = 1,        // 영어
    Kor = 2,        // 한국어
    Jpa = 3,        // 일본어
    Chi_Hant = 4,    // 중국어 - 번체
    Chi_Hans = 5,    // 중국어 - 간체

    Max,
}

public enum ETextType
{
    Game = 0,
    Shop = 1,
    Car = 2,
    Paint = 3,
    Parts = 4,

    Max,
}

public class TextManager : MonoBehaviour
{
    public static TextManager instance = null;

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

        for (int i = 0; i < (int)ETextType.Max; i++)
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
                result = "security-related";
                break;
            case ELanguageType.Kor:
                result = "security-related";
                break;
            case ELanguageType.Jpa:
                result = "security-related";
                break;
            case ELanguageType.Chi_Hant:
                result = "security-related";
                break;
            case ELanguageType.Chi_Hans:
                result = "security-related";
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
        string csvPath = "security-related" + ConvertETextTypeToCsvName(eTextType);
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
            this.info.text[(int)eTextType].Add(infoID, csvReader.GetCsvData().Data[i][languageKey]);
        }
    }

    public string GetText(ETextType eTextType, int textKey)
    {
        if (eTextType == ETextType.Max)
        {
            return this.info.text[(int)ETextType.Game][((int)EGameText.Text_Error).ToString()];
        }

        string text = this.info.text[(int)eTextType][textKey.ToString()];

        if (text == null)
        {
            text = this.info.text[(int)ETextType.Game][((int)EGameText.Text_Error).ToString()];
        }

        return text;
    }
}
