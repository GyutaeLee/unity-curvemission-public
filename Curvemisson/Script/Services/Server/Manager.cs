using System;
using System.Collections;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

using Firebase.Auth;
using Firebase.Database;

namespace Services.Server
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        public FirebaseUser FirebaseUser { get; set; }
        public DatabaseReference DatabaseReference { get; set; }

        private void Awake()
        {
            if (Manager.Instance == null)
            {
                Manager.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            // TODO : 밴 당한 계정 예외 처리 필요
            if (IsFirebaseSignedIn() == false)
            {
                // TODO : 팝업 후에 씬 이동하도록 수정하기
                // 로그인이 되어있지 않으므로 로그인해야함
                // 0. 로그인이 되어 있지 않다는 알림을 띄움

                // 1-1. Intro 씬이 아니면 Intro 씬으로 이동 후 로그인 윈도우를 띄움
                // 1-2. Intro 씬이라면 로그인 윈도우만 띄움
                if (SceneManager.GetActiveScene().name != Constants.SceneName.Intro)
                {
                    SceneManager.LoadScene(Constants.SceneName.Intro);
                }
            }
            else
            {
                this.FirebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                this.DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                Requester.RequestUserInfoFromFirebaseDB();
            }
        }

        public bool IsFirebaseSignedIn()
        {
            bool isLoggedIn = false;

            isLoggedIn = (FirebaseAuth.DefaultInstance.CurrentUser != null);

            return isLoggedIn;
        }

        public string GetFirebasUserEmail()
        {
            return this.FirebaseUser.Email;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            CheckNetworkConnection(null, null, null);
        }

        public void CheckNetworkConnection(string webLink, Action connectionSuccessAction, Action connectionFailAction)
        {
            // TODO : 게임 중지/다시 시작 로직 변경 필요
            Time.timeScale = 0.0f;

            if (webLink == null || webLink == "")
            {
                webLink = "http://google.com";
            }

            StartCoroutine(CoroutineCheckNetworkConnection((isConnected) =>
            {
                if (isConnected == false)
                {
                    // TODO : 인터넷이 연결되지 않았다는 팝업을 띄움
                    //connectionFailAction();
                }
                else
                {
                    //connectionSuccessAction();                
                }

                // TODO : 게임 중지/다시 시작 로직 변경 필요
                Time.timeScale = 1.0f;
            }, webLink));
        }

        private IEnumerator CoroutineCheckNetworkConnection(Action<bool> boolAction, string wwwLink)
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

        public void RefreshFirebase()
        {
            // TODO : https://firebase.google.com/docs/auth/unity/start?hl=ko 읽고 다시 확인하기
            FirebaseAuth.DefaultInstance.StateChanged += AuthStateChanged;
        }

        private void AuthStateChanged(object sender, System.EventArgs eventArgs)
        {
            if (FirebaseAuth.DefaultInstance.CurrentUser != this.FirebaseUser)
            {
                bool isSignedIn = (FirebaseAuth.DefaultInstance.CurrentUser != null);

                if (isSignedIn == false && this.FirebaseUser != null)
                {
                    Debug.Log("Signed out " + this.FirebaseUser.UserId);
                }

                this.FirebaseUser = FirebaseAuth.DefaultInstance.CurrentUser;
                this.DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                if (isSignedIn == true)
                {
                    Debug.Log("Signed in " + this.FirebaseUser.UserId);
                }
            }
        }

        public string GetFirebaseDBUserBaseKey()
        {
            return "security-related" + this.FirebaseUser.UserId;
        }
    }
}