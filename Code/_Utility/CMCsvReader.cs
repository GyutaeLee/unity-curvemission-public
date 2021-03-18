using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public struct CsvData
{
    public List<Dictionary<string, string>> Data;
}

public class CMCsvReader
{
    private CsvData csvData;
    private string csvName;

    private static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    private static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    private static char[] TRIM_CHARS = { '\"' };

    public CMCsvReader(string csvName)
    {
        this.csvName = csvName;
    }

    public void SetCsvFileName(string csvName)
    {
        this.csvName = csvName;
    }

    /* virtual */

    public virtual void ReadCsvFile()
    {
        string csvPath;

#if   (DEV_MODE)
        csvPath = "security-related" + this.csvName;
#elif (RELEASE_MODE)
        csvPath = "security-related" + csvName;
#else
        csvPath = "security-related" + csvName;
        Debug.LogError("CSVReader ERROR : 개발 모드 에러");
#endif
        this.csvData.Data = this.Read(csvPath);

        if (this.csvData.Data == null)
        {
            Debug.LogError("ReadCSVFile ERROR : CSV Read 실패");
            return;
        }
    }

    // TO DO : 이렇게 하려면 "security-related"를 항상 오름차순으로만 넣어야한다. 추후에 확인 필요
    public virtual int GetCsvInfoID(int index)
    {
        return int.Parse(this.csvData.Data[index]["security-related"]);
    }

    public virtual float GetCsvFloatData(int infoID, string key)
    {
        Dictionary<string, string> data = GetDataByInfoID(infoID);

        if (data == null)
        {
            return 0;
        }

        return float.Parse(data[key].ToString());
    }

    public virtual int GetCsvIntData(int infoID, string key)
    {
        Dictionary<string, string> data = GetDataByInfoID(infoID);

        if (data == null)
        {
            return 0;
        }

        return int.Parse(data[key].ToString());
    }

    public virtual List<float> GetCsvFloatListData(int infoID, string key)
    {
        Dictionary<string, string> data = GetDataByInfoID(infoID);

        if (data == null)
        {
            return null;
        }

        string[] array = data[key].Trim('[', ']').Split(',');
        List<float> result = new List<float>();

        for (int i = 0; i < array.Length; i++)
        {
            result.Add(float.Parse(array[i]));
        }

        return result;
    }

    public virtual void PrintCsvData()
    {
        if (this.csvData.Data == null)
        {
            Debug.Log("PrintCSVData ERROR : CSV 데이터가 없습니다.");
            return;
        }

        for (int i = 0; i < this.csvData.Data.Count; i++)
        {
            string msg = "";

            foreach (KeyValuePair<string, string> items in this.csvData.Data[i])
            {
                msg += items.Key + " : " + items.Value + " / "; 
            }
            Debug.Log("INDEX " + i + " - " + msg);
        }
    }

    /* real */

    private Dictionary<string, string> GetDataByInfoID(int infoID)
    {
        string id = infoID.ToString();

        for (int i = 0; i < this.csvData.Data.Count; i++)
        {
            if (this.csvData.Data[i]["security-related"] != id)
            { 
                continue;
            }   

            return this.csvData.Data[i];
        }

        return null;
    }

    private List<Dictionary<string, string>> Read(string csvPath)
    {
        TextAsset data = Resources.Load(csvPath) as TextAsset;
        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        if (lines.Length <= 1)
        {
            Debug.LogError("CSVReader ERROR : " + csvPath + " line 길이 에러");
            return null;
        }

        List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (int i = 1; i < lines.Length; i++)
        {
            var values = Regex.Split(lines[i], SPLIT_RE);

            if (values.Length == 0 || values[0] == "")
            {
                continue;
            }

            var entry = new Dictionary<string, string>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                value = value.Replace("<br>", "\n");
                value = value.Replace("<c>", ",");

                entry[header[j]] = value;
            }
            list.Add(entry);
        }

        return list;
    }

    /* etc */

    public CsvData GetCsvData()
    {
        return this.csvData;
    }
}
