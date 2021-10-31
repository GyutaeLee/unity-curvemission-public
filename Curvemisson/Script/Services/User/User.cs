using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using Newtonsoft.Json;

using Services.Item.Vehicle;
using Services.Item.Avatar;
using Services.Enum.Inventory;

namespace Services.User
{
    public class User : MonoBehaviour
    {
        public static User Instance { get; private set; }

        public class UserData
        {
            public string Email;                                                
            public string Nickname;                                             
            public string StartDate;                                            
            public bool IsBanUser;                                              
            public bool IsTestUser;                                             

            public Dictionary<string, int> EquipmentAvatar;                     
            public Dictionary<string, System.Object> EquipmentCar;              

            public int Coin_1;                                                  
            public Dictionary<string, int> Energy_1;                            

            public Dictionary<string, Dictionary<int, bool>> AvatarInventory;   

            public Dictionary<string, Dictionary<int, Dictionary<int, bool>>> CarInventory;      

            public Dictionary<int, bool> OwnedStages;                                      
            public Dictionary<int, Dictionary<string, float>> SingleRacingRecords;        
        }

        public string BeforeSceneName { get; private set; }

        public int CurrentStageID { get; set; }

        private Vehicle.Car currentCar;
        public Vehicle.Car CurrentCar
        {
            get
            {
                if (this.currentCar == null)
                {
                    SetCurrentCar();
                }
                return this.currentCar;
            }
        }

        private Character.Avatar currentAvatar;
        public Character.Avatar CurrentAvatar
        {
            get
            {
                if (this.currentAvatar == null)
                {
                    SetCurrentAvatar();
                }
                return this.currentAvatar;
            }
        }

        private UserData _userData;
        private UserData userData
        {
            get
            {
                if (this._userData == null)
                {
                    this._userData = new UserData();
                }
                return this._userData;
            }
        }

        private void Awake()
        {
            if (User.Instance == null)
            {
                User.Instance = this;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnLoaded;
        }

        private void OnSceneUnLoaded(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name == Constants.SceneName.Loading)
                return;

            this.BeforeSceneName = scene.name;
        }

        private void SetCurrentCar()
        {
            Item.Vehicle.Car car = new Item.Vehicle.Car(System.Convert.ToInt32(this.userData.EquipmentCar["security-related"]));
            Item.Vehicle.Paint paint = new Item.Vehicle.Paint(System.Convert.ToInt32(this.userData.EquipmentCar["security-related"]));

            if (this.currentCar == null)
            {
                this.currentCar = new Vehicle.Car(car, paint, null);
            }
            else
            {
                this.currentCar.Change(car, paint, null);
            }
        }

        private void SetCurrentAvatar()
        {
            Item.Avatar.Head head = new Item.Avatar.Head(System.Convert.ToInt32(this.userData.EquipmentAvatar["security-related"]));
            Item.Avatar.Top top = new Item.Avatar.Top(System.Convert.ToInt32(this.userData.EquipmentAvatar["security-related"]));
            Item.Avatar.Bottom bottom = new Item.Avatar.Bottom(System.Convert.ToInt32(this.userData.EquipmentAvatar["security-related"]));

            if (this.currentAvatar == null)
            {
                this.currentAvatar = new Character.Avatar(head, top, bottom);
            }
            else
            {
                this.currentAvatar.ChangeHead(head);
                this.currentAvatar.ChangeTop(top);
                this.currentAvatar.ChangeBottom(bottom);
            }
        }

        public void InitializeUserInformationBySnapshotData(Firebase.Database.DataSnapshot snapshot, string nickname)
        {
            string jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.EquipmentAvatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.EquipmentCar = JsonConvert.DeserializeObject<Dictionary<string, System.Object>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Coin_1 = JsonConvert.DeserializeObject<int>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Energy_1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            this.userData.Email = Server.Manager.Instance.GetFirebasUserEmail();
            this.userData.Nickname = nickname;
            this.userData.StartDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:s");
            this.userData.IsTestUser = false;

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.AvatarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, bool>>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.CarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, bool>>>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.OwnedStages = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.SingleRacingRecords = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, float>>>(jsonData);
        }

