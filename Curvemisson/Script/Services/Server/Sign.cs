using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase.Auth;
using Firebase.Extensions;

using Services.Gui;

namespace Services.Server
{
    public static class Sign
    {
        public static void SignUpWithAnonymously(string nickname, System.Action action)
        {
            string progressCircleKey = "SignInWithAnonymously";
            string progressFlagKey = ProgressCircle.Instance.GetProgressFlagKey(progressCircleKey);

            ProgressCircle.Instance.Open(progressCircleKey);
            ProgressCircle.Instance.ScheduleClose(progressCircleKey, progressFlagKey);

            FirebaseAuth.DefaultInstance.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
            {
                ProgressCircle.Instance.CloseScheduled(progressFlagKey);

                if (task.IsCanceled)
                {
                    Debug.LogError("SignInAnonymouslyAsync was canceled.");
                    return;
                }

                if (task.IsFaulted)
                {
                    Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                    return;
                }

                DoSignUpProcess(nickname);
                action();
            });
        }

        public static void SignOutFirebase()
        {
            FirebaseAuth.DefaultInstance.SignOut();
        }

        public static void SignOutFirebaseAndResetGame()
        {
            SignOutFirebase();

            // TODO : Intro 씬으로 돌아와서 다시 로그인 해야함 + 쓰레드 처리
            SceneManager.LoadScene(Constants.SceneName.Intro);
        }

        private static void DoSignUpProcess(string nickname)
        {
            Manager.Instance.RefreshFirebase();
            Poster.PostUserBaseInfoToFirebaseDB(nickname);
        }
    }
}