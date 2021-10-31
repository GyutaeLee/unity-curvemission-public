using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEditor;

public class ClassMaker
{
    private string dataClassDirectoryPath = Application.dataPath + "security-related";

    [MenuItem("Csv/Refresh All Csv Class from Csv Files")]
    private static void RefreshAllCsvClass()
    {
        ClassMaker classMaker = new ClassMaker();
        classMaker.DeletePreviouslyExistingClass();

        const string DataDirectoryPath = "security-related";
        List<string> dataCsvNames = classMaker.FindCsvNameList(DataDirectoryPath);
        foreach (string dataCsvName in dataCsvNames)
        {
            classMaker.CreateCsvClass(dataCsvName);
        }

        AssetDatabase.Refresh();
    }

    private List<string> FindCsvNameList(string csvDirectoryPath)
    {
        DirectoryInfo csvDirectory = new DirectoryInfo(csvDirectoryPath);
        FileInfo[] csvFileInfos = csvDirectory.GetFiles();

        List<string> csvNames = new List<string>();
        foreach (var csv in csvFileInfos)
        {
            if (IsCsvFile(csv.Name) == true)
            {
                csvNames.Add(csv.Name);
            }
        }

        return csvNames;
    }

    private bool IsCsvFile(string file)
    {
        return file.EndsWith(".csv");
    }

    private void DeletePreviouslyExistingClass()
    {
        try
        {
            DirectoryInfo dataCsvClassDirectoryInfo = new DirectoryInfo(this.dataClassDirectoryPath);
            dataCsvClassDirectoryInfo.Delete(true);

            Debug.Log("Log : csv 클래스 폴더를 비웠습니다.");
        }
        catch (Exception)
        {
            Debug.Log("Log : csv 클래스 폴더에 파일이 없습니다.");
        }

    }

    private void CreateCsvClass(string csvName)
    {
        csvName = csvName.Replace(".csv", "");
        string assetCsvPath = "security-related";

        TextAsset csvData = Resources.Load(assetCsvPath) as TextAsset;
        const string LineSplitChars = @"\r\n|\n\r|\n|\r";
        string[] csvDataLines = Regex.Split(csvData.text, LineSplitChars);
        if (csvDataLines.Length <= Util.Csv.Data.Constants.MinimumLineLengthOfCsv)
        {
            Debug.LogError("error : " + assetCsvPath + " line 길이 에러");
            return;
        }

        string csvDirectoryPath = this.dataClassDirectoryPath;
        CheckAndCreateCsvClassDirectory(csvDirectoryPath);

        string writePath = csvDirectoryPath + $"/{csvName}.cs";
        StringBuilder classStringBuilder = CreateClassStringBuilder(csvName, csvDataLines);
        File.WriteAllText(writePath, classStringBuilder.ToString());
    }

    private void CheckAndCreateCsvClassDirectory(string csvClassDirectoryPath)
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(csvClassDirectoryPath);

        if (directoryInfo.Exists == false)
        {
            directoryInfo.Create();
        }
    }

    private StringBuilder CreateClassStringBuilder(string className, string[] csvDataLines)
    {
        StringBuilder classStringBuilder = new StringBuilder();

        bool isGenericExist = false;
        foreach (string csvDataLine in csvDataLines)
        {
            if (csvDataLine.Contains("List") == true)
            {
                isGenericExist = true;
            }
        }

        // Begin
        if (isGenericExist == true)
        {
            classStringBuilder.AppendLine("using System.Collections.Generic;");
        }

        classStringBuilder.AppendLine($"namespace Util.Csv.Data.Class");
        classStringBuilder.AppendLine("{");
        classStringBuilder.AppendLine($"    public class {className} : IData");
        classStringBuilder.AppendLine("    {");

        // Variables
        const string SplitChars = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        char[] trimChars = { '\"' };
        string[] variableNames = Regex.Split(csvDataLines[0], SplitChars);
        string[] dataTypes = Regex.Split(csvDataLines[1], SplitChars);
        for (uint i = 0; i < variableNames.Length && i < dataTypes.Length; i++)
        {
            string dataType = dataTypes[i];
            dataType = dataType.TrimStart(trimChars).TrimEnd(trimChars).Replace("\\", "");
            dataType = dataType.Replace("<br>", "\n");
            dataType = dataType.Replace("<c>", ",");

            classStringBuilder.AppendLine($"        public {dataType} {variableNames[i]} {{ get; set; }}");
        }

        // End
        classStringBuilder.AppendLine("    }");
        classStringBuilder.AppendLine("}");

        return classStringBuilder;
    }
}