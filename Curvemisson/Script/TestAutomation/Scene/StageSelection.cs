using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class StageSelection : AbstractClass
    {
        public IEnumerator SceneTest()
        {
            yield return SceneTest(Services.Constants.SceneName.StageSelection);
        }

        public override IEnumerator FullTest()
        {
            yield return Test.Delay(1.0f);

            // 1. 1001 스테이지 (발산 마을) 이동
            yield return ClickStageButton(1001);
        }

        public IEnumerator ClickStageButton(int stageID)
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "Stage" + stageID + "Button", true);
            button.onClick.Invoke();

            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingPlay);
        }
    }
}