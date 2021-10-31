using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{
    public class Garage : AbstractClass
    {
        private GameObject _carItemGarageCanvas;
        private GameObject carItemGarageCanvas
        {
            get
            {
                if (this._carItemGarageCanvas == null)
                {
                    this._carItemGarageCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "CarItemGarageCanvas(Clone)", true);
                }
                return this._carItemGarageCanvas;
            }
        }

        private GameObject _avatarItemClosetCanvas;
        private GameObject avatarItemClosetCanvas
        {
            get
            {
                if (this._avatarItemClosetCanvas == null)
                {
                    this._avatarItemClosetCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "AvatarItemClosetCanvas(Clone)", true);
                }
                return this._avatarItemClosetCanvas;
            }
        }

        public IEnumerator SceneTest()
        {
            yield return SceneTest(Services.Constants.SceneName.Garage);
        }

        public override IEnumerator FullTest()
        {
            yield return Test.Delay(1.0f);

            // 1. Car Item Garage 열기
            yield return ClickCarItemGarageButton();

            // 1-1. Car Item Car 교체
            yield return ClickCarItemGarageCarButton();
            yield return ChangeCarItemCars();

            // 1-2. Car Item Paint 교체
            yield return ClickCarItemGaragePaintButton();
            yield return ChangeCarItemPaints();

            // 1-3. Car Item Garage 닫기
            yield return ClickCarItemGarageCloseButton();

            // 2. Avatar Item Closet 열기
            yield return ClickAvatarItemClosetButton();

            // 2-1. Avatar Item Head/Top/Bottom 세트로 하나씩 입고/벗기
            yield return WearAvatarItemAll();

            // 2-2. Avatar Item Closet 닫기
            yield return ClickAvatarItemClosetCloseButton();
        }

        public void ClickBackButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "BackButton", true);
            button.onClick.Invoke();
        }

        private IEnumerator ClickCarItemGarageButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "CarItemGarageButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemGarageCloseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemGarageCanvas, "CloseButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemGarageCarButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemGarageCanvas, "CarButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemGaragePaintButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemGarageCanvas, "PaintButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }
        private IEnumerator ClickCarItemGaragePartsButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemGarageCanvas, "PartsButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ChangeCarItemCars()
        {
            GameObject carObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.carItemGarageCanvas, "CarObjects", true);
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < carObjects.transform.childCount; i++)
            {
                buttons.Add(carObjects.transform.GetChild(i).GetComponent<Button>());
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Garage Test] : " + i + "번째 Car Item Cars 버튼 클릭 완료");
            }
        }

        private IEnumerator ChangeCarItemPaints()
        {
            GameObject paintObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.carItemGarageCanvas, "PaintObjects", true);
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < paintObjects.transform.childCount; i++)
            {
                buttons.Add(paintObjects.transform.GetChild(i).GetComponent<Button>());
            }

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Garage Test] : " + i + "번째 Car Item Paints 버튼 클릭 완료");
            }
        }

        private IEnumerator ClickAvatarItemClosetButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "AvatarItemClosetButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickAvatarItemClosetCloseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.avatarItemClosetCanvas, "CloseButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator WearAvatarItemAll()
        {
            GameObject headObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemClosetCanvas, "HeadObjects", true);
            List<Button> headButtons = new List<Button>();
            for (int i = 0; i < headObjects.transform.childCount; i++)
            {
                headButtons.Add(headObjects.transform.GetChild(i).GetComponent<Button>());
            }

            GameObject topObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemClosetCanvas, "TopObjects", true);
            List<Button> topButtons = new List<Button>();
            for (int i = 0; i < topObjects.transform.childCount; i++)
            {
                topButtons.Add(topObjects.transform.GetChild(i).GetComponent<Button>());
            }

            GameObject bottomObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemClosetCanvas, "BottomObjects", true);
            List<Button> bottomButtons = new List<Button>();
            for (int i = 0; i < bottomObjects.transform.childCount; i++)
            {
                bottomButtons.Add(bottomObjects.transform.GetChild(i).GetComponent<Button>());
            }

            Button headButton = null, topButton = null, bottomButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref headButton, this.avatarItemClosetCanvas, "HeadButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref topButton, this.avatarItemClosetCanvas, "TopButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref bottomButton, this.avatarItemClosetCanvas, "BottomButton", true);

            Services.Scene.Garage.AvatarItemCloset avatarItemCloset = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref avatarItemCloset, this.script, "AvatarItemCloset", true);

            for (int i = 0; i < headButtons.Count; i++)
            {
                headButton.onClick.Invoke();
                headButtons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                topButton.onClick.Invoke();
                topButtons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                bottomButton.onClick.Invoke();
                bottomButtons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Garage Test] : " + i + "번째 Avatar Item 버튼 클릭 완료");
            }
        }
    }
}