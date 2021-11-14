using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Services.Item.Avatar;
using Services.Character;
using Services.GameText;
using Services.Delegate;

using Services.Enum.Item;
using Services.Enum.Shop;

using Services.Useful;

using Util.Csv.Data;

namespace Services.Scene.Shop
{
    public class AvatarItemShop : MonoBehaviour
    {
        public class ShopAvatarItemButton : AvatarItemUI
        {
            public Button ItemButton { get; set; }
            private GameObject OwnedStateObject;
            private UnityEngine.UI.Text PriceText;

            public ShopAvatarItemButton(GameObject gameObject) : base(gameObject)
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
                this.ItemText.text = Manager.Instance.GetText(this.AvatarItem.GetTextType(), this.AvatarItem.InfoID);
                this.PriceText.text = this.AvatarItem.Price.ToString();
            }
        }

        private AvatarItemType _currentAvatarItemType;
        private AvatarItemType currentAvatarItemType
        {
            get
            {
                return this._currentAvatarItemType;
            }
            set
            {
                if (value <= AvatarItemType.None || value >= AvatarItemType.Max)
                {
                    this._currentAvatarItemType = AvatarItemType.None;
                }
                else
                {
                    this._currentAvatarItemType = value;
                }
            }
        }

        private AvatarItemUI wornHeadAvatarItemUI;
        private AvatarItemUI wornTopAvatarItemUI;
        private AvatarItemUI wornBottomAvatarItemUI;

        private AvatarUI wornAvatar;

        private ShopAvatarItemButton wornAvatarHeadItemButton;
        private ShopAvatarItemButton wornAvatarTopItemButton;
        private ShopAvatarItemButton wornAvatarBottomItemButton;
        private List<List<ShopAvatarItemButton>> shopAvatarItemButtons;

        private GameObject avatarItemShopCanvas;

        private GameObject myAvatarObject;

        private GameObject shopAvatarItemButtonObject;
        private GameObject purchaseAskPopupObject;

        private Button upShopAvatarItemButton;
        private Button downShopAvatarItemButton;

        private Button headButton;
        private Button topButton;
        private Button bottomButton;

        private Button purchaseAskYesButton;
        private Button purchaseAskNoButton;

        private Button revertButton;
        private Button purchaseAllButton;

        private Button closeButton;

        private UnityEngine.UI.Text userCoinQuantityText;

        public void Open()
        {
            InstantiateCanvas();

            PrepareObjects();
            PrepareButtons();

            Initialize();
        }

        private void Close()
        {
            Destroy(this.avatarItemShopCanvas);
            Destroy(this);
        }

        private void InstantiateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");