        public void SetUserInfoBySnapshotData(Firebase.Database.DataSnapshot snapshot)
        {
            string jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.EquipmentAvatar = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.EquipmentCar = JsonConvert.DeserializeObject<Dictionary<string, System.Object>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Coin_1 = JsonConvert.DeserializeObject<int>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Energy_1 = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Email = JsonConvert.DeserializeObject<string>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.Nickname = JsonConvert.DeserializeObject<string>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.StartDate = JsonConvert.DeserializeObject<string>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.IsTestUser = JsonConvert.DeserializeObject<bool>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.AvatarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, bool>>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.CarInventory = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, Dictionary<int, bool>>>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.OwnedStages = JsonConvert.DeserializeObject<Dictionary<int, bool>>(jsonData);

            jsonData = snapshot.Child("security-related").GetRawJsonValue();
            this.userData.SingleRacingRecords = JsonConvert.DeserializeObject<Dictionary<int, Dictionary<string, float>>>(jsonData);
        }

        public void SetUserCoin_1(int coinQuantity)
        {
            if (coinQuantity <= 0)
                return;

            this.userData.Coin_1 = coinQuantity;
        }

        public void AddUserCoin_1(int coinQuantity)
        {
            if (coinQuantity <= 0)
                return;

            this.userData.Coin_1 += coinQuantity;
        }

        public void SubtractUserCoin_1(int coinQuantity)
        {
            if (coinQuantity <= 0)
                return;

            this.userData.Coin_1 -= coinQuantity;
        }

        public int GetUserCoin_1()
        {
            return this.userData.Coin_1;
        }

        public bool IsUserHaveEnoughCoin_1(int requiredCoinQuantity)
        {
            return this.userData.Coin_1 >= requiredCoinQuantity;
        }

        public string GetUserNickname()
        {
            return this.userData.Nickname;
        }

        public string GetUserEmail()
        {
            return this.userData.Email;
        }

        public string GetUserStartDate()
        {
            return this.userData.StartDate;
        }

        public bool IsBanUser()
        {
            return this.userData.IsBanUser;
        }

        public bool IsTestUser()
        {
            return this.userData.IsTestUser;
        }

        // TODO : Server로 바로 보내지 말고 모아서 보내도록 로직 수정
        public void SetEquippedCarItemCarInfoID(int carItemCarInfoID)
        {
            Server.Poster.PostUserEquipment(Enum.Inventory.InventoryType.Car, "security-related", carItemCarInfoID);
            this.userData.EquipmentCar["security-related"] = carItemCarInfoID;

            SetCurrentCar();
        }

        public void SetEquippedCarItemPaintInfoID(int carItemPaintInfoID)
        {
            Server.Poster.PostUserEquipment(Enum.Inventory.InventoryType.Car, "security-related", carItemPaintInfoID);
            this.userData.EquipmentCar["security-related"] = carItemPaintInfoID;

            SetCurrentCar();
        }

        // TODO : Server로 바로 보내지 말고 모아서 보내도록 로직 수정
        public void ChangeEquippedAvatarItemHeadInfoID(int avatarItemHeadInfoID)
        {
            Server.Poster.PostUserEquipment(Enum.Inventory.InventoryType.Avatar, "security-related", avatarItemHeadInfoID);
            this.userData.EquipmentAvatar["security-related"] = avatarItemHeadInfoID;

            SetCurrentAvatar();
        }

        public void ChangeEquippedAvatarItemTopInfoID(int avatarItemTopInfoID)
        {
            Server.Poster.PostUserEquipment(Enum.Inventory.InventoryType.Avatar, "security-related", avatarItemTopInfoID);
            this.userData.EquipmentAvatar["security-related"] = avatarItemTopInfoID;

            SetCurrentAvatar();
        }

        public void ChangeEquippedAvatarItemBottomInfoID(int avatarItemBottomInfoID)
        {
            Server.Poster.PostUserEquipment(Enum.Inventory.InventoryType.Avatar, "security-related", avatarItemBottomInfoID);
            this.userData.EquipmentAvatar["security-related"] = avatarItemBottomInfoID;

            SetCurrentAvatar();
        }

        public Dictionary<string, int> GetEquipmentAvatarDictionary()
        {
            return this.userData.EquipmentAvatar;
        }

        public void AddCarItemToCarInventory(string carInventoryTextKey, int carItemCarInfoID, int carItemInfoID)
        {
            if (this.userData.CarInventory == null)
            {
                this.userData.CarInventory = new Dictionary<string, Dictionary<int, Dictionary<int, bool>>>();
            }

            if (this.userData.CarInventory.ContainsKey(carInventoryTextKey) == false)
            {
                this.userData.CarInventory[carInventoryTextKey] = new Dictionary<int, Dictionary<int, bool>>();
            }

            if (this.userData.CarInventory[carInventoryTextKey].ContainsKey(carItemCarInfoID) == false)
            {
                this.userData.CarInventory[carInventoryTextKey][carItemCarInfoID] = new Dictionary<int, bool>();
            }

            this.userData.CarInventory[carInventoryTextKey][carItemCarInfoID][carItemInfoID] = true;
        }

