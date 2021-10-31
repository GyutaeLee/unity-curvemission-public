using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TestAutomation.Scene
{ 
    public class Shop : AbstractClass
    {
        private GameObject _carItemShopCanvas;
        private GameObject carItemShopCanvas
        {
            get
            {
                if (this._carItemShopCanvas == null)
                {
                    this._carItemShopCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "CarItemShopCanvas(Clone)", true);
                }
                return this._carItemShopCanvas;
            }
        }

        private GameObject _avatarItemShopCanvas;
        private GameObject avatarItemShopCanvas
        {
            get
            {
                if (this._avatarItemShopCanvas == null)
                {
                    this._avatarItemShopCanvas = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.canvas, "AvatarItemShopCanvas(Clone)", true);
                }
                return this._avatarItemShopCanvas;
            }
        }

        public IEnumerator SceneTest()
        {
            yield return SceneTest(Services.Constants.SceneName.Shop);
        }

        public override IEnumerator FullTest()
        {
            // 테스트용 코인 체크
            if (Services.User.User.Instance.GetUserCoin_1() < 10000)
            {
                Services.Server.Poster.PostUserAddCoinToFirebaseDB(10000);
            }

            while (Services.User.User.Instance.GetUserCoin_1() < 10000)
            {
                yield return null;
            }

            yield return Test.Delay(1.0f);

            // 1. Car Item Shop 열기
            yield return ClickCarItemShopButton();

            // 1-1. Car Item Car 구매
            yield return ClickCarItemShopCarButton();
            yield return PurchaseCarItemCars();

            // 1-2. Car Item Paint 구매
            yield return ClickCarItemShopPaintButton();
            yield return PurchaseCarItemPaints();

            // 1-3. Car Item Shop 닫기
            yield return ClickCarItemShopCloseButton();

            // 2. Avatar Item Shop 열기
            yield return ClickAvatarItemShopButton();

            // 2-1. Avatar Item All 구매
            yield return PurchaseAvatarItemAll();

            // 2-2. Avatar Item Shop 닫기
            yield return ClickAvatarItemShopCloseButton();
        }

        public void ClickBackButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "BackButton", true);
            button.onClick.Invoke();
        }

        private IEnumerator ClickCarItemShopButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "CarItemShopButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemShopCloseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemShopCanvas, "CloseButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemShopCarButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemShopCanvas, "CarButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemShopPaintButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemShopCanvas, "PaintButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickCarItemShopPartsButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.carItemShopCanvas, "PartsButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator PurchaseCarItemCars()
        {
            GameObject carObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.carItemShopCanvas, "CarObjects", true);
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < carObjects.transform.childCount; i++)
            {
                buttons.Add(carObjects.transform.GetChild(i).GetComponent<Button>());
            }

            Button selectedPurchaseButton = null, purchaseAskYesButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref selectedPurchaseButton, this.carItemShopCanvas, "SelectedPurchaseButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref purchaseAskYesButton, this.carItemShopCanvas, "PurchaseAskYesButton", true);

            Services.Scene.Shop.CarItemShop carItemShop = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref carItemShop, this.script, "CarItemShop", true);

            for (int i = 0; i < buttons.Count; i++)
            { 
                buttons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                selectedPurchaseButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                purchaseAskYesButton.onClick.Invoke();
                yield return Test.Delay(1.0f);

                while (carItemShop.IsPurchaseCarItemResultFlagSet == false)
                {
                    yield return null;
                }

                Services.Gui.Popup.Manager.Instance.CloseCheckPopup();
                yield return Test.Delay(0.5f);

                Debug.Log("[Shop Test] : " + i + "번째 Car Item Cars 버튼 클릭 완료");
            }
        }

        private IEnumerator PurchaseCarItemPaints()
        {
            GameObject paintObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.carItemShopCanvas, "PaintObjects", true);
            List<Button> buttons = new List<Button>();
            for (int i = 0; i < paintObjects.transform.childCount; i++)
            {
                buttons.Add(paintObjects.transform.GetChild(i).GetComponent<Button>());
            }

            Button selectedPurchaseButton = null, purchaseAskYesButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref selectedPurchaseButton, this.carItemShopCanvas, "SelectedPurchaseButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref purchaseAskYesButton, this.carItemShopCanvas, "PurchaseAskYesButton", true);

            Services.Scene.Shop.CarItemShop carItemShop = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref carItemShop, this.script, "CarItemShop", true);

            for (int i = 0; i < buttons.Count; i++)
            {
                buttons[i].onClick.Invoke();
                yield return Test.Delay(0.5f);

                selectedPurchaseButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                purchaseAskYesButton.onClick.Invoke();
                yield return Test.Delay(1.0f);

                while (carItemShop.IsPurchaseCarItemResultFlagSet == false)
                {
                    yield return null;
                }

                Services.Gui.Popup.Manager.Instance.CloseCheckPopup();
                yield return Test.Delay(0.5f);

                Debug.Log("[Shop Test] : " + i + "번째 Car Item Paints 버튼 클릭 완료");
            }
        }

        private IEnumerator ClickAvatarItemShopButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.canvas, "AvatarItemShopButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator ClickAvatarItemShopCloseButton()
        {
            Button button = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild<Button>(ref button, this.avatarItemShopCanvas, "CloseButton", true);
            button.onClick.Invoke();

            yield return Test.Delay(0.5f);
        }

        private IEnumerator PurchaseAvatarItemAll()
        {
            GameObject headObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemShopCanvas, "HeadObjects", true);
            List<Button> headButtons = new List<Button>();
            for (int i = 0; i < headObjects.transform.childCount; i++)
            {
                headButtons.Add(headObjects.transform.GetChild(i).GetComponent<Button>());
            }

            GameObject topObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemShopCanvas, "TopObjects", true);
            List<Button> topButtons = new List<Button>();
            for (int i = 0; i < topObjects.transform.childCount; i++)
            {
                topButtons.Add(topObjects.transform.GetChild(i).GetComponent<Button>());
            }

            GameObject bottomObjects = Services.Useful.ObjectFinder.GetGameObjectInAllChild(this.avatarItemShopCanvas, "BottomObjects", true);
            List<Button> bottomButtons = new List<Button>();
            for (int i = 0; i < bottomObjects.transform.childCount; i++)
            {
                bottomButtons.Add(bottomObjects.transform.GetChild(i).GetComponent<Button>());
            }

            Button headButton = null, topButton = null, bottomButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref headButton, this.avatarItemShopCanvas, "HeadButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref topButton, this.avatarItemShopCanvas, "TopButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref bottomButton, this.avatarItemShopCanvas, "BottomButton", true);

            Button revertButton = null, purchaseAllButton = null, purchaseAskYesButton = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref revertButton, this.avatarItemShopCanvas, "RevertButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref purchaseAllButton, this.avatarItemShopCanvas, "PurchaseAllButton", true);
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref purchaseAskYesButton, this.avatarItemShopCanvas, "PurchaseAskYesButton", true);

            Services.Scene.Shop.AvatarItemShop avatarItemShop = null;
            Services.Useful.ObjectFinder.FindComponentInAllChild(ref avatarItemShop, this.script, "AvatarItemShop", true);

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

                purchaseAllButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                purchaseAskYesButton.onClick.Invoke();
                yield return Test.Delay(1.0f);

                while (avatarItemShop.IsPurchaseAvatarItemResultFlagSet == false)
                {
                    yield return null;
                }

                Services.Gui.Popup.Manager.Instance.CloseCheckPopup();
                yield return Test.Delay(0.5f);

                revertButton.onClick.Invoke();
                yield return Test.Delay(0.5f);

                Debug.Log("[Shop Test] : " + i + "번째 Avatar Item 버튼 클릭 완료");
            }
        }
    }
}