using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Newtonsoft.Json;

using Firebase.Database;

// TO DO : UI 코드 나누기
public class SPRRankingManager : MonoBehaviour, ICMInterface
{
    public class SPRRankingInformation
    {
        public Dictionary<string, int> avatar;        

        public Dictionary<string, float> srRecords;   

        public string nickname;                       
        public string uid;
    }

    public class SPRRankingUI
    {
        public Image IMG_topRankerAvatar;
        public Image IMG_topRankerBestLapCar;
        public Text TXT_topRankerBestLapTime;
        public Text TXT_topRankerNickname;

        public Sprite SPT_topRankerBestLapCar;
    }

    private Dictionary<int, List<SPRRankingUI>> sprRankingUI;
    private Dictionary<int, List<SPRRankingInformation>> sprRankingInfo;

    private DataSnapshot dataSnapshot = null;
    private bool isDone = false; // TO DO : 함수별로 여러 쓰레드 사용할 떄 어떻게 나눌지 고민하기

    private void Start()
    {
        PrepareBaseObjects();
        InitRankingManager();
    }

    public void PrepareBaseObjects()
    {

    }

    private void InitRankingManager()
    {
        StartCoroutine(CoroutineInitSPRRanking());
    }

    private IEnumerator CoroutineInitSPRRanking()
    {
        RequestSPRRankingFromFirebaseDB();

        while (this.isDone == false)
        {
            yield return null;
        }

        SetSPRRanking();
    }

    private void SetSPRRanking()
    {
        this.sprRankingInfo = new Dictionary<int, List<SPRRankingInformation>>();

        foreach (DataSnapshot stage in this.dataSnapshot.Children)
        {
            int stageID = int.Parse(stage.Key);

            // 1. Get Ranking Info List
            List<SPRRankingInformation> SPRRankingInfoList = GetSPRRankingInfoList(stage);

            // 2. Add to Dictionary
            this.sprRankingInfo.Add(stageID, SPRRankingInfoList);

            // 3. Sort Dictionary by "time"
            this.sprRankingInfo[stageID].Sort(delegate (SPRRankingInformation r1, SPRRankingInformation r2)
            {
                return r1.srRecords["security-related"].CompareTo(r2.srRecords["security-related"]);
            });

            // 4. Set UI
            SetSPRRankingUI(stageID);
        }
    }

    private List<SPRRankingInformation> GetSPRRankingInfoList(DataSnapshot stage)
    {        
        List<SPRRankingInformation> SPRRankingInfoList = new List<SPRRankingInformation>();

        foreach (DataSnapshot child in stage.Children)
        {
            SPRRankingInformation SPRRankingInfo = new SPRRankingInformation();
            string jsonData;

            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.avatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.srRecords = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonData);

            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.nickname = JsonConvert.DeserializeObject<string>(jsonData);

            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.uid = JsonConvert.DeserializeObject<string>(jsonData);

            // Add to List
            SPRRankingInfoList.Add(SPRRankingInfo);
        }

        return SPRRankingInfoList;
    }

    private void SetSPRRankingUI(int stageID)
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");
        GameObject SPRRankingCanvas = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "SPRRankingCanvas", true);
        GameObject stageObject = CMObjectManager.FindGameObjectInAllChild(SPRRankingCanvas, stageID.ToString(), true);

        int rankingIndex = 0;

        foreach (SPRRankingInformation rankingInfo in this.sprRankingInfo[stageID])
        {
            /* Set UI */
            // TO DO : 이미지 세팅
            GameObject indexObject = CMObjectManager.FindGameObjectInAllChild(stageObject, rankingIndex.ToString(), true);
            SPRRankingUI tUI = new SPRRankingUI();

            // avatar

            // records
            tUI.IMG_topRankerBestLapCar = CMObjectManager.FindGameObjectInAllChild(indexObject, "IMG_topRankerBestLapCar", true).GetComponent<Image>();

            // Load the sprites from a sprite sheet file (png).
            string carSpriteSheetName = "Texture/CarItem/Car/" + (int)rankingInfo.srRecords["security-related"] + "/" + (int)rankingInfo.srRecords["security-related"];
            tUI.SPT_topRankerBestLapCar = Resources.Load<Sprite>(carSpriteSheetName);
            tUI.IMG_topRankerBestLapCar.sprite = tUI.SPT_topRankerBestLapCar;

            // time
            tUI.TXT_topRankerBestLapTime = CMObjectManager.FindGameObjectInAllChild(indexObject, "TXT_topRankerBestLapTime", true).GetComponent<Text>();
            tUI.TXT_topRankerBestLapTime.text = rankingInfo.srRecords["security-related"].ToString("F3");

            // nickname
            tUI.TXT_topRankerNickname = CMObjectManager.FindGameObjectInAllChild(indexObject, "TXT_topRankerNickname", true).GetComponent<Text>();
            tUI.TXT_topRankerNickname.text = rankingInfo.nickname;

            rankingIndex++;
        }
    }

    private void RequestSPRRankingFromFirebaseDB()
    {
        string baseKey = "security-related";

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).LimitToFirst(ServerManager.kServerMaxRankingCount).
            GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                this.isDone = true;
                this.dataSnapshot = task.Result;
            }
        });
    }
}