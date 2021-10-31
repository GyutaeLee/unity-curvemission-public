using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class RankingStation : AbstractClass
    {
        private GameObject _singleRacingRankingStationCanvas;
        private GameObject singleRacingRankingStationCanvas
        {
            get
            {
                if (this._singleRacingRankingStationCanvas == null)
                {
                    this._singleRacingRankingStationCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "SingleRacingRankingStationCanvas(Clone)", true);
                }
                return this._singleRacingRankingStationCanvas;
            }
        }

        public IEnumerator SceneTest()
        {
            yield return SceneTest(Services.Constants.SceneName.RankingStation);
        }

        public override IEnumerator FullTest()
        {
            yield return Test.Delay(1.0f);

            // 서버 랭킹 데이터 세팅 대기
            yield return WaitForFirebaseRankingData();

            // 1. Single Racing Ranking Station 열기
            yield return ClickSingleRacingRankingStationButton();

            // 1-1. User Record Card 열기
            yield return ClickSingleRacingUserRecordCardButton();

            // 1-2. Total Ranking 열기
            yield return ClickSingleRacingTotalRankingButton();

            // 1-3. Driver Card 모두 열기/닫기
            yield return OpenSingleRacingTotalRankingDriverRecordCardAll();

            // 1-4. Total Ranking 닫기
            yield return ClickSingleRacingTOtalRankingCloseButton();

            // TODO : Map Combobox 기능 생기면 추가 필요
        }

        public void ClickBackButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "BackButton", true);
            button.onClick.Invoke();
        }

        private IEnumerator WaitForFirebaseRankingData()
        {
            Services.Scene.RankingStation.Main main = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref main, this.script, "Main", true);
            while (main.IsSingleRacingRankingInformationSet == false)
            {
                yield return null;
            }
        }

        private IEnumerator ClickSingleRacingRankingStationButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "SingleRacingRankingStationButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickSingleRacingUserRecordCardButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.singleRacingRankingStationCanvas, "UserRecordCardButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickSingleRacingTotalRankingButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.singleRacingRankingStationCanvas, "TotalRankingButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickSingleRacingTOtalRankingCloseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.singleRacingRankingStationCanvas, "BackButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator OpenSingleRacingTotalRankingDriverRecordCardAll()
        {
            GameObject topRankingObject = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.singleRacingRankingStationCanvas, "TopRankingObject", true);
            List<Button> topRankingButtons = new List<Button>();
            for (int i = 0; i < topRankingObject.transform.childCount; i++)
            {
                Button button = null;
                Services.Useful.ObjectFinder.FindComponentInAllChild(ref button, topRankingObject.transform.GetChild(i).gameObject, "RecordCardButton", true);
                topRankingButtons.Add(button);
            }

            GameObject elseRankingObject = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.singleRacingRankingStationCanvas, "ElseRankingObject", true);
            List<Button> elseRankingButtons = new List<Button>();
            for (int i = 0; i < elseRankingObject.transform.childCount; i++)
            {
                Button button = null;
                Services.Useful.ObjectFinder.FindComponentInAllChild(ref button, elseRankingObject.transform.GetChild(i).gameObject, "RecordCardButton", true);
                elseRankingButtons.Add(button);
            }

            GameObject driverRecordCardObject = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.singleRacingRankingStationCanvas, "DriverRecordCardObject", true);
            Button driverRecordCardCloseButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref driverRecordCardCloseButton, driverRecordCardObject, "CloseButton", true);

            for (int i = 0; i < topRankingButtons.Count; i++)
            {
                topRankingButtons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                driverRecordCardCloseButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Ranking Station Test] : " + i + "번째 Top Ranking 버튼 클릭 완료");
            }

            for (int i = 0; i < elseRankingButtons.Count; i++)
            {
                elseRankingButtons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                driverRecordCardCloseButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Ranking Station Test] : " + i + "번째 Else Ranking 버튼 클릭 완료");
            }
        }
    }
}