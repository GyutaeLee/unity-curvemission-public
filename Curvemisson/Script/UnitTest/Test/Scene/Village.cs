using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace UnitTest.Scene
{
    public class Village : Test
    {
        public override void AwakeInUnitTest()
        {
            SceneManager.LoadScene(Services.Constants.SceneName.Village);
        }

        [UnityTest]
        public IEnumerator WaitingForStart()
        {
            while (this.SceneState == SceneState.Ready)
            {
                yield return null;

                if (SceneManager.GetActiveScene().name == Services.Constants.SceneName.Village)
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

            Services.Scene.Village.Gui gui = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref gui, GameObject.Find("Script"), "Gui", true);
            gui.Test_SetUserDataUI();

            Services.Scene.Village.Village village = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref village, GameObject.Find("Script"), "Village", true);
            village.ClickMoveVillageToRightButton();

            this.SceneState = SceneState.InActive;
        }
    }
}