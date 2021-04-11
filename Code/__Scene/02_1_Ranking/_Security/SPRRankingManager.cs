using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using Newtonsoft.Json;

using Firebase.Database;

// TO DO : UI 코드 나누기
public class SPRRankingManager : MonoBehaviour
{
    public class SPRRankingInformation
    {
        public Dictionary<string, int> avatar;        // hair, head, earring, face, top, hand, bottom, shoe
        public Dictionary<string, float> srRecords;   // records { time, car, paint}

        public string nickname;                       // nickname
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
    private bool thread_wait_requestRanking = false;

    private void Start()
    {
        InitRankingManager();
    }

    private void InitRankingManager()
    {
        StartCoroutine(CoroutineInitSPRRanking());
    }

    private IEnumerator CoroutineInitSPRRanking()
    {
        InActiveThreadRequestRanking();
        RequestSPRRankingFromFirebaseDB();

        delegateGetFlag delegateGetFlag = new delegateGetFlag(GetThreadRequestRanking);
        yield return StartCoroutine(CMDelegate.CoroutineThreadWait(delegateGetFlag));

        if (GetThreadRequestRanking() == false)
        {
            string errorText = string.Format(TextManager.instance.GetText(ETextType.Game, (int)EGameText.Error), EnumError.GetEGameErrorCodeString(EGameError.ThreadWaitTimeOver));
            PopupManager.instance.OpenCheckPopup(errorText);
            SceneManager.LoadScene(UserManager.instance.GetBeforeSceneName());            
            yield break;
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

            // avatar
            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.avatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            // records
            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.srRecords = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonData);

            // nickname
            jsonData = child.Child("security-related").GetRawJsonValue();
            SPRRankingInfo.nickname = JsonConvert.DeserializeObject<string>(jsonData);

            // uid
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
            //tUI.IMG_topRankerAvatar = 

            // records
            tUI.IMG_topRankerBestLapCar = CMObjectManager.FindGameObjectInAllChild(indexObject, "IMG_topRankerBestLapCar", true).GetComponent<Image>();

            // Load the sprites from a sprite sheet file (png).
            string carSpriteSheetName = "security-related" + (int)rankingInfo.srRecords["security-related"] + "/" + (int)rankingInfo.srRecords["security-related"];
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

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).LimitToFirst(ServerManager.GetServerSPRRankingMaxCount()).
            GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                ActiveThreadRequestRanking();
                this.dataSnapshot = task.Result;
            }
        });
    }

    private void ActiveThreadRequestRanking()
    {
        this.thread_wait_requestRanking = true;
    }

    private void InActiveThreadRequestRanking()
    {
        this.thread_wait_requestRanking = false;
    }

    private bool GetThreadRequestRanking()
    {
        return this.thread_wait_requestRanking;
    }
}