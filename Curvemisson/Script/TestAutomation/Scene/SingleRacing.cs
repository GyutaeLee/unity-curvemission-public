using System.Collections;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class SingleRacing : AbstractClass
    {
        private IEnumerator Preparation()
        {
            yield return Test.PreparationForTest();

            Services.User.User.Instance.CurrentStageID = 1001;

            SceneManager.LoadScene(Services.Constants.SceneName.SingleRacingPlay);
            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingPlay);
        }

        public IEnumerator SceneTest()
        {
            yield return Preparation();
            yield return FullTest();
        }

        public override IEnumerator FullTest()
        {
            // 1. Pause 버튼 테스트
            yield return PauseCanvasTest();

            // 2. Play 테스트
            if (Test.IsDelayActive == true)
            {
                yield return PlayTest();
            }
            else
            {
                yield return PlayTestFastMode();
            }

            // 3. Result 테스트
            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingResult);
            yield return ResultTest();
        }

        public IEnumerator UnitTestPlay()
        {
            yield return Preparation();
            yield return PlayTest();
        }

        public IEnumerator UnitTestPlayFastMode()
        {
            yield return Preparation();
            yield return PlayTestFastMode();
        }

        public IEnumerator UnitTestResult()
        {
            yield return Preparation();
            yield return PlayTestFastMode();

            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingResult);
            yield return ResultTest();
        }

        private IEnumerator PauseCanvasTest()
        {
            while (Services.Static.Game.IsGameStatePlaying() == false)
            {
                yield return null;
            }

            // 1. Pause 버튼 클릭
            ClickPauseButton();
            GameObject pauseCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "PauseCanvas", true);

            // 2. Resume 버튼 클릭
            Button resumeButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref resumeButton, pauseCanvas, "ResumeButton", true);
            resumeButton.onClick.Invoke();
            yield return Test.Delay(0.5f);

            // 3. Pause 버튼 클릭
            ClickPauseButton();

            // 4. Retry 버튼 클릭 
            Button retryButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref retryButton, pauseCanvas, "RetryButton", true);
            retryButton.onClick.Invoke();
            yield return Test.Delay(0.5f);

            // 5. 게임 시작 대기
            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingPlay);
            while (Services.Static.Game.IsGameStatePlaying() == false)
            {
                yield return null;
            }

            // 6. Pause 버튼 클릭
            ClickPauseButton();

            // 7. Village 버튼 클릭
            pauseCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "PauseCanvas", true);
            Button villageButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref villageButton, pauseCanvas, "VillageButton", true);
            villageButton.onClick.Invoke();

            // 8. Village Scene Load 대기 및 Stage Selection 씬 이동
            yield return Test.WaitingScene(Services.Constants.SceneName.Village);
            yield return Test.Delay(0.5f);
            TestAutomation.Scene.Village village = new Village();
            village.ClickStageSelectionButton();

            // 9. 스테이지 버튼 클릭
            yield return Test.WaitingScene(Services.Constants.SceneName.StageSelection);
            yield return Test.Delay(0.5f);
            TestAutomation.Scene.StageSelection stageSelection = new StageSelection();
            yield return stageSelection.ClickStageButton(1001);

            // 10. 게임 시작 대기
            yield return Test.WaitingScene(Services.Constants.SceneName.SingleRacingPlay);
            while (Services.Static.Game.IsGameStatePlaying() == false)
            {
                yield return null;
            }
        }

        private IEnumerator PlayTest()
        {
            while (Services.Static.Game.IsGameStatePlaying() == false)
            {
                yield return null;
            }

            Services.Vehicle.Input input = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Services.Vehicle.Input>(ref input, GameObject.Find("Game"), "Vehicle", true);

            while (SceneManager.GetActiveScene().name == Services.Constants.SceneName.SingleRacingPlay)
            {
                if (Services.Scene.SingleRacing.Lap.Instance.CurrentLapCount == 1)
                {
                    if (Random.Range(0.0f, 10.0f) >= 7.0f)
                    {
                        input.Test_TouchScreen();
                    }
                }
                else
                {
                    input.Test_TouchScreen();
                }

                yield return null;
            }
        }

        private IEnumerator PlayTestFastMode()
        {
            while (Services.Static.Game.IsGameStatePlaying() == false)
            {
                yield return null;
            }

            Services.Vehicle.Controller controller = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Services.Vehicle.Controller>(ref controller, GameObject.Find("Game"), "Vehicle", true);

            while (SceneManager.GetActiveScene().name == Services.Constants.SceneName.SingleRacingPlay)
            {
                controller.Test_FastAcceleration(20.0f);
                controller.Curve(true);

                yield return null;
            }
        }

        private IEnumerator ResultTest()
        {
            Services.Scene.SingleRacing.Result result = GameObject.Find("SingleRacingResult").GetComponent<Services.Scene.SingleRacing.Result>();
            string finishObjectName = (result.IsBestRecord == true) ? "FinishBest" : "FinishNormal";
            GameObject finishObject = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, finishObjectName, true);

            while (result.IsEffectFinished == false)
            {
                yield return null;
            }

            // 1. 광고 팝업 열기
            Button bonusButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref bonusButton, finishObject, "BonusButton", true);
            bonusButton.onClick.Invoke();
            yield return Test.Delay(0.5f);

            // 1-1. 광고 시청
            // TODO : 광고 보는 로직 추가 필요

            // 1-2. 광고 팝업 닫기
            GameObject advertisementPopup = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "AdvertisementPopup", true);
            Button advertisementPopupCloseButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref advertisementPopupCloseButton, advertisementPopup, "CloseButton", true);
            advertisementPopupCloseButton.onClick.Invoke();
            yield return Test.Delay(0.5f);

            // 2. 맵 선택 / 마을 / 다시하기 버튼 회차 별로 클릭해보기
            // TODO : 구현 필요
        }

        private void ClickPauseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "PauseButton", true);
            button.onClick.Invoke();
        }
    }
}