using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation
{
    public class Test
    {
        // 테스트 화면이 빨리 지나가지 않도록 딜레이를 주는 플래그
        public static bool IsDelayActive = false;

        [Test]
        public void ActiveDelayMode()
        {
            IsDelayActive = true;
        }

        [UnityTest]
        public IEnumerator IntegrationTest()
        {
            // 1. Logo
            SceneManager.LoadScene(Services.Constants.SceneName.Logo);

            if (IsDelayActive == false)
            {
                SceneManager.LoadScene(Services.Constants.SceneName.Intro);
            }

            // 2. Intro
            yield return WaitingScene(Services.Constants.SceneName.Intro);
            TestAutomation.Scene.Intro intro = new Scene.Intro();
            yield return intro.FullTest();

            // 3. Village
            yield return WaitingScene(Services.Constants.SceneName.Village);
            Delay(0.5f);
            TestAutomation.Scene.Village village = new Scene.Village();
            yield return village.ClickMoveVillageToRightButton();
            village.ClickShopButton();

            // 4. Shop
            yield return WaitingScene(Services.Constants.SceneName.Shop);
            TestAutomation.Scene.Shop shop = new Scene.Shop();
            yield return shop.FullTest();
            shop.ClickBackButton();

            // 5. Village
            yield return WaitingScene(Services.Constants.SceneName.Village);
            yield return village.ClickMoveVillageToLeftButton();
            village.ClickGarageButton();

            // 6. Garage
            yield return WaitingScene(Services.Constants.SceneName.Garage);
            TestAutomation.Scene.Garage garage = new Scene.Garage();
            yield return garage.FullTest();
            garage.ClickBackButton();

            // 7. Village
            yield return WaitingScene(Services.Constants.SceneName.Village);
            yield return village.ClickMoveVillageToRightButton();
            village.ClickRankingStationButton();

            // 8. Ranking Station
            yield return WaitingScene(Services.Constants.SceneName.RankingStation);
            TestAutomation.Scene.RankingStation rankingStation = new Scene.RankingStation();
            yield return rankingStation.FullTest();
            rankingStation.ClickBackButton();

            // 9. Village
            yield return WaitingScene(Services.Constants.SceneName.Village);
            village.ClickStageSelectionButton();

            // 10. Stage Selection
            yield return WaitingScene(Services.Constants.SceneName.StageSelection);
            TestAutomation.Scene.StageSelection stageSelection = new Scene.StageSelection();
            yield return stageSelection.FullTest();

            // 11. Single Racing
            yield return WaitingScene(Services.Constants.SceneName.SingleRacingStage);
            TestAutomation.Scene.SingleRacing singleRacing = new Scene.SingleRacing();
            yield return singleRacing.FullTest();
        }

        [UnityTest]
        public IEnumerator SceneTestIntro()
        {
            TestAutomation.Scene.Intro intro = new Scene.Intro();
            yield return intro.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestVillage()
        {
            TestAutomation.Scene.Village village = new Scene.Village();
            yield return village.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestGarage()
        {
            TestAutomation.Scene.Garage garage = new Scene.Garage();
            yield return garage.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestShop()
        {
            TestAutomation.Scene.Shop shop = new Scene.Shop();
            yield return shop.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestRankingStation()
        {
            TestAutomation.Scene.RankingStation rankingStation = new Scene.RankingStation();
            yield return rankingStation.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestStageSelection()
        {
            TestAutomation.Scene.StageSelection stageSelection = new Scene.StageSelection();
            yield return stageSelection.SceneTest();
        }

        [UnityTest]
        public IEnumerator SceneTestSingleRacing()
        {
            TestAutomation.Scene.SingleRacing singleRacing = new Scene.SingleRacing();
            yield return singleRacing.SceneTest();
        }

        [UnityTest]
        public IEnumerator UnitTestSingleRacingGamePlay()
        {
            TestAutomation.Scene.SingleRacing singleRacing = new Scene.SingleRacing();
            yield return singleRacing.UnitTestPlay();
        }

        [UnityTest]
        public IEnumerator UnitTestSingleRacingGamePlayFastMode()
        {
            TestAutomation.Scene.SingleRacing singleRacing = new Scene.SingleRacing();
            yield return singleRacing.UnitTestPlayFastMode();
        }

        [UnityTest]
        public IEnumerator UnitTestSingleRacingResult()
        {
            TestAutomation.Scene.SingleRacing singleRacing = new Scene.SingleRacing();
            yield return singleRacing.UnitTestResult();
        }

        public static IEnumerator WaitingScene(string sceneName)
        {
            while (SceneManager.GetActiveScene().name != sceneName)
            {
                yield return null;
            }
        }

        public static IEnumerator Delay(float seconds)
        {
            if (Test.IsDelayActive == false)
                yield break;

            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator PreparationForTest()
        {
            SceneManager.LoadScene(Services.Constants.SceneName.Intro);
            yield return Test.WaitingScene(Services.Constants.SceneName.Intro);

            if (Services.Server.Manager.Instance.IsFirebaseSignedIn() == false)
            {
                Debug.LogError("로그인 후 테스트를 다시 진행해주세요.");
                yield break;
            }

            yield return WaitForFirebaseUserData();
        }

        public static IEnumerator WaitForFirebaseUserData()
        {
            string userNickname = Services.User.User.Instance.GetUserNickname();
            while (userNickname == "" || userNickname == null)
            {
                yield return null;
                userNickname = Services.User.User.Instance.GetUserNickname();
            }
        }
    }
}
