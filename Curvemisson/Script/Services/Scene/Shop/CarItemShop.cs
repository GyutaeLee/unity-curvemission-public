using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Services.Item.Vehicle;
using Services.Vehicle;
using Services.GameText;
using Services.Useful;
using Services.Delegate;

using Services.Enum.Item;
using Services.Enum.GameText;
using Services.Enum.Inventory;
using Services.Enum.Shop;

using Util.Csv.Data;

namespace Services.Scene.Shop
{
    public class CarItemShop : MonoBehaviour
    {
        private class ShopCarItemButton : CarItemUI
        {
            public Button ItemButton { get; set; }
            private GameObject OwnedStateObject;
            private UnityEngine.UI.Text PriceText;

            public ShopCarItemButton(GameObject gameObject) : base(gameObject)
            {
                this.ItemButton = gameObject.GetComponent<Button>();

                ObjectFinder.FindGameObjectInAllChild(ref this.OwnedStateObject, gameObject, "OwnedStateObject", true);
                ObjectFinder.FindComponentInAllChild(ref this.PriceText, gameObject, "PriceText", true);
            }

            public void SetOwned(bool isOwned)
            {
                this.ItemButton.targetGraphic.enabled = !isOwned;
                this.ItemButton.enabled = !isOwned;

                this.OwnedStateObject.SetActive(isOwned);
            }

            public override void SetText()
            {
                this.ItemText.text = Manager.Instance.GetText(this.CarItem.GetTextType(), this.CarItem.InfoID);
                this.PriceText.text = this.CarItem.Price.ToString();
            }
        }

        private class SelectedCarItemUI
        {
            public CarItem CarItem;

            public int SelectedIndex;

            public Image CarImage;
            public Image PaintImage;
            public List<Image> PartsImages;

            public UnityEngine.UI.Text CarNameText;
            public UnityEngine.UI.Text PaintNameText;
            public UnityEngine.UI.Text ItemPriceText;
        }

        private CarItemType _currentCarItemType;
        private CarItemType currentCarItemType
        {
            get
            {
                return this._currentCarItemType;
            }
            set
            {
                if (value <= CarItemType.None || value >= CarItemType.Max)
                {
                    this._currentCarItemType = CarItemType.None;
                }
                else
                {
                    this._currentCarItemType = value;
                }
            }
        }

        private CarUI myCarUI;
        private List<List<ShopCarItemButton>> shopCarItemButtons;
        private SelectedCarItemUI selectedCarItemUI;

        private GameObject carItemShopCanvas;

        private GameObject myCarObject;
        private GameObject shopCarItemButtonObject;

        private GameObject selectedCarItemObject;
        private GameObject selectedCarItemPaintObject;
        private GameObject selectedCarItemPartsObject;
        private GameObject selectedCarItemStatObject;

        private GameObject purchaseAskPopupObject;

        private Button myCarLeftButton;
        private Button myCarRightButton;

        private Button upShopCarItemButton;
        private Button downShopCarItemButton;

        private Button carButton;
        private Button paintButton;
        private Button partsButton;

        private Button selectedPurchaseButton;
        private Button selectedBackButton;

        private Button purchaseAskYesButton;
        private Button purchaseAskNoButton;

        private Button closeButton;

        private UnityEngine.UI.Text userCoinQuantityText;

        public void Open()
        {
            InstantiateCanvas();

            PrpareObjects();
            PrepareButtons();

            Initialize();
        }

        private void Close()
        {
            Destroy(this.carItemShopCanvas);
            Destroy(this);
        }

        private void InstantiateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");

            this.carItemShopCanvas = Resources.Load<GameObject>("Prefab/UI/Canvas/CarItemShopCanvas");
            this.carItemShopCanvas = Instantiate(this.carItemShopCanvas, canvas.transform);
            this.carItemShopCanvas.SetActive(true);
        }