            this.avatarItemShopCanvas = Resources.Load<GameObject>("Prefab/UI/Canvas/AvatarItemShopCanvas");
            this.avatarItemShopCanvas = Instantiate(this.avatarItemShopCanvas, canvas.transform);
            this.avatarItemShopCanvas.SetActive(true);
        }

        private void PrepareObjects()
        {
            ObjectFinder.FindGameObjectInAllChild(ref this.myAvatarObject, this.avatarItemShopCanvas, "MyAvatarObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.shopAvatarItemButtonObject, this.avatarItemShopCanvas, "ShopAvatarItemButtonObject", true);
            ObjectFinder.FindGameObjectInAllChild(ref this.purchaseAskPopupObject, this.avatarItemShopCanvas, "PurchaseAskPopupObject", true);

            this.wornHeadAvatarItemUI = new AvatarItemUI(ObjectFinder.GetGameObjectInAllChild(this.myAvatarObject, "WornHeadAvatarItemObject", true));
            this.wornTopAvatarItemUI = new AvatarItemUI(ObjectFinder.GetGameObjectInAllChild(this.myAvatarObject, "WornTopAvatarItemObject", true));
            this.wornBottomAvatarItemUI = new AvatarItemUI(ObjectFinder.GetGameObjectInAllChild(this.myAvatarObject, "WornBottomAvatarItemObject", true));

            this.wornAvatar = new AvatarUI(ObjectFinder.GetGameObjectInAllChild(this.myAvatarObject, "WornAvatarObject", true));

            ObjectFinder.FindComponentInAllChild(ref this.userCoinQuantityText, this.avatarItemShopCanvas, "UserCoinQuantityText", true);
        }

        private void PrepareButtons()
        {
            ObjectFinder.FindComponentInAllChild(ref this.revertButton, this.myAvatarObject, "RevertButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.purchaseAllButton, this.myAvatarObject, "PurchaseAllButton", true);
            this.revertButton.onClick.AddListener(() => { RevertWornAvatar(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.purchaseAllButton.onClick.AddListener(() => { OpenPurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.upShopAvatarItemButton, this.avatarItemShopCanvas, "UpShopAvatarItemButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.downShopAvatarItemButton, this.avatarItemShopCanvas, "DownShopAvatarItemButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.headButton, this.avatarItemShopCanvas, "HeadButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.topButton, this.avatarItemShopCanvas, "TopButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.bottomButton, this.avatarItemShopCanvas, "BottomButton", true);
            this.upShopAvatarItemButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.downShopAvatarItemButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.headButton.onClick.AddListener(() => { ChangeShopAvatarItemButton(AvatarItemType.Head); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.topButton.onClick.AddListener(() => { ChangeShopAvatarItemButton(AvatarItemType.Top); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.bottomButton.onClick.AddListener(() => { ChangeShopAvatarItemButton(AvatarItemType.Bottom); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.purchaseAskYesButton, this.avatarItemShopCanvas, "PurchaseAskYesButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.purchaseAskNoButton, this.avatarItemShopCanvas, "PurchaseAskNoButton", true);
            this.purchaseAskYesButton.onClick.AddListener(() => { PurchaseAllWornAvatarItem(); ClosePurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.purchaseAskNoButton.onClick.AddListener(() => { ClosePurchaseAskPopup(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.closeButton, this.avatarItemShopCanvas, "CloseButton", true);
            this.closeButton.onClick.AddListener(() => { Close(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void Initialize()
        {
            this.currentAvatarItemType = AvatarItemType.Head;

            CreateWornAvatar();
            RevertWornAvatar();
            InitializeShopAvatarItemButton();

            ResetCoinQunatityText();
        }

        private void CreateWornAvatar()
        {
            int headInfoID = User.User.Instance.CurrentAvatar.Head.InfoID;
            int topInfoID = User.User.Instance.CurrentAvatar.Top.InfoID;
            int bottomInfoID = User.User.Instance.CurrentAvatar.Bottom.InfoID;

            this.wornHeadAvatarItemUI.AvatarItem = new Item.Avatar.Head(headInfoID);
            this.wornTopAvatarItemUI.AvatarItem = new Item.Avatar.Top(topInfoID);
            this.wornBottomAvatarItemUI.AvatarItem = new Item.Avatar.Bottom(bottomInfoID);

            this.wornAvatar.CreateAvatar(headInfoID, topInfoID, bottomInfoID);
        }

        private void RevertWornAvatar()
        {
            SetWornHeadAvatarSprite(User.User.Instance.CurrentAvatar.Head.InfoID);
            SetWornTopAvatarSprite(User.User.Instance.CurrentAvatar.Top.InfoID);
            SetWornBottomAvatarSprite(User.User.Instance.CurrentAvatar.Bottom.InfoID);

            this.wornAvatarHeadItemButton = null;
            this.wornAvatarTopItemButton = null;
            this.wornAvatarBottomItemButton = null;
        }

        private void SetWornHeadAvatarSprite(int avatarItemHeadInfoID)
        {
            this.wornHeadAvatarItemUI.AvatarItem = new Item.Avatar.Head(avatarItemHeadInfoID);
            this.wornHeadAvatarItemUI.SetImage();

            this.wornAvatar.ChangeHead(avatarItemHeadInfoID);
        }

        private void SetWornTopAvatarSprite(int avatarItemTopInfoID)
        {
            this.wornTopAvatarItemUI.AvatarItem = new Item.Avatar.Top(avatarItemTopInfoID);
            this.wornTopAvatarItemUI.SetImage();

            this.wornAvatar.ChangeTop(avatarItemTopInfoID);
        }

        private void SetWornBottomAvatarSprite(int avatarItemBottomInfoID)
        {
            this.wornBottomAvatarItemUI.AvatarItem = new Item.Avatar.Bottom(avatarItemBottomInfoID);
            this.wornBottomAvatarItemUI.SetImage();

            this.wornAvatar.ChangeBottom(avatarItemBottomInfoID);
        }

        private void InitializeShopAvatarItemButton()
        {
            InitializeShopAvatarItemButtonData();

            for (int i = 0; i < this.shopAvatarItemButtons.Count; i++)
            {
                for (int j = 0; j < this.shopAvatarItemButtons[i].Count; j++)
                {
                    ShopAvatarItemButton copyShopAvatarItemButton = this.shopAvatarItemButtons[i][j];

                    SetShopAvatarItemButtonImageAndText(copyShopAvatarItemButton);
                    CheckAndInActiveOwnedShopAvatarItemButton(copyShopAvatarItemButton);
                }
            }
        }

        private void InitializeShopAvatarItemButtonData()
        {
            this.shopAvatarItemButtons = new List<List<ShopAvatarItemButton>>();

            for (int i = 0; i < this.shopAvatarItemButtonObject.transform.childCount; i++)
            {
                Transform transform = this.shopAvatarItemButtonObject.transform.GetChild(i);
                AvatarItemType avatarItemType = (AvatarItemType)(i + 1);
                List<ShopAvatarItemButton> shopAvatarItemButtonList = CreateShopAvatarItemButtonList(transform, avatarItemType);

                this.shopAvatarItemButtons.Add(shopAvatarItemButtonList);
            }
        }

        private List<ShopAvatarItemButton> CreateShopAvatarItemButtonList(Transform transform, AvatarItemType avatarItemType)
        {
            List<ShopAvatarItemButton> shopAvatarItemButtonList = new List<ShopAvatarItemButton>();

            int avatarItemIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Util.Csv.Data.Class.AvatarItem avatarItem = Storage<Util.Csv.Data.Class.AvatarItem>.Instance.GetDataByIndex(avatarItemIndex);

                if (avatarItem == null)
                    break;

                while (avatarItem != null && avatarItem.Type != (int)avatarItemType)
                {
                    avatarItemIndex++;
                    avatarItem = Storage<Util.Csv.Data.Class.AvatarItem>.Instance.GetDataByIndex(avatarItemIndex);
                }

                if (avatarItem == null)
                    break;

                avatarItemIndex++;

                GameObject grandson = transform.GetChild(i).gameObject;
                ShopAvatarItemButton shopAvatarItemButton = new ShopAvatarItemButton(grandson);

                CreateShopAvatarItemButtonAvatarItem(shopAvatarItemButton, avatarItem);
                AddShopAvatarItemButtonListener(shopAvatarItemButton);

                shopAvatarItemButtonList.Add(shopAvatarItemButton);
            }

            return shopAvatarItemButtonList;
        }

        private void CreateShopAvatarItemButtonAvatarItem(ShopAvatarItemButton shopAvatarItemButton, Util.Csv.Data.Class.AvatarItem avatarItem)
        {
            switch ((AvatarItemType)avatarItem.Type)
            {
                case AvatarItemType.Head:
                    shopAvatarItemButton.AvatarItem = new Item.Avatar.Head(avatarItem.InfoID, avatarItem.Price);
                    break;
                case AvatarItemType.Top:
                    shopAvatarItemButton.AvatarItem = new Item.Avatar.Top(avatarItem.InfoID, avatarItem.Price);
                    break;
                case AvatarItemType.Bottom:
                    shopAvatarItemButton.AvatarItem = new Item.Avatar.Bottom(avatarItem.InfoID, avatarItem.Price);
                    break;
                default:
                    break;
            }
        }

        private void AddShopAvatarItemButtonListener(ShopAvatarItemButton shopAvatarItemButton)
        {
            shopAvatarItemButton.ItemButton.onClick.AddListener(() => { WearAvatarItemOnWornAvatar(shopAvatarItemButton); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void WearAvatarItemOnWornAvatar(ShopAvatarItemButton shopAvatarItemButton)
        {
            switch (shopAvatarItemButton.AvatarItem.Type)
            {
                case AvatarItemType.Head:
                    this.wornAvatarHeadItemButton = shopAvatarItemButton;
                    SetWornHeadAvatarSprite(shopAvatarItemButton.AvatarItem.InfoID);
                    break;
                case AvatarItemType.Top:
                    this.wornAvatarTopItemButton = shopAvatarItemButton;
                    SetWornTopAvatarSprite(shopAvatarItemButton.AvatarItem.InfoID);
                    break;
                case AvatarItemType.Bottom:
                    this.wornAvatarBottomItemButton = shopAvatarItemButton;
                    SetWornBottomAvatarSprite(shopAvatarItemButton.AvatarItem.InfoID);
                    break;
                default:
                    break;
            }
        }

        private void SetShopAvatarItemButtonImageAndText(ShopAvatarItemButton shopAvatarItemButton)
        {
            shopAvatarItemButton.SetImage();
            shopAvatarItemButton.SetText();
        }

        private void CheckAndInActiveOwnedShopAvatarItemButton(ShopAvatarItemButton shopAvatarItemButton)
        {
            if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(shopAvatarItemButton.AvatarItem) == true)
            {
                shopAvatarItemButton.SetOwned(true);
            }
            else
            {
                shopAvatarItemButton.SetOwned(false);
            }
        }

        private void OpenPurchaseAskPopup()
        {
            int wornAvatarItemPriceSum = GetWornAvatarItemPriceSum();

            this.purchaseAskPopupObject.SetActive(true);
            this.purchaseAskPopupObject.transform.Find("Text").GetComponent<UnityEngine.UI.Text>().text = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Shop, (int)Enum.GameText.Shop.Shop_3), wornAvatarItemPriceSum);
        }

        private void ClosePurchaseAskPopup()
        {
            this.purchaseAskPopupObject.SetActive(false);
        }

        private int GetWornAvatarItemPriceSum()
        {
            int wornAvatarItemPriceSum = 0;

            if (this.wornAvatarHeadItemButton != null)
            {
                wornAvatarItemPriceSum += this.wornAvatarHeadItemButton.AvatarItem.Price;
            }

            if (this.wornAvatarTopItemButton != null)
            {
                wornAvatarItemPriceSum += this.wornAvatarTopItemButton.AvatarItem.Price;
            }

            if (this.wornAvatarBottomItemButton != null)
            {
                wornAvatarItemPriceSum += this.wornAvatarBottomItemButton.AvatarItem.Price;
            }

            return wornAvatarItemPriceSum;
        }

        private void ChangeShopAvatarItemButton(AvatarItemType avatarItemType)
        {
            if (avatarItemType == AvatarItemType.None)
                return;

            if (avatarItemType == this.currentAvatarItemType)
                return;

            SetActiveShopAvatarItemButton(avatarItemType);

            this.currentAvatarItemType = avatarItemType;
        }

        private void SetActiveShopAvatarItemButton(AvatarItemType avatarItemType)
        {
            int seletedIndex = (int)avatarItemType - 1;

            for (int i = 0; i < this.shopAvatarItemButtonObject.transform.childCount; i++)
            {
                GameObject buttonObject = this.shopAvatarItemButtonObject.transform.GetChild(i).gameObject;

                if (i == seletedIndex)
                {
                    buttonObject.SetActive(true);
                }
                else
                {
                    buttonObject.SetActive(false);
                }
            }
        }


#if (UNITY_INCLUDE_TESTS)
        public bool IsPurchaseAvatarItemResultFlagSet { get; private set; }
#endif
        private void PurchaseAllWornAvatarItem()
        {
            delegatePurchaseResult delegatePR = new delegatePurchaseResult(Thread.Waiter.ActiveThreadWaitPurchaseResult);
            Thread.Waiter.InActiveThreadWaitPurchaseResult();

            List<AvatarItem> wornAvatarItemInfoList = GetWornAvatarItemList();
            PurchaseResultType purchaseResultType = Main.Instance.PurchaseAvatarItem(wornAvatarItemInfoList, delegatePR);

            if (purchaseResultType == PurchaseResultType.Success)
            {
                StartCoroutine(CoroutineWaitingPurchaseAvatarItemResult());
            }
            else
            {
                DoPurchaseFailProcess(purchaseResultType);
            }

#if (UNITY_INCLUDE_TESTS)
            this.IsPurchaseAvatarItemResultFlagSet = true;
            Debug.Log("[Test] : " + wornAvatarItemInfoList + " 구매 결과 - [" + purchaseResultType + "]");
#endif
        }

        private List<AvatarItem> GetWornAvatarItemList()
        {
            List<AvatarItem> wornAvatarItemInfoList = new List<AvatarItem>();
            if (this.wornAvatarHeadItemButton != null)
            {
                wornAvatarItemInfoList.Add(this.wornAvatarHeadItemButton.AvatarItem);
            }

            if (this.wornAvatarTopItemButton != null)
            {
                wornAvatarItemInfoList.Add(this.wornAvatarTopItemButton.AvatarItem);
            }

            if (this.wornAvatarBottomItemButton != null)
            {
                wornAvatarItemInfoList.Add(this.wornAvatarBottomItemButton.AvatarItem);
            }

            return wornAvatarItemInfoList;
        }

        private IEnumerator CoroutineWaitingPurchaseAvatarItemResult()
        {
            delegateGetFlag delegateGetFlag = new delegateGetFlag(Thread.Waiter.GetThreadWaitIsPurchaseResultCompleted);
            yield return StartCoroutine(Thread.Waiter.CoroutineThreadWait(delegateGetFlag));

            if (Thread.Waiter.GetThreadWaitIsPurchaseResultCompleted() == false)
            {
                string errorText = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Game, (int)Enum.GameText.Game.Error), Enum.RequestResult.Game.ThreadWaitTimeOver);
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
            if (this.wornAvatarHeadItemButton != null)
            {
                this.wornAvatarHeadItemButton.SetOwned(true);
            }

            if (this.wornAvatarTopItemButton != null)
            {
                this.wornAvatarTopItemButton.SetOwned(true);
            }

            if (this.wornAvatarBottomItemButton != null)
            {
                this.wornAvatarBottomItemButton.SetOwned(true);
            }

            ResetCoinQunatityText();
        }

        private void DoPurchaseFailProcess(PurchaseResultType purchaseResultType)
        {
            Gui.Popup.Manager.Instance.OpenCheckPopup(Main.Instance.GetPurchaseResultText(purchaseResultType));
        }

        private void ResetCoinQunatityText()
        {
            this.userCoinQuantityText.text = User.User.Instance.GetUserCoin_1().ToString();
        }
    }
}