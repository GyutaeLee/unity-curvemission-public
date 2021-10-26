using System;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Firebase.Database;

using Services.Item.Avatar;
using Services.Delegate;
using Services.Gui;

using Services.Enum.Inventory;
using Services.Enum.Shop;

namespace Services.Server
{
    public static class Poster
    {
        public static void PostUserAddCoinToFirebaseDB(int addCoinQuantity)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string progressCircleKey = "PostUserCoinToFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                }
                else if (task.IsCompleted)
                {
                    string childKey = "security-related";
                    DataSnapshot snapshot = task.Result;
                    string jsonData;

                    jsonData = snapshot.Child(childKey).GetRawJsonValue();
                    int serverCurrentCoin = JsonConvert.DeserializeObject<int>(jsonData);

                    if (serverCurrentCoin != User.User.Instance.GetUserCoin_1())
                    {
                        User.User.Instance.SetUserCoin_1(serverCurrentCoin);
                    }

                    User.User.Instance.AddUserCoin_1(addCoinQuantity);
                    Manager.Instance.DatabaseReference.Child(baseKey + "/" + childKey).SetValueAsync(User.User.Instance.GetUserCoin_1());
                }
            });
        }

        public static void PostPurchaseCarItemToFirebaseDB(CarInventoryType carInventoryType, int addCoinQuantity, int carItemCarInfoID, int carItemInfoID, delegatePurchaseResult delegatePR)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string progressCircleKey = "PostPurchaseCarItemToFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                    delegatePR(PurchaseResultType.Fail);
                }
                else if (task.IsCompleted)
                {
                    string goodsCoinChildKey = "security-related";
                    DataSnapshot snapshot = task.Result;
                    string jsonData = snapshot.Child(goodsCoinChildKey).GetRawJsonValue();

                    int currentServerCoin = JsonConvert.DeserializeObject<int>(jsonData);
                    if (currentServerCoin != User.User.Instance.GetUserCoin_1())
                    {
                        User.User.Instance.SetUserCoin_1(currentServerCoin);

                        if (User.User.Instance.GetUserCoin_1() < addCoinQuantity)
                        {
                            delegatePR(PurchaseResultType.Fail);
                            return;
                        }
                    }

                    // TODO : 구매시 코인도 함께 업데이트하도록 수정
                    User.User.Instance.SubtractUserCoin_1(addCoinQuantity);
                    Manager.Instance.DatabaseReference.Child(baseKey + "/" + goodsCoinChildKey).SetValueAsync(User.User.Instance.GetUserCoin_1());

                    if (carInventoryType == CarInventoryType.Car)
                    {
                        PushUserCarInventoryToFirebaseDB(CarInventoryType.Paint, carItemCarInfoID, Constants.CarItem.DefaultCarItemPaintInfoID);
                    }
                    PushUserCarInventoryToFirebaseDB(carInventoryType, carItemCarInfoID, carItemInfoID);
                    delegatePR(PurchaseResultType.Success);
                }
            });
        }

        public static void PushUserCarInventoryToFirebaseDB(CarInventoryType carInventoryType, int carItemCarInfoID, int carItemInfoID)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string carInventoryTextKey = Static.CarInventory.GetCarInvetoryTypeTextKey(carInventoryType);
            string childKey = "security-related" + carInventoryTextKey + "/" + carItemCarInfoID + "/" + carItemInfoID;

            User.User.Instance.AddCarItemToCarInventory(carInventoryTextKey, carItemCarInfoID, carItemInfoID);

            Manager.Instance.DatabaseReference.Child(baseKey + "/" + childKey).SetValueAsync(true);
        }

        public static void PostPurchaseAvatarItemToFirebaseDB(List<AvatarItem> avatarItemInfoList, delegatePurchaseResult delegatePR)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string progressCircleKey = "PostPurchaseAvatarItemToFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                    delegatePR(PurchaseResultType.Fail);
                }
                else if (task.IsCompleted)
                {
                    string goodsCoinChildKey = "security-related";
                    DataSnapshot snapshot = task.Result;
                    string jsonData = snapshot.Child(goodsCoinChildKey).GetRawJsonValue();


                    int avatarItemPriceSum = 0;
                    for (int i = 0; i < avatarItemInfoList.Count; i++)
                    {
                        avatarItemPriceSum += avatarItemInfoList[i].Price;
                    }

                    int currentServerCoin = JsonConvert.DeserializeObject<int>(jsonData);
                    if (currentServerCoin != User.User.Instance.GetUserCoin_1())
                    {
                        User.User.Instance.SetUserCoin_1(currentServerCoin);

                        if (User.User.Instance.IsUserHaveEnoughCoin_1(avatarItemPriceSum) == false)
                        {
                            delegatePR(PurchaseResultType.Fail);
                            return;
                        }
                    }

                    // TODO : 구매시 코인도 함께 업데이트하도록 수정
                    User.User.Instance.SubtractUserCoin_1(avatarItemPriceSum);
                    Manager.Instance.DatabaseReference.Child(baseKey + "/" + goodsCoinChildKey).SetValueAsync(User.User.Instance.GetUserCoin_1());

                    PushUserAvatarInventoryToFirebaseDB(avatarItemInfoList);
                    PushUserAvatarInventoryToLocal(avatarItemInfoList);
                    delegatePR(PurchaseResultType.Success);
                }
            });
        }

        private static void PushUserAvatarInventoryToFirebaseDB(List<AvatarItem> avatarItemInfoList)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string goodsCoinChildKey = "security-related";

            Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
            for (int i = 0; i < avatarItemInfoList.Count; i++)
            {
                AvatarInventoryType avatarInventoryType = Static.AvatarItem.ConvertAvatarItemTypeToAvatarInventoryType(avatarItemInfoList[i].Type);
                string avatarInventoryTextKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(avatarInventoryType);
                string childKey = "security-related/" + avatarInventoryTextKey + "/" + avatarItemInfoList[i].InfoID;

                childUpdates[baseKey + "/" + childKey] = true;
            }

            childUpdates[baseKey + "/" + goodsCoinChildKey] = User.User.Instance.GetUserCoin_1();
            Manager.Instance.DatabaseReference.UpdateChildrenAsync(childUpdates);
        }

        private static void PushUserAvatarInventoryToLocal(List<AvatarItem> avatarItemInfoList)
        {
            for (int i = 0; i < avatarItemInfoList.Count; i++)
            {
                AvatarInventoryType avatarInventoryType = Static.AvatarItem.ConvertAvatarItemTypeToAvatarInventoryType(avatarItemInfoList[i].Type);
                string avatarInventoryTextKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(avatarInventoryType);

                User.User.Instance.AddAvatarItemToAvatarInventory(avatarInventoryTextKey, avatarItemInfoList[i].InfoID);
            }
        }

        public static void PostUserSingleRacingRecordToFirebaseDB(int stageID, float newRecordTime, Vehicle.Car newRecordCar)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string progressCircleKey = "PostUserSingleRacingRecordToFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseDatabase.DefaultInstance.GetReference(baseKey).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                }
                else if (task.IsCompleted)
                {
                    string childKey = "security-related/" + stageID;
                    DataSnapshot snapshot = task.Result;
                    string jsonData;

                    jsonData = snapshot.Child("security-related/" + stageID).GetRawJsonValue();
                    bool isStageOpen = JsonConvert.DeserializeObject<bool>(jsonData);

                    if (isStageOpen == false)
                    {
                        return;
                    }

                    jsonData = snapshot.Child(childKey + "/security-related").GetRawJsonValue();
                    float bestRecordTime = JsonConvert.DeserializeObject<float>(jsonData);


                    if (Static.Record.IsValidRecordValue(bestRecordTime) == false || bestRecordTime > newRecordTime)
                    {
                        PushUserSingleRacingRecordToFirebaseDB(baseKey, childKey, newRecordTime, newRecordCar.CarInfoID, newRecordCar.PaintInfoID);
                    }
                }
            });
        }

        private static void PushUserSingleRacingRecordToFirebaseDB(string baseKey, string childKey, float newRecordTime, int newRecordCar, int newRecordPaint)
        {
            Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();

            childUpdates[baseKey + "/" + "/security-related"] = User.User.Instance.CurrentAvatar.Head.InfoID;
            childUpdates[baseKey + "/" + "/security-related"] = User.User.Instance.CurrentAvatar.Top.InfoID;
            childUpdates[baseKey + "/" + "/security-related"] = User.User.Instance.CurrentAvatar.Bottom.InfoID;

            childUpdates[baseKey + "/" + childKey + "/security-related"] = newRecordTime;
            childUpdates[baseKey + "/" + childKey + "/security-related"] = newRecordCar;
            childUpdates[baseKey + "/" + childKey + "/security-related"] = newRecordPaint;

            Manager.Instance.DatabaseReference.UpdateChildrenAsync(childUpdates);
        }

        public static void CheckAndPostUserSingleRacingRankingToFirebaseDB(int stageID, delegateActiveFlag delegateActiveFlag)
        {
            string baseKey = "security-related" + stageID;
            string progressCircleKey = "CheckAndPostUserSingleRacingRankingToFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            FirebaseDatabase.DefaultInstance.GetReference(baseKey).RunTransaction(mutableData =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                List<object> SingleRacingRankings = mutableData.Value as List<object>;
                bool isSuccess = false;
                TransactionResult result;

                isSuccess = CheckAndModifyUserSingleRacingRanking(stageID, ref mutableData, ref SingleRacingRankings);

                if (isSuccess == true)
                {
                    result = TransactionResult.Success(mutableData);
                }
                else
                {
                    result = TransactionResult.Abort();
                }

                if (delegateActiveFlag != null)
                {
                    delegateActiveFlag();
                }

                return result;
            });
        }

        private static bool CheckAndModifyUserSingleRacingRanking(int stageID, ref MutableData mutableData, ref List<object> SingleRacingRankings)
        {
            // TODO : null로 처음에 한 번씩 들어올때는 어떻게 해야하나?        
            if (SingleRacingRankings == null)
            {
                SingleRacingRankings = new List<object>();
                return true;
            }

            float maxTime = int.MinValue;
            object erasedValue = null;
            bool isOwnRecord = false;

            if (FindErasedValueInSingleRacingRanking(SingleRacingRankings, stageID, ref maxTime, ref erasedValue, ref isOwnRecord) == false)
            {
                return false;
            }

            if (ModifySingleRacingRankingByUserData(stageID, maxTime, isOwnRecord, erasedValue, ref mutableData, ref SingleRacingRankings) == false)
            {
                return false;
            }

            return true;
        }

        private static bool FindErasedValueInSingleRacingRanking(List<object> SingleRacingRankings, int stageID, ref float maxTime, ref object erasedValue, ref bool isOwnRecord)
        {
            foreach (var child in SingleRacingRankings)
            {
                if (child == null)
                {
                    continue;
                }

                string jsonData = JsonConvert.SerializeObject(child);
                Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData);

                jsonData = JsonConvert.SerializeObject(data["security-related"]);
                Dictionary<string, float> records = JsonConvert.DeserializeObject<Dictionary<string, float>>(jsonData);
                float recordTime = records["security-related"];

                if (IsUserDataExist((string)data["security-related"]) == true)
                {
                    if (IsLowerThanUserPreviousRecord(stageID, recordTime) == true)
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

                if (recordTime > maxTime)
                {
                    maxTime = recordTime;
                    erasedValue = child;
                }
            }

            return true;
        }

        private static bool IsUserDataExist(string dataUserID)
        {
            if (dataUserID == Manager.Instance.FirebaseUser.UserId)
            {
                return true;
            }

            return false;
        }

        private static bool IsLowerThanUserPreviousRecord(int stageID, float recordTime)
        {
            if (recordTime <= User.User.Instance.GetSingleRacingRecords(stageID, "security-related"))
            {
                return true;
            }

            return false;
        }

        private static bool ModifySingleRacingRankingByUserData(int stageID, float maxTime, bool isOwnRecord, object erasedValue, ref MutableData mutableData, ref List<object> SingleRacingRankings)
        {
            if (ShouldDeleteRecordFromSingleRacingRanking(mutableData.ChildrenCount, isOwnRecord) == true)
            {
                if (IsUserRecordCanEnteredInSingleRacingRanking(stageID, maxTime) == true)
                {
                    SingleRacingRankings.Remove(erasedValue);
                }
                else
                {
                    return false;
                }
            }
            else if (isOwnRecord == true)
            {
                SingleRacingRankings.Remove(erasedValue);
            }

            Dictionary<string, object> newRanking = GetUserSingleRacingRankingData(stageID);

            SingleRacingRankings.Add(newRanking);
            mutableData.Value = SingleRacingRankings;

            return true;
        }

        private static bool ShouldDeleteRecordFromSingleRacingRanking(long rankingCount, bool isOwnRecord)
        {
            if (rankingCount < Constants.Record.SingleRacingRankingMaxCount)
                return false;

            if (isOwnRecord == true)
                return false;

            return true;
        }

        private static bool IsUserRecordCanEnteredInSingleRacingRanking(int stageID, float maxTime)
        {
            if (maxTime <= User.User.Instance.GetSingleRacingRecords(stageID, "security-related"))
            {
                return false;
            }

            return true;
        }

        private static Dictionary<string, object> GetUserSingleRacingRankingData(int stageID)
        {
            Dictionary<string, object> ranking = new Dictionary<string, object>();

            ranking["security-related"] = User.User.Instance.GetEquipmentAvatarDictionary();
            ranking["security-related"] = User.User.Instance.GetUserNickname();
            ranking["security-related"] = Manager.Instance.FirebaseUser.UserId;

            Dictionary<string, System.Object> singleRacingRecord = new Dictionary<string, System.Object>();
            singleRacingRecord["security-related"] = User.User.Instance.GetSingleRacingRecords(stageID, "security-related").ToString();
            singleRacingRecord["security-related"] = User.User.Instance.GetSingleRacingRecords(stageID, "security-related");
            singleRacingRecord["security-related"] = User.User.Instance.GetSingleRacingRecords(stageID, "security-related");
            ranking["security-related"] = singleRacingRecord;

            return ranking;
        }

        public static void PostUserBaseInfoToFirebaseDB(string nickname)
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
                    User.User.Instance.InitializeUserInformationBySnapshotData(snapshot, nickname);

                    PushUserBaseDataToFirebaseDB(snapshot);
                    PushUserInfosDataToFirebaseDB();
                }
            });
        }

        // TODO : Parts 는 함수 따로 만들어야함
        public static void PostUserEquipment(Enum.Inventory.InventoryType inventoryType, string equipmentTextKey, int equipmentInfoID)
        {
            string baseKey = Manager.Instance.GetFirebaseDBUserBaseKey();
            string inventoryKey = Static.Inventory.ConvertInventoryTypeToTextKey(inventoryType);
            string childKey = "security-related/" + inventoryKey + "/" + equipmentTextKey;

            Manager.Instance.DatabaseReference.Child(baseKey + "/" + childKey).SetValueAsync(equipmentInfoID);
        }

        private static void PushUserBaseDataToFirebaseDB(DataSnapshot snapshot)
        {
            string jsonData = snapshot.GetRawJsonValue();
            Manager.Instance.DatabaseReference.Child("security-related" + Manager.Instance.FirebaseUser.UserId).SetRawJsonValueAsync(jsonData);
        }

        private static void PushUserInfosDataToFirebaseDB()
        {
            JObject jsonData = new JObject();

            jsonData.Add("security-related", User.User.Instance.GetUserEmail());
            jsonData.Add("security-related", User.User.Instance.GetUserNickname());
            jsonData.Add("security-related", User.User.Instance.GetUserStartDate());
            jsonData.Add("security-related", User.User.Instance.IsBanUser());
            jsonData.Add("security-related", User.User.Instance.IsTestUser());

            Manager.Instance.DatabaseReference.Child("security-related" + Manager.Instance.FirebaseUser.UserId + "/security-related").SetRawJsonValueAsync(jsonData.ToString());
        }
    }
}