        private void PrpareObjects()
        {
            ObjectFinder.FindGameObjectInAllChild(ref this.myCarObject, this.carItemShopCanvas, "MyCarObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.shopCarItemButtonObject, this.carItemShopCanvas, "ShopCarItemButtonObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.selectedCarItemObject, this.carItemShopCanvas, "SelectedCarItemObject", true);

            ObjectFinder.FindGameObjectInAllChild(ref this.selectedCarItemPaintObject, this.selectedCarItemObject, "SelectedCarItemPaintObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.selectedCarItemPartsObject, this.selectedCarItemObject, "SelectedCarItemPartsObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.selectedCarItemStatObject, this.selectedCarItemObject, "SelectedCarItemStatObject", true);

            this.selectedCarItemUI = new SelectedCarItemUI();
            ObjectFinder.FindComponentInAllChild(ref this.selectedCarItemUI.CarImage, this.selectedCarItemObject, "CarImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.selectedCarItemUI.CarNameText, this.selectedCarItemObject, "CarNameText", true);

            ObjectFinder.FindComponentInAllChild(ref this.selectedCarItemUI.PaintImage, this.selectedCarItemPaintObject, "PaintImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.selectedCarItemUI.PaintNameText, this.selectedCarItemPaintObject, "PaintNameText", true);

            GameObject selectedPurchaseButton = ObjectFinder.GetGameObjectInAllChild(this.selectedCarItemObject, "SelectedPurchaseButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.selectedCarItemUI.ItemPriceText, selectedPurchaseButton, "ItemPriceText", true);

            ObjectFinder.FindComponentInAllChild(ref this.userCoinQuantityText, this.carItemShopCanvas, "UserCoinQuantityText", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.purchaseAskPopupObject, this.selectedCarItemObject, "PurchaseAskPopupObject", true);
        }

        private void PrepareButtons()
        {
            ObjectFinder.FindComponentInAllChild(ref this.myCarLeftButton, this.myCarObject, "MyCarLeftButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.myCarRightButton, this.myCarObject, "MyCarRightButton", true);
            this.myCarLeftButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.myCarRightButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.upShopCarItemButton, this.carItemShopCanvas, "UpShopCarItemButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.downShopCarItemButton, this.carItemShopCanvas, "DownShopCarItemButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.carButton, this.carItemShopCanvas, "CarButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.paintButton, this.carItemShopCanvas, "PaintButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.partsButton, this.carItemShopCanvas, "PartsButton", true);
            this.upShopCarItemButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.downShopCarItemButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.carButton.onClick.AddListener(() => { ChangeShopCarItemButton(CarItemType.Car); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.paintButton.onClick.AddListener(() => { ChangeShopCarItemButton(CarItemType.Paint); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.partsButton.onClick.AddListener(() => { ChangeShopCarItemButton(CarItemType.Parts); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.selectedPurchaseButton, this.selectedCarItemObject, "SelectedPurchaseButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.selectedBackButton, this.selectedCarItemObject, "SelectedBackButton", true);
            this.selectedPurchaseButton.onClick.AddListener(() => { OpenPurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.selectedBackButton.onClick.AddListener(() => { CloseSelectedCarItemUI(); OpenShopCarItemButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.purchaseAskYesButton, this.purchaseAskPopupObject, "PurchaseAskYesButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.purchaseAskNoButton, this.purchaseAskPopupObject, "PurchaseAskNoButton", true);
            this.purchaseAskYesButton.onClick.AddListener(() => { PurchaseSelectedCarItem(); ClosePurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.purchaseAskNoButton.onClick.AddListener(() => { ClosePurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.closeButton, this.carItemShopCanvas, "CloseButton", true);
            this.closeButton.onClick.AddListener(() => { Close(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void Initialize()
        {
            this.currentCarItemType = CarItemType.Car;

            InitializeMyCarUI();
            InitializeShopCarItemButton();

            RefreshUserCoinQunatityText();
        }

        private void InitializeMyCarUI()
        {
            this.myCarUI = new CarUI(User.User.Instance.CurrentCar, this.myCarObject);

            this.myCarUI.SetImage();
            this.myCarUI.SetText();
        }

        private void InitializeShopCarItemButton()
        {
            InitializeShopCarItemButtonData();

            for (int i = 0; i < this.shopCarItemButtons.Count; i++)
            {
                for (int j = 0; j < this.shopCarItemButtons[i].Count; j++)
                {
                    ShopCarItemButton shopCarItemButton = this.shopCarItemButtons[i][j];

                    SetShopCarItemButtonImageAndText(shopCarItemButton);
                    CheckAndInActiveOwnedShopCarItemButton(shopCarItemButton);
                }
            }
        }

        private void InitializeShopCarItemButtonData()
        {
            this.shopCarItemButtons = new List<List<ShopCarItemButton>>();

            for (int i = 0; i < this.shopCarItemButtonObject.transform.childCount; i++)
            {
                Transform transform = this.shopCarItemButtonObject.transform.GetChild(i);
                CarItemType carItemType = (CarItemType)(i + 1);
                List<ShopCarItemButton> shopCarItemButtonList = CreateShopCarItemButtonList(transform, carItemType);

                this.shopCarItemButtons.Add(shopCarItemButtonList);
            }
        }

        private List<ShopCarItemButton> CreateShopCarItemButtonList(Transform transform, CarItemType carItemType)
        {
            List<ShopCarItemButton> shopCarItemButtonList = new List<ShopCarItemButton>();

            int carItemIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Util.Csv.Data.Class.CarItem carItem = Storage<Util.Csv.Data.Class.CarItem>.Instance.GetDataByIndex(carItemIndex);

                if (carItem == null)
                    break;

                while (carItem != null && carItem.Type != (int)carItemType)
                {
                    carItemIndex++;
                    carItem = Storage<Util.Csv.Data.Class.CarItem>.Instance.GetDataByIndex(carItemIndex);
                }

                if (carItem == null)
                    break;

                carItemIndex++;

                GameObject grandson = transform.GetChild(i).gameObject;
                ShopCarItemButton shopCarItemButton = new ShopCarItemButton(grandson);

                CreateShopCarItemButtonCarItem(shopCarItemButton, carItem);
                AddShopCarItemButtonListener(shopCarItemButton, i);

                shopCarItemButtonList.Add(shopCarItemButton);
            }

            return shopCarItemButtonList;
        }

        private void CreateShopCarItemButtonCarItem(ShopCarItemButton shopCarItemButton, Util.Csv.Data.Class.CarItem carItem)
        {
            switch ((CarItemType)carItem.Type)
            {
                case CarItemType.Car:
                    shopCarItemButton.CarItem = new Item.Vehicle.Car(carItem.InfoID, carItem.Price);
                    break;
                case CarItemType.Paint:
                    shopCarItemButton.CarItem = new Item.Vehicle.Paint(carItem.InfoID, carItem.Price);
                    break;
                case CarItemType.Parts:
                    shopCarItemButton.CarItem = new Item.Vehicle.Parts(carItem.InfoID, carItem.Price);
                    break;
                default:
                    break;
            }
        }

        private void AddShopCarItemButtonListener(ShopCarItemButton shopCarItemButton, int index)
        {
            shopCarItemButton.ItemButton.onClick.AddListener(() => { OpenSelectedCarItemUI(index); CloseShopCarItemButton(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void SetShopCarItemButtonImageAndText(ShopCarItemButton shopCarItemButton)
        {
            shopCarItemButton.SetImage();
            shopCarItemButton.SetText();
        }

        private void CheckAndInActiveOwnedShopCarItemButton(ShopCarItemButton shopCarItemButton)
        {
            int carInfoID = (shopCarItemButton.CarItem.Type == CarItemType.Car) ? shopCarItemButton.CarItem.InfoID : this.myCarUI.Car.CarInfoID;
            if (User.User.Instance.IsOwnedCarItemInCarInventory(new Item.Vehicle.Car(carInfoID), shopCarItemButton.CarItem) == true)
            {
                shopCarItemButton.SetOwned(true);
            }
            else
            {
                shopCarItemButton.SetOwned(false);
            }
        }

        private void RefreshUserCoinQunatityText()
        {
            this.userCoinQuantityText.text = User.User.Instance.GetUserCoin_1().ToString();
        }

        private void ChangeShopCarItemButton(CarItemType carItemType)
        {
            if (carItemType == CarItemType.None)
                return;

            if (carItemType == this.currentCarItemType)
                return;

            int selectedIndex = (int)carItemType - 1;
            for (int i = 0; i < this.shopCarItemButtonObject.transform.childCount; i++)
            {
                GameObject shopCarItemButtonObject = this.shopCarItemButtonObject.transform.GetChild(i).gameObject;

                if (i == selectedIndex)
                {
                    shopCarItemButtonObject.SetActive(true);
                }
                else
                {
                    shopCarItemButtonObject.SetActive(false);
                }
            }

            this.currentCarItemType = carItemType;
        }

        private void OpenSelectedCarItemUI(int selectedIndex)
        {
            CarItemType carItemType = this.currentCarItemType;

            if (carItemType == CarItemType.None)
                return;

            SetSelectedCarItemUIData(carItemType, selectedIndex);

            RefreshSelectedCarItemUIImage();
            RefreshSelectedCarItemUIText();

            switch (carItemType)
            {
                case CarItemType.Car:
                    this.selectedCarItemStatObject.SetActive(true);
                    break;
                case CarItemType.Paint:
                    this.selectedCarItemPaintObject.SetActive(true);
                    break;
                case CarItemType.Parts:
                    this.selectedCarItemStatObject.SetActive(true);
                    this.selectedCarItemPartsObject.SetActive(true);
                    break;
                default:
                    break;
            }

            this.selectedCarItemObject.SetActive(true);
        }

        private void CloseSelectedCarItemUI()
        {
            this.selectedCarItemStatObject.SetActive(false);
            this.selectedCarItemPaintObject.SetActive(false);
            this.selectedCarItemPartsObject.SetActive(true);
            this.selectedCarItemObject.SetActive(false);
        }

        private void SetSelectedCarItemUIData(CarItemType carItemType, int selectedIndex)
        {
            ShopCarItemButton shopCarItemButton = this.GetShopCarItemButton(carItemType, selectedIndex);

            this.selectedCarItemUI.CarItem = shopCarItemButton.CarItem;
            this.selectedCarItemUI.SelectedIndex = selectedIndex;
        }

        private void RefreshSelectedCarItemUIImage()
        {
            switch (this.selectedCarItemUI.CarItem.Type)
            {
                case CarItemType.Car:
                    RefreshSelectedItemCarImage();
                    break;
                case CarItemType.Paint:
                    RefreshSelectedItemPaintImage();
                    break;
                case CarItemType.Parts:
                    RefreshSelectedItemPartsImage();
                    break;
                default:
                    break;
            }
        }

        private void RefreshSelectedItemCarImage()
        {
            this.selectedCarItemUI.CarImage.sprite = this.selectedCarItemUI.CarItem.Sprite;
        }

        private void RefreshSelectedItemPaintImage()
        {
            Vehicle.Car car = new Vehicle.Car(new Item.Vehicle.Car(this.myCarUI.Car.CarInfoID), new Item.Vehicle.Paint(this.selectedCarItemUI.CarItem.InfoID));

            this.selectedCarItemUI.CarImage.sprite = car.Sprite;
            this.selectedCarItemUI.PaintImage.sprite = this.selectedCarItemUI.CarItem.Sprite;
        }

        private void RefreshSelectedItemPartsImage()
        {
            // TODO : Parts는 어떻게 할지 고민
        }

        private void RefreshSelectedCarItemUIText()
        {
            if (this.selectedCarItemUI.CarNameText != null)
            {
                switch (this.selectedCarItemUI.CarItem.Type)
                {
                    case CarItemType.Car:
                        this.selectedCarItemUI.CarNameText.text = Manager.Instance.GetText(TextType.CarItemCar, this.selectedCarItemUI.CarItem.InfoID);
                        break;
                    case CarItemType.Paint:
                    case CarItemType.Parts:
                        this.selectedCarItemUI.CarNameText.text = Manager.Instance.GetText(TextType.CarItemCar, this.myCarUI.Car.CarInfoID);
                        break;
                    default:
                        break;
                }
            }

            this.selectedCarItemUI.PaintNameText.text = Manager.Instance.GetText(this.selectedCarItemUI.CarItem.GetTextType(), this.selectedCarItemUI.CarItem.InfoID);
            this.selectedCarItemUI.ItemPriceText.text = "(  " + this.selectedCarItemUI.CarItem.Price.ToString() + ")";
        }

        private void OpenShopCarItemButton()
        {
            this.shopCarItemButtonObject.SetActive(true);
        }

        private void CloseShopCarItemButton()
        {
            this.shopCarItemButtonObject.SetActive(false);
        }

        private void OpenPurchaseAskPopup()
        {
            this.purchaseAskPopupObject.SetActive(true);
            this.purchaseAskPopupObject.transform.Find("DescriptionText").GetComponent<UnityEngine.UI.Text>().text = string.Format(Manager.Instance.GetText(TextType.Shop, (int)Enum.GameText.Shop.Shop_3), this.selectedCarItemUI.CarItem.Price);
        }

        private void ClosePurchaseAskPopup()
        {
            this.purchaseAskPopupObject.SetActive(false);
        }

        private ShopCarItemButton GetShopCarItemButton(CarItemType carItemType, int index)
        {
            return this.shopCarItemButtons[(int)carItemType - 1][index];
        }

#if (UNITY_INCLUDE_TESTS)
        public bool IsPurchaseCarItemResultFlagSet { get; private set; }
#endif
        private void PurchaseSelectedCarItem()
        {
            delegatePurchaseResult delegatePR = new delegatePurchaseResult(Thread.Waiter.ActiveThreadWaitPurchaseResult);
            Thread.Waiter.InActiveThreadWaitPurchaseResult();

            CarInventoryType carInventoryType = Static.CarItem.ConvertCarItemTypeToCarInventoryType(this.currentCarItemType);
            int carInfoID = (this.currentCarItemType == CarItemType.Car) ? this.selectedCarItemUI.CarItem.InfoID : this.myCarUI.Car.CarInfoID;
            PurchaseResultType purchaseResultType = Main.Instance.PurchaseCarItem(carInventoryType, new Item.Vehicle.Car(carInfoID), this.selectedCarItemUI.CarItem, delegatePR);

            if (purchaseResultType == PurchaseResultType.Success)
            {
                StartCoroutine(CoroutineWaitingPurchaseCarItemResult());
            }
            else
            {
                DoPurchaseFailProcess(purchaseResultType);
            }

#if (UNITY_INCLUDE_TESTS)
            this.IsPurchaseCarItemResultFlagSet = true;
            Debug.Log("[Test] : " + this.currentCarItemType + " " + carInfoID + " 구매 결과 - [" + purchaseResultType + "]");
#endif
        }

        private IEnumerator CoroutineWaitingPurchaseCarItemResult()
        {
            delegateGetFlag delegateGetFlag = new delegateGetFlag(Thread.Waiter.GetThreadWaitIsPurchaseResultCompleted);
            yield return StartCoroutine(Thread.Waiter.CoroutineThreadWait(delegateGetFlag));

            if (Thread.Waiter.GetThreadWaitIsPurchaseResultCompleted() == false)
            {
                string errorText = string.Format(Manager.Instance.GetText(TextType.Game, (int)Enum.Error.GameError.ThreadWaitTimeOver));
                Gui.Popup.Manager.Instance.OpenCheckPopup(errorText);
                yield break;
            }

            Gui.Popup.Manager.Instance.OpenCheckPopup(Main.Instance.GetPurchaseResultText(Thread.Waiter.Thread_PurchaseResultType));

            if (Thread.Waiter.Thread_PurchaseResultType == PurchaseResultType.Success)
            {
                DoPurchaseSuccessProcess();
            }
        }

        private void DoPurchaseSuccessProcess()
        {
            ShopCarItemButton selectedshopCarItemButton = this.GetShopCarItemButton(this.currentCarItemType, this.selectedCarItemUI.SelectedIndex);

            selectedshopCarItemButton.SetOwned(true);
            RefreshUserCoinQunatityText();

            CloseSelectedCarItemUI();
            OpenShopCarItemButton();
            ChangeShopCarItemButton(this.currentCarItemType);
        }

        private void DoPurchaseFailProcess(PurchaseResultType purchaseResultType)
        {
            Gui.Popup.Manager.Instance.OpenCheckPopup(Main.Instance.GetPurchaseResultText(purchaseResultType));

            CloseSelectedCarItemUI();
            OpenShopCarItemButton();
            ChangeShopCarItemButton(this.currentCarItemType);
        }
    }
}