using UnityEngine;

using Firebase.Database;
using Firebase.Storage;

using Services.Gui;
using System.Threading.Tasks;

namespace Services.Server
{
    public static class Requester
    {
        public static void RequestUserInfoFromFirebaseDB()
        {
            string progressCircleKey = "RequestUserInfoFromFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseDatabase.DefaultInstance.GetReference("security-related" + Manager.Instance.FirebaseUser.UserId).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;

                    if (snapshot.Exists == false)
                    {
                        Sign.SignOutFirebaseAndResetGame();
                        return;
                    }

                    User.User.Instance.SetUserInfoBySnapshotData(snapshot);
                }
            });
        }

        public static void RequestSingleRacingRankingFromFirebaseDB(System.Action<DataSnapshot> action)
        {
            string baseKey = "cvmission_ranking/single_racing";
            string progressCircleKey = "RequestSingleRacingRankingFromFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            Services.Thread.Waiter.InActiveThreadWait();
            FirebaseDatabase.DefaultInstance.GetReference(baseKey).LimitToFirst(Constants.Record.SingleRacingRankingMaxCount).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);
                Thread.Waiter.ActiveThreadWait();

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                }
                else if (task.IsCompleted)
                {
                    action(task.Result);
                }
            });
        }

        public static void RequestOtherUserSingleRacingRecordingFile(string userID, int stageID)
        {
            string progressCircleKey = "RequestOtherUserSingleRacingRecordingFile";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            StorageReference storageReference = FirebaseStorage.DefaultInstance.RootReference;
            StorageReference singleRacingRecordingReference = storageReference.Child(userID + "/" + "security-related" + Static.Replay.GetUserSingleRacingReplayFileName(stageID));
            string recordingFilePath = Static.Replay.GetOtherUserSingleRacingReplayFilePath();

            Services.Thread.Waiter.InActiveThreadWaitServerRequestResult();
            singleRacingRecordingReference.GetFileAsync(recordingFilePath).ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (!task.IsFaulted && !task.IsCanceled)
                {
                    Thread.Waiter.ActiveThreadWaitServerRequestResult(Enum.RequestResult.Server.Success);
                    Debug.Log("File downloaded.");
                }
                else
                {
                    Thread.Waiter.ActiveThreadWaitServerRequestResult(Enum.RequestResult.Server.DownloadRecordingFileFailaure);
                    Debug.Log("File download failure");
                }
            });
        }
    }
}