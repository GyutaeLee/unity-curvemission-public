using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Services.Enum.Sprite;
using Services.Enum.Inventory;
using Services.Enum.Item;
using Services.Item.Avatar;
using Services.Character;
using Services.Useful;

using Util.Csv.Data;

namespace Services.Scene.Garage
{
    public class AvatarItemCloset : MonoBehaviour
    {
        private static class Constant
        {
            public const int ItemSingleLineCount = 3;
        }

        public class ClosetAvatarItemButton : AvatarItemUI
        {
            public Button ItemButton { get; set; }
            private GameObject unownedStateObject;

            public ClosetAvatarItemButton(GameObject gameObject) : base(gameObject)
            {
                this.ItemButton = gameObject.GetComponent<Button>();
                ObjectFinder.FindGameObjectInAllChild(ref this.unownedStateObject, gameObject, "UnownedStateObject", true);
            }

            public void SetOwned(bool isOwned)
            {
                this.ItemButton.targetGraphic.enabled = !isOwned;
                this.ItemButton.enabled = !isOwned;

                this.unownedStateObject.SetActive(isOwned);
            }
        }

        private AvatarItemType _currentAvatarItemType;
        private AvatarItemType currentAvatarItemType
        {
            get
            {
                return _currentAvatarItemType;
            }
            set
            {
                if (value <= AvatarItemType.None || value >= AvatarItemType.Max)
                {
                    _currentAvatarItemType = AvatarItemType.None;
                }
                else
                {
                    _currentAvatarItemType = value;
                }
            }
        }

        private AvatarUI myAvatarUI;

        public List<List<ClosetAvatarItemButton>> closetAvatarItemButtons;
        public List<int> selectedClosetAvatarItemIndex;

        private GameObject avatarItemClosetCanvas;

        private GameObject closetAvatarItemButtonObject;

        private Image closetAvatarItemSelectedBox;

        private Sprite melonButtonEnableSprite;
        private Sprite melonButtonDisableSprite;

        private Button headButton;
        private Button topButton;
        private Button bottomButton;

        private Button avatarItemLeftButton;
        private Button avatarItemRightButton;

        private Button closeButton;

        private UnityEngine.UI.Text userCoinQuantityText;

        public void Open()
        {
            InstantiateCanvas();

            PrepareObjects();
            PrepareButtons();

            Initialize();
        }

        public void Close()
        {
            Destroy(this.avatarItemClosetCanvas);
            Destroy(this);
        }

        private void InstantiateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");

            this.avatarItemClosetCanvas = Resources.Load<GameObject>("Prefab/UI/Canvas/AvatarItemClosetCanvas");
            this.avatarItemClosetCanvas = Instantiate(this.avatarItemClosetCanvas, canvas.transform);
            this.avatarItemClosetCanvas.SetActive(true);
        }

        private void PrepareObjects()
        {
            GameObject myAvatar = null;
            ObjectFinder.FindGameObjectInAllChild(ref myAvatar, this.avatarItemClosetCanvas, "MyAvatar", true);
            this.myAvatarUI = new AvatarUI(myAvatar);

            ObjectFinder.FindGameObjectInAllChild(ref this.closetAvatarItemButtonObject, this.avatarItemClosetCanvas, "ClosetAvatarItemButtonObject", true);
            ObjectFinder.FindComponentInAllChild(ref this.closetAvatarItemSelectedBox, this.avatarItemClosetCanvas, "ClosetAvatarItemSelectedBoxImage", true);

            ObjectFinder.FindComponentInAllChild(ref this.userCoinQuantityText, this.avatarItemClosetCanvas, "UserCoinQuantityText", true);
        }

