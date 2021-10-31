using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Services.Delegate;

using Newtonsoft.Json;

using Firebase.Database;

namespace Services.Scene.RankingStation
{
    public class RankingData
    {
        public Dictionary<string, int> Avatar { get; set; }     
        public Dictionary<string, float> Record { get; set; }   

        public string Nickname { get; set; }                    
        public string UserId { get; set; }
    }

    public class Main : MonoBehaviour
    {
        [SerializeField]
        private GameObject singleRacingRankingStationObject;

        private Dictionary<int, List<RankingData>> singleRacingRankingInformationDictionary;
        private DataSnapshot dataSnapshot = null;

        private void Start()
        {
            StartCoroutine(CoroutineInitializeSingleRacingRanking());
        }

        public void OpenSingleRacingRankingStation()
        {
            SingleRacingRankingStation singleRacingRankingStation = this.singleRacingRankingStationObject.AddComponent<SingleRacingRankingStation>();
            singleRacingRankingStation.Open(singleRacingRankingInformationDictionary);
        }

        private IEnumerator CoroutineInitializeSingleRacingRanking()
        {
            Server.Requester.RequestSingleRacingRankingFromFirebaseDB(SetDataSnashot);

            delegateGetFlag delegateGetFlag = new delegateGetFlag(Thread.Waiter.GetThreadWaitIsCompleted);
            yield return StartCoroutine(Thread.Waiter.CoroutineThreadWait(delegateGetFlag));

            if (Thread.Waiter.GetThreadWaitIsCompleted() == false)
            {
                DoRequestSingleRacingRankingFailedProcess();
                yield break;
            }

            InitializeSingleRacingRankingInformationDictionary();
        }

        private void SetDataSnashot(DataSnapshot dataSnapshot)
        {
            this.dataSnapshot = dataSnapshot;
        }

        private void DoRequestSingleRacingRankingFailedProcess()
        {
            string errorText = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Game, (int)Enum.GameText.Game.Error), Enum.Error.GameError.ThreadWaitTimeOver);
            Gui.Popup.Manager.Instance.OpenCheckPopup(errorText);

            SceneManager.LoadScene(User.User.Instance.BeforeSceneName);
        }

#if (UNITY_INCLUDE_TESTS)
        public bool IsSingleRacingRankingInformationSet { get; private set; }
#endif
        private void InitializeSingleRacingRankingInformationDictionary()
        {
            this.singleRacingRankingInformationDictionary = new Dictionary<int, List<RankingData>>();

            foreach (DataSnapshot stage in this.dataSnapshot.Children)
            {
                int stageID = int.Parse(stage.Key);

                List<RankingData> singleRacingRankingInformationList = GetSingleRacingRankingInformationList(stage);
                this.singleRacingRankingInformationDictionary.Add(stageID, singleRacingRankingInformationList);
                this.singleRacingRankingInformationDictionary[stageID].Sort(delegate (RankingData r1, RankingData r2)
                {
                    return r1.Record["security-related"].CompareTo(r2.Record["security-related"]);
                });
            }
#if (UNITY_INCLUDE_TESTS)
            IsSingleRacingRankingInformationSet = true;
#endif
        }

        private List<RankingData> GetSingleRacingRankingInformationList(DataSnapshot stage)
        {
            List<RankingData> singleRacingRankingInformationList = new List<RankingData>();

            foreach (DataSnapshot child in stage.Children)
            {
                RankingData singleRacingRankingInformation = new RankingData();
                string jsonData;

                jsonData = child.Child("security-related").GetRawJsonValue();
                singleRacingRankingInformation.Avatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

                jsonData = child.Child("security-related").GetRawJsonValue();
                singleRacingRankingInformation.Record = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonData);

                jsonData = child.Child("security-related").GetRawJsonValue();
                singleRacingRankingInformation.Nickname = JsonConvert.DeserializeObject<string>(jsonData);

                jsonData = child.Child("security-related").GetRawJsonValue();
                singleRacingRankingInformation.UserId = JsonConvert.DeserializeObject<string>(jsonData);

                singleRacingRankingInformationList.Add(singleRacingRankingInformation);
            }

            return singleRacingRankingInformationList;
        }
    }
}