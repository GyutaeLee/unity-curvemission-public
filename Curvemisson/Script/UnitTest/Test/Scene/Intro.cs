using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnitTest.Scene
{
    public class Intro : Test
    {
        public override void AwakeInUnitTest()
        {
            SceneManager.LoadScene(Services.Constants.SceneName.Logo);
        }

        [UnityTest]
        public IEnumerator WaitingForStart()
        {
            while (this.SceneState == SceneState.Ready)
            {
                yield return null;

                if (SceneManager.GetActiveScene().name == Services.Constants.SceneName.Intro)
                {
                    this.SceneState = SceneState.Active;
                }
            }

            Start();
        }

        private void Start()
        {
            if (Services.Server.Manager.Instance.IsFirebaseSignedIn() == false)
            {
                Debug.LogError("로그인 후 테스트를 다시 진행해주세요.");
                return;
            }

            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, GameObject.Find("Canvas"), "GameStartButton", true);
            button.onClick.Invoke();

            this.SceneState = SceneState.InActive;
        }
    }
}