        private void PrepareButtons()
        {
            Sprite[] buttonPopupSprites = Resources.LoadAll<Sprite>(Services.Constants.UI.ButtonPopupSheetPath);

            this.melonButtonEnableSprite = buttonPopupSprites[(int)BasicButtonSheet.MelonButtonEnabled];
            this.melonButtonDisableSprite = buttonPopupSprites[(int)BasicButtonSheet.MelonButtonDisabled];

            ObjectFinder.FindComponentInAllChild(ref this.headButton, this.avatarItemClosetCanvas, "HeadButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.topButton, this.avatarItemClosetCanvas, "TopButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.bottomButton, this.avatarItemClosetCanvas, "BottomButton", true);
            this.headButton.onClick.AddListener(() => { ChangeClosetAvatarItemButton(AvatarItemType.Head); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.topButton.onClick.AddListener(() => { ChangeClosetAvatarItemButton(AvatarItemType.Top); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.bottomButton.onClick.AddListener(() => { ChangeClosetAvatarItemButton(AvatarItemType.Bottom); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.avatarItemLeftButton, this.avatarItemClosetCanvas, "AvatarItemLeftButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.avatarItemRightButton, this.avatarItemClosetCanvas, "AvatarItemRightButton", true);
            this.avatarItemLeftButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.avatarItemRightButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.closeButton, this.avatarItemClosetCanvas, "CloseButton", true);
            this.closeButton.onClick.AddListener(() => { Close(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void Initialize()
        {
            this.currentAvatarItemType = AvatarItemType.Head;

            InitializeMyAvatar();
            InitializeClosetAvatarItemButtons();

            ChangeClosetAvatarItemButton(this.currentAvatarItemType);
            ChangeClosetAvatarItemSelectedBoxPosition(this.selectedClosetAvatarItemIndex[(int)this.currentAvatarItemType]);

            ResetUserCoinQuantityText();
        }

        private void InitializeMyAvatar()
        {
            int headInfoID = User.User.Instance.CurrentAvatar.Head.InfoID;
            int topInfoID = User.User.Instance.CurrentAvatar.Top.InfoID;
            int bottomInfoID = User.User.Instance.CurrentAvatar.Bottom.InfoID;

            this.myAvatarUI.CreateAvatar(headInfoID, topInfoID, bottomInfoID);
            this.myAvatarUI.SetImage();
        }

        private void InitializeClosetAvatarItemButtons()
        {
            InitializeClosetAvatarItemsButtonData();

            for (int i = 0; i < this.closetAvatarItemButtons.Count; i++)
            {
                for (int j = 0; j < this.closetAvatarItemButtons[i].Count; j++)
                {
                    ClosetAvatarItemButton copyClosetAvatarItemButton = this.closetAvatarItemButtons[i][j];

                    SetClosetAvatarItemButtonImageAndText(copyClosetAvatarItemButton);
                    CheckAndInActiveUnownedClosetAvatarItemButton(copyClosetAvatarItemButton);
                }
            }
        }

        private void InitializeClosetAvatarItemsButtonData()
        {
            this.closetAvatarItemButtons = new List<List<ClosetAvatarItemButton>>();
            this.selectedClosetAvatarItemIndex = new List<int>(new int[(int)AvatarItemType.Max]);

            for (int i = 0; i < this.closetAvatarItemButtonObject.transform.childCount; i++)
            {
                Transform child = this.closetAvatarItemButtonObject.transform.GetChild(i);
                AvatarItemType avatarItemType = (AvatarItemType)(i + 1);
                List<ClosetAvatarItemButton> closetAvatarItemButtonList = CreateClosetAvatarItemButtonList(child, avatarItemType);

                this.closetAvatarItemButtons.Add(closetAvatarItemButtonList);
            }
        }

        private List<ClosetAvatarItemButton> CreateClosetAvatarItemButtonList(Transform transform, AvatarItemType avatarItemType)
        {
            List<ClosetAvatarItemButton> closetAvatarItemHeadButtonList = new List<ClosetAvatarItemButton>();

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
                ClosetAvatarItemButton clostAvatarItemButton = new ClosetAvatarItemButton(grandson);

                CreateClosetAvatarItemButtonAvatarItem(clostAvatarItemButton, avatarItem);
                AddClosetAvatarItemButtonListener(clostAvatarItemButton, i);
                CheckAndSetClosetAvatarItemSelectedItemIndex(clostAvatarItemButton, i);

                closetAvatarItemHeadButtonList.Add(clostAvatarItemButton);
            }

            return closetAvatarItemHeadButtonList;
        }

        private void CreateClosetAvatarItemButtonAvatarItem(ClosetAvatarItemButton closetAvatarItemButton, Util.Csv.Data.Class.AvatarItem avatarItem)
        {
            switch ((AvatarItemType)avatarItem.Type)
            {
                case AvatarItemType.Head:
                    closetAvatarItemButton.AvatarItem = new Item.Avatar.Head(avatarItem.InfoID);
                    break;
                case AvatarItemType.Top:
                    closetAvatarItemButton.AvatarItem = new Item.Avatar.Top(avatarItem.InfoID);
                    break;
                case AvatarItemType.Bottom:
                    closetAvatarItemButton.AvatarItem = new Item.Avatar.Bottom(avatarItem.InfoID);
                    break;
                default:
                    break;
            }

            
        }

        private void AddClosetAvatarItemButtonListener(ClosetAvatarItemButton closetAvatarItemButton, int itemPositionIndex)
        {
            closetAvatarItemButton.ItemButton.onClick.AddListener(() => { ChangeMyAvatarItem(closetAvatarItemButton.AvatarItem, itemPositionIndex); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        // TODO : UI를 닫을 때 한번에 서버에 보내도록 수정
        private void ChangeMyAvatarItem(AvatarItem avatarItem, int itemPositionIndex)
        {
            this.selectedClosetAvatarItemIndex[(int)avatarItem.Type] = itemPositionIndex;

            switch (avatarItem.Type)
            {
                case AvatarItemType.Head:
                    ChangeMyAvatarItemHead(avatarItem.InfoID, itemPositionIndex);
                    break;
                case AvatarItemType.Top:
                    ChangeMyAvatarItemTop(avatarItem.InfoID, itemPositionIndex);
                    break;
                case AvatarItemType.Bottom:
                    ChangeMyAvatarItemBottom(avatarItem.InfoID, itemPositionIndex);
                    break;
                default:
                    break;
            }
        }

        private void ChangeMyAvatarItemHead(int headInfoID, int itemIndex)
        {
            string textKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(AvatarInventoryType.Head);

            if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(textKey, headInfoID) == false)
            {
                return;
            }

            if (this.myAvatarUI.GetHeadInfoID() == headInfoID)
            {
                return;
            }

            User.User.Instance.ChangeEquippedAvatarItemHeadInfoID(headInfoID);
            this.myAvatarUI.ChangeHead(User.User.Instance.CurrentAvatar.Head.InfoID);

            ChangeClosetAvatarItemSelectedBoxPosition(itemIndex);
        }

        private void ChangeMyAvatarItemTop(int topInfoID, int itemIndex)
        {
            string textKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(AvatarInventoryType.Top);

            if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(textKey, topInfoID) == false)
            {
                return;
            }

            if (this.myAvatarUI.GetTopInfoID() == topInfoID)
            {
                return;
            }

            User.User.Instance.ChangeEquippedAvatarItemTopInfoID(topInfoID);
            this.myAvatarUI.ChangeTop(User.User.Instance.CurrentAvatar.Top.InfoID);

            ChangeClosetAvatarItemSelectedBoxPosition(itemIndex);
        }

        private void ChangeMyAvatarItemBottom(int bottomInfoID, int itemIndex)
        {
            string textKey = Static.AvatarInventory.GetAvatarInventoryTypeTextKey(AvatarInventoryType.Bottom);

            if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(textKey, bottomInfoID) == false)
            {
                return;
            }

            if (this.myAvatarUI.GetBottomInfoID() == bottomInfoID)
            {
                return;
            }

            User.User.Instance.ChangeEquippedAvatarItemBottomInfoID(bottomInfoID);
            this.myAvatarUI.ChangeBottom(User.User.Instance.CurrentAvatar.Bottom.InfoID);

            ChangeClosetAvatarItemSelectedBoxPosition(itemIndex);
        }

        private void CheckAndSetClosetAvatarItemSelectedItemIndex(ClosetAvatarItemButton closetAvatarItemButton, int itemPositionIndex)
        {
            switch (closetAvatarItemButton.AvatarItem.Type)
            {
                case AvatarItemType.Head:
                    if (closetAvatarItemButton.AvatarItem.InfoID == this.myAvatarUI.GetHeadInfoID())
                    {
                        this.selectedClosetAvatarItemIndex[(int)AvatarItemType.Head] = itemPositionIndex;
                    }
                    break;
                case AvatarItemType.Top:
                    if (closetAvatarItemButton.AvatarItem.InfoID == this.myAvatarUI.GetTopInfoID())
                    {
                        this.selectedClosetAvatarItemIndex[(int)AvatarInventoryType.Top] = itemPositionIndex;
                    }
                    break;
                case AvatarItemType.Bottom:
                    if (closetAvatarItemButton.AvatarItem.InfoID == this.myAvatarUI.GetBottomInfoID())
                    {
                        this.selectedClosetAvatarItemIndex[(int)AvatarInventoryType.Bottom] = itemPositionIndex;
                    }
                    break;
                default:
                    break;
            }
        }

        private void SetClosetAvatarItemButtonImageAndText(ClosetAvatarItemButton closetAvatarItemButton)
        {
            closetAvatarItemButton.SetImage();
            closetAvatarItemButton.SetText();
        }

        private void CheckAndInActiveUnownedClosetAvatarItemButton(ClosetAvatarItemButton closetAvatarItemButton)
        {
            if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(closetAvatarItemButton.AvatarItem) == true)
            {
                closetAvatarItemButton.SetOwned(false);
            }
            else
            {
                closetAvatarItemButton.SetOwned(true);
            }
        }

        private void ChangeClosetAvatarItemButton(AvatarItemType avatarItemType)
        {
            if (avatarItemType == AvatarItemType.None)
                return;

            if (avatarItemType == this.currentAvatarItemType)
                return;

            this.currentAvatarItemType = avatarItemType;

            SetActiveClosetAvatarItemButtonObject(avatarItemType);

            ChangeClosetAvatarItemButtonSprite(avatarItemType);
            ChangeClosetAvatarItemSelectedBoxPosition(this.selectedClosetAvatarItemIndex[(int)avatarItemType]);
        }

        private void SetActiveClosetAvatarItemButtonObject(AvatarItemType avatarItemType)
        {
            int selectedIndex = (int)avatarItemType - 1;

            for (int i = 0; i < this.closetAvatarItemButtonObject.transform.childCount; i++)
            {
                GameObject goodsUI = this.closetAvatarItemButtonObject.transform.GetChild(i).gameObject;

                if (i == selectedIndex)
                {
                    goodsUI.SetActive(true);
                }
                else
                {
                    goodsUI.SetActive(false);
                }
            }
        }

        private void ChangeClosetAvatarItemButtonSprite(AvatarItemType avatarItemType)
        {
            switch (avatarItemType)
            {
                case AvatarItemType.Head:
                    this.headButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    this.topButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.bottomButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    break;
                case AvatarItemType.Top:
                    this.headButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.topButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    this.bottomButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    break;
                case AvatarItemType.Bottom:
                    this.headButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.topButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.bottomButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    break;
                default:
                    break;
            }
        }

        private void ResetUserCoinQuantityText()
        {
            this.userCoinQuantityText.text = User.User.Instance.GetUserCoin_1().ToString();
        }

        static readonly float[] ItemXPosition = { -50.0f, 0.0f, 50.0f };
        static readonly float[] ItemYPosition = { -26.0f, -46.0f, -66.0f };
        private void ChangeClosetAvatarItemSelectedBoxPosition(int itemPositionIndex)
        {
            int xIndex = itemPositionIndex % Constant.ItemSingleLineCount;
            int yIndex = itemPositionIndex / Constant.ItemSingleLineCount;

            Vector2 v = new Vector2();
            v.x = ItemXPosition[xIndex];
            v.y = ItemYPosition[yIndex];

            this.closetAvatarItemSelectedBox.rectTransform.anchoredPosition = v;
        }
    }
}