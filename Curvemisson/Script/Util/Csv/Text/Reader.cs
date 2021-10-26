using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Util.Csv.Text
{
    public class Reader
    {
        public static class Constants
        {
            public const int MinimumLineLengthOfCsv = 1;
        }

        private string csvPath;
        private string csvName;
        private Dictionary<int, List<string>> textDictionaries;

        public Reader(string csvName)
        {
            this.csvName = csvName;
            this.csvPath = "Csv/Text/" + csvName;
        }

        public void Read()
        {
            string csvPath = this.csvPath;
            TextAsset data = Resources.Load<TextAsset>(csvPath);

            const string LineSplitChars = @"\r\n|\n\r|\n|\r";
            string[] csvRows = Regex.Split(data.text, LineSplitChars);
            if (csvRows.Length <= Constants.MinimumLineLengthOfCsv)
            {
                Debug.LogError("error : " + csvPath + " line 길이 에러");
                return;
            }

            ConvertLowsToDatas(csvRows);
        }

        private void ConvertLowsToDatas(string[] lines)
        {
            const string SplitChars = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
            char[] trimChars = { '\"' };

            if (this.textDictionaries == null)
            {
                this.textDictionaries = new Dictionary<int, List<string>>();
            }

            string[] header = Regex.Split(lines[0], SplitChars);
            for (var i = Constants.MinimumLineLengthOfCsv; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SplitChars);

                if (values.Length == 0 || values[0] == "")
                {
                    continue;
                }

                List<string> entry = new List<string>();
                for (var j = 1; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(trimChars).TrimEnd(trimChars).Replace("\\", "");
                    value = value.Replace("<br>", "\n");
                    value = value.Replace("<c>", ",");

                    entry.Add(value);
                }

                int infoID = Int32.Parse(values[0]);
                this.textDictionaries[infoID] = entry;
            }
        }

        public void StoreDatasInStorage()
        {
            Storage.Instance.ChangeDatas(this.csvName, this.textDictionaries);
        }

        public string GetData(int infoID, int countryIndex)
        {
            return this.textDictionaries[infoID][countryIndex];
        }
    }
}