using UnityEngine;

using Firebase.Database;

using Services.Gui;

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
            string baseKey = "security-related";
            string progressCircleKey = "RequestSingleRacingRankingFromFirebaseDB";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            Services.Thread.Waiter.InActiveThreadWait();
            FirebaseDatabase.DefaultInstance.GetReference(baseKey).LimitToFirst(Constants.Record.SingleRacingRankingMaxCount).GetValueAsync().ContinueWith(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsFaulted)
                {
                    Debug.Log("ERROR : DB not found");
                }
                else if (task.IsCompleted)
                {
                    Thread.Waiter.ActiveThreadWait();
                    action(task.Result);
                }
            });
        }

    }
}