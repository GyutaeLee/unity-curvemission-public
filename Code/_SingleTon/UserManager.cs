using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Newtonsoft.Json;

public class UserManager : MonoBehaviour
{
    public static UserManager instance = null;

    public class UserInformation
    {
        public struct Infos
        {
            public string email;                                            
            public string nickname;                                         
            public string startDate;                                        
            public bool isBanUser;                                          
            public bool isTestUser;                                         
        }

        public Infos infos;

        public Dictionary<string, int> avatar;                              

        public int coin_1;                                                  
        public Dictionary<string, int> energy_1;                            

        public Dictionary<string, Dictionary<int, bool>> avatarInventory;   

        public Dictionary<string, Dictionary<int, Dictionary<int, bool>>> carInventory;      

        public Dictionary<int, bool> ownedStages;                           
        public Dictionary<int, Dictionary<string, float>> srRecords;        
    }

    private UserInformation info;

    // TO DO : 더 좋은 위치 고민하기
    private string beforeSceneName;

    private void Awake()
    {
        InitInstance();
        this.info = new UserInformation();
    }

    private void Start()
    {
        InitUserManager();
    }

    private void InitInstance()
    {
        if (UserManager.instance == null)
        {
            UserManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void InitUserManager()
    {

    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnLoaded;
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        this.beforeSceneName = scene.name;
    }


    /* Setting User Info Function */

    public void InitUserInfoBySnapshotData(Firebase.Database.DataSnapshot snapshot, string nickname)
    {
        string jsonData;

        instance.info = new UserInformation();

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.avatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.coin_1 = JsonConvert.DeserializeObject<int>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.energy_1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

        instance.info.infos.email = ServerManager.instance.GetFirebasUserEmail();
        instance.info.infos.nickname = nickname;
        instance.info.infos.startDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:s");
        instance.info.infos.isTestUser = false;

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.avatarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, bool>>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.carInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, bool>>>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.ownedStages = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.srRecords = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, float>>>(jsonData);
    }

    public void SetUserInfoBySnapshotData(Firebase.Database.DataSnapshot snapshot)
    {
        string jsonData;

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.avatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.coin_1 = JsonConvert.DeserializeObject<int>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.energy_1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.infos.email = JsonConvert.DeserializeObject<string>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.infos.nickname = JsonConvert.DeserializeObject<string>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.infos.startDate = JsonConvert.DeserializeObject<string>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.infos.isTestUser = JsonConvert.DeserializeObject<bool>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.avatarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, bool>>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.carInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, bool>>>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.ownedStages = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonData);

        jsonData = snapshot.Child("security-related").GetRawJsonValue();
        instance.info.srRecords = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, float>>>(jsonData);
    }

    /* etc */

    public string GetBeforeSceneName()
    {
        return this.beforeSceneName;
    }

    public int GetUserCoin_1()
    {
        return this.info.coin_1;
    }

    public void SetUserCoin_1(int coinQuantity)
    {
        this.info.coin_1 = coinQuantity;
    }

    public void AddUserCoin_1(int coinQuantity)
    {
        this.info.coin_1 += coinQuantity;
    }

    public string GetUserNickname()
    {
        return this.info.infos.nickname;
    }

    public UserInformation.Infos GetUserInfos()
    {
        return this.info.infos;
    }

    public Dictionary<string, int> GetAvatar()
    {
        return this.info.avatar;
    }

    public void SetCarInventory(string inventoryTextKey, int carInfoID, int itemInfoID, bool isOpen)
    {
        this.info.carInventory[inventoryTextKey][carInfoID][itemInfoID] = isOpen;
    }

    public bool GetCarInventory(string inventoryTextKey, int carInfoID, int itemInfoID)
    {
        if (this.info.carInventory.ContainsKey(inventoryTextKey) == false)
        {
            return false;
        }

        if (this.info.carInventory[inventoryTextKey].ContainsKey(carInfoID) == false)
        {
            return false;
        }

        if (this.info.carInventory[inventoryTextKey][carInfoID].ContainsKey(itemInfoID) == false)
        {
            return false;
        }

        return this.info.carInventory[inventoryTextKey][carInfoID][itemInfoID];
    }

    public void SetSRRecords(int stageID, string type, float newRecordValue)
    {
        this.info.srRecords[stageID][type] = newRecordValue;
    }

    public float GetSRRecords(int stageID, string type)
    {
        if (this.info.srRecords.ContainsKey(stageID) == false)
        {
            return 0;
        }

        if (this.info.srRecords[stageID].ContainsKey(type) == false)
        {
            return 0;
        }

        return this.info.srRecords[stageID][type];
    }
}