        public void AddAvatarItemToAvatarInventory(string avatarInventoryTextKey, int avatarInfoID)
        {
            if (this.userData.AvatarInventory == null)
            {
                this.userData.AvatarInventory = new Dictionary<string, Dictionary<int, bool>>();
            }

            if (this.userData.AvatarInventory.ContainsKey(avatarInventoryTextKey) == false)
            {
                this.userData.AvatarInventory[avatarInventoryTextKey] = new Dictionary<int, bool>();
            }

            this.userData.AvatarInventory[avatarInventoryTextKey][avatarInfoID] = true;
        }

        public bool IsOwnedCarItemInCarInventory(Item.Vehicle.Car car, CarItem carItem)
        {
            CarInventoryType carInventoryType = Static.CarItem.ConvertCarItemTypeToCarInventoryType(carItem.Type);
            string carInventoryTextKey = Static.CarInventory.GetCarInvetoryTypeTextKey(carInventoryType);

            if (this.userData.CarInventory.ContainsKey(carInventoryTextKey) == false)
                return false;

            if (this.userData.CarInventory[carInventoryTextKey].ContainsKey(car.InfoID) == false)
                return false;

            int carItemInfoID = (carInventoryType == CarInventoryType.Car) ? Constants.CarItem.DefaultCarItemPaintInfoID : carItem.InfoID;
            if (this.userData.CarInventory[carInventoryTextKey][car.InfoID].ContainsKey(carItemInfoID) == false)
                return false;

            return this.userData.CarInventory[carInventoryTextKey][car.InfoID][carItemInfoID];
        }

        public bool IsOwnedCarItemInCarInventory(string carInventoryTextKey, int carItemCarInfoID, int carItemInfoID)
        {
            if (this.userData.CarInventory.ContainsKey(carInventoryTextKey) == false)
                return false;

            if (this.userData.CarInventory[carInventoryTextKey].ContainsKey(carItemCarInfoID) == false)
                return false;

            if (this.userData.CarInventory[carInventoryTextKey][carItemCarInfoID].ContainsKey(carItemInfoID) == false)
                return false;

            return this.userData.CarInventory[carInventoryTextKey][carItemCarInfoID][carItemInfoID];
        }

        public bool IsOwnedAvatarItemInAvatarInventory(AvatarItem avatarItem)
        {
            AvatarInventoryType avatarInventoryType = Static.AvatarItem.ConvertAvatarItemTypeToAvatarInventoryType(avatarItem.Type);
            string avatarInventoryTextKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(avatarInventoryType);

            if (this.userData.AvatarInventory.ContainsKey(avatarInventoryTextKey) == false)
                return false;

            if (this.userData.AvatarInventory[avatarInventoryTextKey].ContainsKey(avatarItem.InfoID) == false)
                return false;

            return this.userData.AvatarInventory[avatarInventoryTextKey][avatarItem.InfoID];
        }

        public bool IsOwnedAvatarItemInAvatarInventory(string avatarInventoryTextKey, int avatarItemInfoID)
        {
            if (this.userData.AvatarInventory.ContainsKey(avatarInventoryTextKey) == false)
                return false;

            if (this.userData.AvatarInventory[avatarInventoryTextKey].ContainsKey(avatarItemInfoID) == false)
                return false;

            return this.userData.AvatarInventory[avatarInventoryTextKey][avatarItemInfoID];
        }

        public void SetSingleRacingRecord(int stageID, float newRecordTime, Vehicle.Car car)
        {
            SetSingleRacingRecords(stageID, "security-related", newRecordTime);
            SetSingleRacingRecords(stageID, "security-related", car.CarInfoID);
            SetSingleRacingRecords(stageID, "security-related", car.PaintInfoID);
        }

        private void SetSingleRacingRecords(int stageID, string type, float newRecordValue)
        {
            this.userData.SingleRacingRecords[stageID][type] = newRecordValue;
        }

        public float GetSingleRacingRecords(int stageID, string type)
        {
            if (this.userData.SingleRacingRecords.ContainsKey(stageID) == false)
            {
                return Constants.Record.NoneValue;
            }

            if (this.userData.SingleRacingRecords[stageID].ContainsKey(type) == false)
            {
                return Constants.Record.NoneValue;
            }

            return this.userData.SingleRacingRecords[stageID][type];
        }
    }
}