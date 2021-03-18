using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using Newtonsoft.Json;

using Firebase.Auth;
using Firebase.Database;

public class ServerManager : MonoBehaviour
{
    public static ServerManager instance = null;
    public const int kServerMaxRankingCount = 5;

    public class ServerInformation
    {
        public FirebaseUser firebaseUser;
        public DatabaseReference databaseReference;
    }

    private ServerInformation serverInfo;

    private GameObject POPUP_InternetConnectionCheck;

    private void Awake()
    {
        InitInstance();
        this.serverInfo = new ServerInformation();
    }

    private void Start()
    {
        InitServerManager();
    }

    private void InitInstance()
    {
        if (ServerManager.instance == null)
        {
            ServerManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitServerManager()
    {
        // TO DO : 밴 당한 계정 예외 처리 필요
        if (IsFirebaseLoggedIn() == false)
        {
            ProceedLoginProess();
        }
        else
        {
            ProceedAutoLogin();
            RequestUserInfoFromFirebaseDB();
        }
    }

    private void ProceedLoginProess()
    {
        // TO DO : 팝업 후에 씬 이동하도록 수정하기
        // 로그인이 되어있지 않으므로 로그인해야함
        // 0. 로그인이 되어 있지 않다는 알림을 띄움

        // 1-1. Intro 씬이 아니면 Intro 씬으로 이동 후 로그인 윈도우를 띄움
        // 1-2. Intro 씬이라면 로그인 윈도우만 띄움
        const string introSceneName = "security-related";
        if (SceneManager.GetActiveScene().name != introSceneName)
        {
            SceneManager.LoadScene(introSceneName);
        }
    }

    private void ProceedAutoLogin()
    {
        this.serverInfo.firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
        this.serverInfo.databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    /* On~ Function */

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CheckServerConnection(null, null, null);
    }

    /* Firebase Code */

    public void RefreshFirebase()
    {
        // TO DO : https://firebase.google.com/docs/auth/unity/start?hl=ko 읽고 다시 확인하기
        FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (FirebaseAuth.DefaultInstance.CurrentUser != this.serverInfo.firebaseUser)
        {
            bool isSignedIn = IsFirebaseLoggedIn();

            if (isSignedIn == false && this.serverInfo.firebaseUser != null)
            {
                Debug.Log("Signed out " + this.serverInfo.firebaseUser.UserId);
            }

            this.serverInfo.firebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
            this.serverInfo.databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            if (isSignedIn == true)
            {
                Debug.Log("Signed in " + this.serverInfo.firebaseUser.UserId);
            }
        }
    }

    public bool IsFirebaseLoggedIn()
    {
        bool isLoggedIn = false;

        isLoggedIn = (FirebaseAuth.DefaultInstance.CurrentUser != null);

        return isLoggedIn;
    }

    /* Server Connection */

    public void ActiveServerConnectionCheckPOPUP(bool isEnabled)
    {
        if (this.POPUP_InternetConnectionCheck != null)
        {
            this.POPUP_InternetConnectionCheck.SetActive(isEnabled);
        }
        else
        {
            Debug.Log("ERROR : POPUP_InternetConnectionCheck is null");
        }
    }

    public void CheckServerConnection(string webLink, Action connectionSuccessAction, Action connectionFailAction)
    {
        // TO DO : 게임 중지/다시 시작 로직 변경 필요
        // game pause
        Time.timeScale = 0.0f;

        if (webLink == null || webLink == "")
        {
            Debug.Log("WARNING : NO WEB LINK!");
            webLink = "security-related";
        }

        StartCoroutine(CoroutineCheckServerConnection((isConnected) =>
        {
            if (isConnected == false)
            {
                // 인터넷이 연결되지 않았다는 팝업을 띄움
                Debug.Log("ERROR : NO INTERNET CONNECTION");
                //connectionSuccessAction();
            }
            else
            {
                Debug.Log("SUCCESS : INTERNET CONNECTED");
                //connectionFailAction();
            }

            // TO DO : 게임 중지/다시 시작 로직 변경 필요
            // game resume
            Time.timeScale = 1.0f;
        }, webLink));
    }

    private IEnumerator CoroutineCheckServerConnection(Action<bool> boolAction, string wwwLink)
    {
        UnityWebRequest www = new UnityWebRequest(wwwLink);

        yield return www;

        if (www.error != null)
        {
            Debug.Log("WWW REQUEST ERROR : " + www.error);
            boolAction(false);
        }
        else
        {
            boolAction(true);
        }
    }

    /* Sign In & Out */

    public void SignOutFirebase()
    {
        FirebaseAuth.DefaultInstance.SignOut();
    }

    public void SignOutFirebaseAndResetGame()
    {
        SignOutFirebase();
        SetInitialLocalGameDatas();

        // TO DO : Intro 씬으로 돌아와서 다시 로그인 해야함 + 쓰레드 처리
        SceneManager.LoadScene("security-related");
    }

    public void SetInitialLocalGameDatas()
    {
        // TO DO : 초기 유저값 세팅 추가하기
        SecurityPlayerPrefs.SetInt("security-related", 0);
        SecurityPlayerPrefs.SetInt("security-related", 0);
    }

    /* 쓰기 작업 */

    public void PostUserCoinToFirebaseDB(int addCoinQuantity)
    {
        string baseKey = "security-related/" + this.serverInfo.firebaseUser.UserId;
        string progressCircleKey = "PostUserCoinToFirebaseDB";
        string progressFlagKey = LoadingManager.instance.GetLoadingFlagKey(progressCircleKey);

        LoadingManager.instance.OpenProgressCircle(progressCircleKey);
        LoadingManager.instance.ScheduleCloseProgressCircleInOtheThread(progressCircleKey, progressFlagKey);

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                // close progress circle
                LoadingManager.instance.SetLoadingFlag(progressFlagKey, true);

                string childKey = "security-related";
                DataSnapshot snapshot = task.Result;
                string jsonData;

                // goods
                jsonData = snapshot.Child(childKey).GetRawJsonValue();
                int serverCurrentCoin = JsonConvert.DeserializeObject<int>(jsonData);

                if (serverCurrentCoin != UserManager.instance.GetUserCoin_1())
                {
                    // 서버와 로컬의 코인 개수가 다를 경우, 강제로 서버로 맞춘다.
                    UserManager.instance.SetUserCoin_1(serverCurrentCoin);
                }

                UserManager.instance.AddUserCoin_1(addCoinQuantity);
                this.serverInfo.databaseReference.Child(baseKey + "/" + childKey).SetValueAsync(UserManager.instance.GetUserCoin_1());
            }
        });
    }

    public void PostUserCarInventoryToFirebaseDB(ECarInventoryType eCarInventoryType, int carInfoID, int itemInfoID, bool isOpen)
    {
        string inventoryTextKey = InventoryInformation.GetCarInvetoryTextKey(eCarInventoryType);
        string baseKey = "security-related" + this.serverInfo.firebaseUser.UserId;
        string childKey = "security-related" + inventoryTextKey + "/" + carInfoID + "/" + itemInfoID;

        UserManager.instance.SetCarInventory(inventoryTextKey, carInfoID, itemInfoID, isOpen);

        this.serverInfo.databaseReference.Child(baseKey + "/" + childKey).SetValueAsync(isOpen);
    }

    public void PostUserSPRRecordToFirebaseDB(int stageID, float newRecordTime, int newRecordCar, int newRecordPaint)
    {
        string baseKey = "security-related" + this.serverInfo.firebaseUser.UserId;
        string progressCircleKey = "PostUserSPRRecordToFirebaseDB";
        string progressFlagKey = LoadingManager.instance.GetLoadingFlagKey(progressCircleKey);

        LoadingManager.instance.OpenProgressCircle(progressCircleKey);
        LoadingManager.instance.ScheduleCloseProgressCircleInOtheThread(progressCircleKey, progressFlagKey);

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                // close progress circle
                LoadingManager.instance.SetLoadingFlag(progressFlagKey, true);

                string childKey = "security-related" + stageID;
                DataSnapshot snapshot = task.Result;
                string jsonData;

                // 1. stages
                jsonData = snapshot.Child("security-related" + stageID).GetRawJsonValue();
                bool isStageOpen = JsonConvert.DeserializeObject<bool>(jsonData);

                // 1-1. 맵이 오픈되지 않은 경우
                if (isStageOpen == false)
                {
                    return;
                }

                // 2. records - time
                jsonData = snapshot.Child(childKey + "security-related").GetRawJsonValue();
                float bestRecordTime = JsonConvert.DeserializeObject<float>(jsonData);

                // 2-1. 새로운 기록이 더 느린 경우
                if (bestRecordTime != SPRLapManager.kNoneLapTime && bestRecordTime <= newRecordTime)
                {
                    return;
                }

                // 3. update user records
                UserManager.instance.SetSRRecords(stageID, "security-related", newRecordTime);
                UserManager.instance.SetSRRecords(stageID, "security-related", newRecordCar);
                UserManager.instance.SetSRRecords(stageID, "security-related", newRecordPaint);

                // 4. make child updates dictionary
                Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();

                // time은 string으로 저장해서 float로 받아온다.
                childUpdates[baseKey + "/" + childKey + "security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related").ToString();
                childUpdates[baseKey + "/" + childKey + "security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related");
                childUpdates[baseKey + "/" + childKey + "security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related");

                // 5. Post To Firebase DB
                this.serverInfo.databaseReference.UpdateChildrenAsync(childUpdates);
            }
        });
    }

    /* 랭킹 업데이트 */

    public void CheckAndPostUserSPRRankingToFirebaseDB(int stageID)
    {
        string baseKey = "security-related" + stageID;

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).RunTransaction(mutableData =>
        {
            List<object> SPRRankings = mutableData.Value as List<object>;

            if (CheckAndModifyUserSPRRanking(stageID, ref mutableData, ref SPRRankings) == false)
            {
                return TransactionResult.Abort();
            }                        

            return TransactionResult.Success(mutableData);
        });
    }       

    private bool CheckAndModifyUserSPRRanking(int stageID, ref MutableData mutableData, ref List<object> SPRRankings)
    {
        // TO DO : null로 처음에 한 번씩 들어올때는 어떻게 해야하나?        
        if (SPRRankings == null)
        {
            SPRRankings = new List<object>();
            return true;
        }

        float maxTime = int.MinValue;
        object erasedValue = null;
        bool isOwnRecord = false;

        if (FindErasedValueInSPRRanking(SPRRankings, stageID, ref maxTime, ref erasedValue, ref isOwnRecord) == false)
        {
            return false;
        }

        if (ModifySPRRankingByUserData(stageID, maxTime, isOwnRecord, erasedValue, ref mutableData, ref SPRRankings) == false)
        {
            return false;
        }

        return true;
    }

    private bool FindErasedValueInSPRRanking(List<object> SPRRankings, int stageID, ref float maxTime, ref object erasedValue, ref bool isOwnRecord)
    {
        foreach (var child in SPRRankings)
        {
            if (child == null)
            {
                continue;
            }

            string jsonData = JsonConvert.SerializeObject(child);
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

            jsonData = JsonConvert.SerializeObject(data["security-related"]);
            Dictionary<string, float> records = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonData);
            float rankingTime = records["security-related"];

            // 본인의 랭킹이 존재하는 경우
            if ((string)data["security-related"] == this.serverInfo.firebaseUser.UserId)
            {
                // 본인의 랭킹보다 기록이 좋지 못한 경우에는 return
                if (rankingTime <= UserManager.instance.GetSRRecords(stageID, "security-related"))
                {
                    return false;
                }
                else
                {
                    isOwnRecord = true;
                    erasedValue = child;
                    break;
                }
            }

            // 가장 낮은 기록을 가지고 있는 값(지워져야하는 값) 갱신
            if (rankingTime > maxTime)
            {
                maxTime = rankingTime;
                erasedValue = child;
            }
        }

        return true;
    }

    private bool ModifySPRRankingByUserData(int stageID, float maxTime, bool isOwnRecord, object erasedValue, ref MutableData mutableData, ref List<object> SPRRankings)
    {
        /* 1. 지워야할 기록 지우기 */
        // 랭킹 보드가 최대(kMaxRankingCount)로 채워져 있고 && 내 기록이 랭킹에 없는 경우  
        if (mutableData.ChildrenCount >= ServerManager.kServerMaxRankingCount && isOwnRecord == false)
        {
            // 1-1. 랭킹 보드에 들어갈 수 없는 기록
            // 가장 좋지 않은 기록보다 안 좋은 경우
            if (maxTime <= UserManager.instance.GetSRRecords(stageID, "security-related"))
            {
                return false;
            }
            // 1-2. 랭킹 보드에 들어갈 수 있는 기록
            else
            {
                // "랭킹 보드에서 가장 낮은 순위" 제거
                SPRRankings.Remove(erasedValue);
            }
        }
        // 랭킹에 내 기록이 있는 경우
        else if (isOwnRecord == true)
        {
            SPRRankings.Remove(erasedValue);
        }
        // 랭킹 보드가 최대로 채워지지 않은 경우
        else
        {

        }

        /* 2. 랭킹 보드에 기록을 추가 */
        Dictionary<string, object> newRanking = GetUserSPRRankingData(stageID);

        SPRRankings.Add(newRanking);
        mutableData.Value = SPRRankings;

        return true;
    }

    private Dictionary<string, object> GetUserSPRRankingData(int stageID)
    {
        Dictionary<string, object> ranking = new Dictionary<string, object>();

        ranking["security-related"] = UserManager.instance.GetAvatar();
        ranking["security-related"] = UserManager.instance.GetUserNickname();
        ranking["security-related"] = this.serverInfo.firebaseUser.UserId;

        // records
        Dictionary<string, System.Object> srRecord = new Dictionary<string, System.Object>();
        srRecord["security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related").ToString();
        srRecord["security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related");
        srRecord["security-related"] = UserManager.instance.GetSRRecords(stageID, "security-related");
        ranking["security-related"] = srRecord;

        return ranking;
    }

    public void PostUserBaseInfoToFirebaseDB(string nickname)
    {
        string baseKey = "security-related";

        FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // Snapshot 데이터로부터 유저의 기본 데이터를 초기화한다.
                UserManager.instance.InitUserInfoBySnapshotData(snapshot, nickname);

                // Push user base data to Firebase Database
                PushUserBaseDataToFirebaseDB(snapshot);

                // Push user infos data to Firebase Database
                PushUserInfosDataToFirebaseDB(snapshot);
            }
        });
    }

    private void PushUserBaseDataToFirebaseDB(DataSnapshot snapshot)
    {
        string jsonData = snapshot.GetRawJsonValue();
        this.serverInfo.databaseReference.Child("security-related" + this.serverInfo.firebaseUser.UserId).SetRawJsonValueAsync(jsonData);
    }

    private void PushUserInfosDataToFirebaseDB(DataSnapshot snapshot)
    {
        string jsonData;

        jsonData = JsonConvert.SerializeObject(UserManager.instance.GetUserInfos());
        jsonData = jsonData.Replace("security-related", "security-related");
        jsonData = jsonData.Replace("security-related", "security-related");
        this.serverInfo.databaseReference.Child("security-related" + this.serverInfo.firebaseUser.UserId + "security-related").SetRawJsonValueAsync(jsonData);
    }

    private void RequestUserInfoFromFirebaseDB()
    {
        string progressCircleKey = "RequestUserInfoFromFirebaseDB";
        string progressFlagKey = LoadingManager.instance.GetLoadingFlagKey(progressCircleKey);

        LoadingManager.instance.OpenProgressCircle(progressCircleKey);
        LoadingManager.instance.ScheduleCloseProgressCircleInOtheThread(progressCircleKey, progressFlagKey);

        FirebaseDatabase.DefaultInstance.GetReference("security-related" + this.serverInfo.firebaseUser.UserId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("ERROR : DB not found");
            }
            else if (task.IsCompleted)
            {
                // close progress circle
                LoadingManager.instance.SetLoadingFlag(progressFlagKey, true);

                DataSnapshot snapshot = task.Result;

                if (snapshot.Exists == false)
                {
                    SignOutFirebaseAndResetGame();                    
                    return;
                }    

                UserManager.instance.SetUserInfoBySnapshotData(snapshot);
            }
        });
    }

    /* etc */

    public string GetFirebasUserEmail()
    {
        return this.serverInfo.firebaseUser.Email;
    }
}
