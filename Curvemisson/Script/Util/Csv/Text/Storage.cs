using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util.Csv.Text
{
    public class Storage
    {
        private static Storage _instance;
        public static Storage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Storage();
                }
                return _instance;
            }
        }

        // csvName -> infoID -> country
        private Dictionary<string, Dictionary<int, List<string>>> texts;

        public void ChangeDatas(string csvName, Dictionary<int, List<string>> textDictionaries)
        {
            if (this.texts == null)
            {
                this.texts = new Dictionary<string, Dictionary<int, List<string>>>();
            }

            this.texts[csvName] = textDictionaries;
        }

        public string GetText(string csvName, int infoID, int countryIndex)
        {
            if (this.texts == null)
            {
                ReadAndStoreDatas(csvName);
            }

            if (this.texts.ContainsKey(csvName) == false)
                return "";

            if (this.texts[csvName].ContainsKey(infoID) == false)
                return "";

            if (this.texts[csvName][infoID].Count <= countryIndex)
                return "";

            return this.texts[csvName][infoID][countryIndex];
        }

        private void ReadAndStoreDatas(string csvName)
        {
            Reader reader = new Reader(csvName);

            reader.Read();
            reader.StoreDatasInStorage();
        }
    }

}