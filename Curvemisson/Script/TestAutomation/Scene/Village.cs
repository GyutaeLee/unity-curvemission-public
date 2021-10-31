using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class Village : AbstractClass
    {
        public IEnumerator SceneTest()
        {
            yield return SceneTest(Services.Constants.SceneName.Village);
        }

        public override IEnumerator FullTest()
        {
            yield return Test.Delay(1.0f);

            yield return ClickMoveVillageToRightButton();
            yield return ClickMoveVillageToLeftButton();
        }

        public IEnumerator ClickMoveVillageToRightButton()
        {
            Services.Scene.Village.Village village = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref village, this.script, "Village", true);
            village.ClickMoveVillageToRightButton();

            yield return Test.Delay(1.5f);
        }

        public IEnumerator ClickMoveVillageToLeftButton()
        {
            Services.Scene.Village.Village village = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref village, this.script, "Village", true);
            village.ClickMoveVillageToLeftButton();

            yield return Test.Delay(1.5f);
        }

        public void ClickShopButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "ShopButton", true);
            button.onClick.Invoke();
        }

        public void ClickGarageButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "GarageButton", true);
            button.onClick.Invoke();
        }

        public void ClickRankingStationButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "RankingStationButton", true);
            button.onClick.Invoke();
        }

        public void ClickStageSelectionButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "StageSelectionButton", true);
            button.onClick.Invoke();
        }
    }
}