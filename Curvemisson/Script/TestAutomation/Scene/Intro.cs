using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class Intro : AbstractClass
    {
        public IEnumerator SceneTest()
        {
            yield return Test.PreparationForTest();
            yield return FullTest();
        }

        public override IEnumerator FullTest()
        {
            yield return Test.Delay(0.5f);

            if (Services.Server.Manager.Instance.IsFirebaseSignedIn() == false)
            {
                Debug.LogError("로그인 후 테스트를 다시 진행해주세요.");
                yield break;
            }

            yield return Test.WaitForFirebaseUserData();
            yield return ClickGameStartButton();
        }
        
        private IEnumerator ClickGameStartButton()
        {
            yield return Test.Delay(1.0f);

            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "GameStartButton", true);
            button.onClick.Invoke();
        }
    }
}
