using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Services.Enum.Item;
using Services.Enum.Sprite;
using Services.Enum.Inventory;
using Services.Vehicle;
using Services.Item.Vehicle;
using Services.Useful;

using Util.Csv.Data;

namespace Services.Scene.Garage
{
    public class CarItemGarage : MonoBehaviour
    {
        private static class Constant
        {
            public const int UnSelectedPositionIndex = -1;
        }

        private class GarageCarItemButton : CarItemUI
        {
            public Button ItemButton { get; set; }
            private GameObject unownedStateObject;

            public GarageCarItemButton(GameObject gameObject) : base(gameObject)
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

        private CarItemType _currentCarItemType;
        private CarItemType currentCarItemType
        {
            get
            {
                return _currentCarItemType;
            }
            set
            {
                if (value <= CarItemType.None || value >= CarItemType.Max)
                {
                    _currentCarItemType = CarItemType.None;
                }
                else
                {
                    _currentCarItemType = value;
                }
            }
        }

        private CarUI myCarUI;

        private List<List<GarageCarItemButton>> garageCarItemButtons;
        private List<int> selectedGarageCarItemIndex;
                
        private GameObject carItemGarageCanvas;
                
        private GameObject garageCarItemButtonObject;

        private Image garageCarItemSelectedBox;

        private Sprite melonButtonEnableSprite;
        private Sprite melonButtonDisableSprite;

        private Button carButton;
        private Button paintButton;
        private Button partsButton;

        private Button carItemleftButton;
        private Button carItemRightButton;

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
            Destroy(this.carItemGarageCanvas);
            Destroy(this);
        }

        private void InstantiateCanvas()
        {
            GameObject canvas = GameObject.Find("Canvas");

            this.carItemGarageCanvas = Resources.Load<GameObject>("Prefab/UI/Canvas/CarItemGarageCanvas");
            this.carItemGarageCanvas = Instantiate(this.carItemGarageCanvas, canvas.transform);
            this.carItemGarageCanvas.SetActive(true);
        }

        private void PrpareObjects()
        {
            ObjectFinder.FindGameObjectInAllChild(ref this.garageCarItemButtonObject, this.carItemGarageCanvas, "GarageCarItemButtonObject", true);
            ObjectFinder.FindComponentInAllChild(ref this.garageCarItemSelectedBox, this.carItemGarageCanvas, "GarageCarItemSelectedBoxImage", true);

            ObjectFinder.FindComponentInAllChild(ref this.userCoinQuantityText, this.carItemGarageCanvas, "UserCoinQuantityText", true);
        }

        private void PrepareButtons()
        {
            Sprite[] buttonPopupSprites = Resources.LoadAll<Sprite>(Services.Constants.UI.ButtonPopupSheetPath);

            this.melonButtonEnableSprite = buttonPopupSprites[(int)BasicButtonSheet.MelonButtonEnabled];
            this.melonButtonDisableSprite = buttonPopupSprites[(int)BasicButtonSheet.MelonButtonDisabled];

            ObjectFinder.FindComponentInAllChild(ref this.carButton, this.carItemGarageCanvas, "CarButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.paintButton, this.carItemGarageCanvas, "PaintButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.partsButton, this.carItemGarageCanvas, "PartsButton", true);
            this.carButton.onClick.AddListener(() => { ChangeGarageCarItemButton(CarItemType.Car); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.paintButton.onClick.AddListener(() => { ChangeGarageCarItemButton(CarItemType.Paint); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.partsButton.onClick.AddListener(() => { ChangeGarageCarItemButton(CarItemType.Parts); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.carItemleftButton, this.carItemGarageCanvas, "CarItemLeftButton", true);
            ObjectFinder.FindComponentInAllChild(ref this.carItemRightButton, this.carItemGarageCanvas, "CarItemRightButton", true);
            this.carItemleftButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
            this.carItemRightButton.onClick.AddListener(() => { Debug.Log("기능 없음. 추가 필요"); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });

            ObjectFinder.FindComponentInAllChild(ref this.closeButton, this.carItemGarageCanvas, "CloseButton", true);
            this.closeButton.onClick.AddListener(() => { Close(); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        private void Initialize()
        {
            this.currentCarItemType = CarItemType.Car;

            InitializeMyCarUI();
            SetGarageCarItemButtons();

            ChangeGarageCarItemButton(this.currentCarItemType);
            ChangeGarageCarItemSelectedBoxPosition(this.selectedGarageCarItemIndex[(int)this.currentCarItemType]);

            ResetUserCoinQuantityText();
        }

        private void InitializeMyCarUI()
        {
            GameObject myCarObject = null;
            ObjectFinder.FindGameObjectInAllChild(ref myCarObject, this.carItemGarageCanvas, "MyCar", true);

            this.myCarUI = new CarUI(User.User.Instance.CurrentCar, myCarObject);
            this.myCarUI.SetImage();
            this.myCarUI.SetText();
        }

        private void ChangeMyCarUI(int carItemCarInfoID, int carItemPaintInfoID)
        {
            this.myCarUI.Car.Change(new Item.Vehicle.Car(carItemCarInfoID), new Item.Vehicle.Paint(carItemPaintInfoID), null);
            
            this.myCarUI.SetImage();
            this.myCarUI.SetText();
        }

        private void SetGarageCarItemButtons()
        {
            // TODO : 이거 따로 빼면서, 오류 잡아야함!!!!!
            CreateGarageCarItemsData();

            for (int i = 0; i < this.garageCarItemButtons.Count; i++)
            {
                for (int j = 0; j < this.garageCarItemButtons[i].Count; j++)
                {
                    GarageCarItemButton copyGarageCarItemButton = this.garageCarItemButtons[i][j];

                    SetGarageCarItemButtonImageAndText(copyGarageCarItemButton);
                    CheckAndInActiveUnownedGarageCarItemButton(copyGarageCarItemButton);
                }
            }
        }

        private void CreateGarageCarItemsData()
        {
            this.selectedGarageCarItemIndex = new List<int>(new int[(int)CarItemType.Max]);
            for (int i = 0; i < this.selectedGarageCarItemIndex.Count; i++)
            {
                this.selectedGarageCarItemIndex[i] = Constant.UnSelectedPositionIndex;
            }

            this.garageCarItemButtons = new List<List<GarageCarItemButton>>();
            for (int i = 0; i < this.garageCarItemButtonObject.transform.childCount; i++)
            {
                Transform child = this.garageCarItemButtonObject.transform.GetChild(i);
                CarItemType carItemType = (CarItemType)(i + 1);
                List<GarageCarItemButton> garageCarItemButtonList = CreateGarageCarItemButtonList(child, carItemType);

                this.garageCarItemButtons.Add(garageCarItemButtonList);
            }
        }

        private List<GarageCarItemButton> CreateGarageCarItemButtonList(Transform transform, CarItemType carItemType)
        {
            List<GarageCarItemButton> garageCarItemButtonList = new List<GarageCarItemButton>();

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
                GarageCarItemButton garageCarItemButton = new GarageCarItemButton(grandson);

                CreateGarageCarItemButtonCarItem(garageCarItemButton, carItem);
                AddGarageCarItemButtonListener(garageCarItemButton, i);
                CheckAndSetGarageCarItemSelectedItemIndex(garageCarItemButton, i);

                garageCarItemButtonList.Add(garageCarItemButton);
            }

            return garageCarItemButtonList;
        }

        private void CreateGarageCarItemButtonCarItem(GarageCarItemButton garageCarItemButton, Util.Csv.Data.Class.CarItem carItem)
        {
            switch ((CarItemType)carItem.Type)
            {
                case CarItemType.Car:
                    garageCarItemButton.CarItem = new Item.Vehicle.Car(carItem.InfoID, carItem.Price);
                    break;
                case CarItemType.Paint:
                    garageCarItemButton.CarItem = new Item.Vehicle.Paint(carItem.InfoID, carItem.Price);
                    break;
                case CarItemType.Parts:
                    garageCarItemButton.CarItem = new Item.Vehicle.Parts(carItem.InfoID, carItem.Price);
                    break;
                default:
                    break;
            }
        }

        private void AddGarageCarItemButtonListener(GarageCarItemButton garageCarItemButton, int itemPositionIndex)
        {
            garageCarItemButton.ItemButton.onClick.AddListener(() => { ChangeMyCarItem(garageCarItemButton.CarItem, itemPositionIndex); Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1); });
        }

        // TODO : UI를 닫을 때 한번에 서버에 보내도록 수정
        private void ChangeMyCarItem(CarItem carItemInfo, int itemIndex)
        {
            this.selectedGarageCarItemIndex[(int)carItemInfo.Type] = itemIndex;

            switch (carItemInfo.Type)
            {
                case CarItemType.Car:
                    ChangeMyCarItemCar(carItemInfo.InfoID, itemIndex);
                    break;
                case CarItemType.Paint:
                    ChangeMyCarItemPaint(carItemInfo.InfoID, itemIndex);
                    break;
                case CarItemType.Parts:
                    ChangeMyCarItemParts(carItemInfo.InfoID, itemIndex);
                    break;
                default:
                    break;
            }
        }

        private void ChangeMyCarItemCar(int carItemCarInfoID, int itemIndex)
        {
            string textKey = Static.CarInventory.GetCarInvetoryTypeTextKey(CarInventoryType.Car);

            if (User.User.Instance.IsOwnedCarItemInCarInventory(textKey, carItemCarInfoID, Constants.CarItem.DefaultCarItemPaintInfoID) == false)
                return;            

            if (this.myCarUI.Car.CarInfoID == carItemCarInfoID)
                return;

            int carItemPaintInfoID = Constants.CarItem.DefaultCarItemPaintInfoID;

            ChangeMyCarUI(carItemCarInfoID, carItemPaintInfoID);

            ResetGarageCarItemSelectedBoxIndex();
            SetGarageCarItemButtons();

            User.User.Instance.SetEquippedCarItemCarInfoID(carItemCarInfoID);
            User.User.Instance.SetEquippedCarItemPaintInfoID(carItemPaintInfoID);
        }

        private void ChangeMyCarItemPaint(int paintInfoID, int itemIndex)
        {
            string textKey = Static.CarInventory.GetCarInvetoryTypeTextKey(CarInventoryType.Paint);

            if (User.User.Instance.IsOwnedCarItemInCarInventory(textKey, this.myCarUI.Car.CarInfoID, paintInfoID) == false)
                return;

            if (this.myCarUI.Car.PaintInfoID == paintInfoID)
                return;

            ChangeMyCarUI(this.myCarUI.Car.CarInfoID, paintInfoID);
            ChangeGarageCarItemSelectedBoxPosition(itemIndex);

            User.User.Instance.SetEquippedCarItemPaintInfoID(paintInfoID);
        }

        private void ChangeMyCarItemParts(int partInfoID, int itemPoisitionIndex)
        {
            string textKey = Static.CarInventory.GetCarInvetoryTypeTextKey(CarInventoryType.Parts);

            if (User.User.Instance.IsOwnedCarItemInCarInventory(textKey, this.myCarUI.Car.CarInfoID, partInfoID) == false)
            {
                return;
            }

            // TODO : 추가 작업 필요
        }

        private void CheckAndSetGarageCarItemSelectedItemIndex(GarageCarItemButton garageCarItemButton, int itemIndex)
        {
            switch (garageCarItemButton.CarItem.Type)
            {
                case CarItemType.Car:
                    if (garageCarItemButton.CarItem.InfoID == this.myCarUI.Car.CarInfoID)
                    {
                        this.selectedGarageCarItemIndex[(int)CarItemType.Car] = itemIndex;
                    }
                    break;
                case CarItemType.Paint:
                    if (garageCarItemButton.CarItem.InfoID == this.myCarUI.Car.PaintInfoID)
                    {
                        this.selectedGarageCarItemIndex[(int)CarItemType.Paint] = itemIndex;
                    }
                    break;
                case CarItemType.Parts:
                    break;
                default:
                    break;
            }
        }

        private void SetGarageCarItemButtonImageAndText(GarageCarItemButton garageCarItemButton)
        {
            garageCarItemButton.SetImage();
            garageCarItemButton.SetText();
        }

        private void CheckAndInActiveUnownedGarageCarItemButton(GarageCarItemButton garageCarItemButton)
        {
            int carInfoID = (garageCarItemButton.CarItem.Type == CarItemType.Car) ? garageCarItemButton.CarItem.InfoID : this.myCarUI.Car.CarInfoID;
            if (User.User.Instance.IsOwnedCarItemInCarInventory(new Item.Vehicle.Car(carInfoID), garageCarItemButton.CarItem) == true)
            {
                garageCarItemButton.SetOwned(false);
            }
            else
            {
                garageCarItemButton.SetOwned(true);
            }
        }               

        private void ChangeGarageCarItemButton(CarItemType carItemType)
        {
            if (carItemType == CarItemType.None)
                return;

            if (carItemType == this.currentCarItemType)
                return;

            this.currentCarItemType = carItemType;

            SetActiveGarageCarItemButtonObject(carItemType);

            ChangeGarageCarItemButtonSprite(carItemType);
            ChangeGarageCarItemSelectedBoxPosition(this.selectedGarageCarItemIndex[(int)carItemType]);
        }

        private void SetActiveGarageCarItemButtonObject(CarItemType carItemType)
        {
            int selectedGarageCarItemIndex = (int)carItemType - 1;

            for (int i = 0; i < this.garageCarItemButtonObject.transform.childCount; i++)
            {
                GameObject garageCarItemObject = this.garageCarItemButtonObject.transform.GetChild(i).gameObject;

                if (i == selectedGarageCarItemIndex)
                {
                    garageCarItemObject.SetActive(true);
                }
                else
                {
                    garageCarItemObject.SetActive(false);
                }
            }
        }

        private void ChangeGarageCarItemButtonSprite(CarItemType carItemType)
        {
            switch (carItemType)
            {
                case CarItemType.Car:
                    this.carButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    this.paintButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.partsButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    break;
                case CarItemType.Paint:
                    this.carButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.paintButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    this.partsButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    break;
                case CarItemType.Parts:
                    this.carButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.paintButton.gameObject.GetComponent<Image>().sprite = this.melonButtonDisableSprite;
                    this.partsButton.gameObject.GetComponent<Image>().sprite = this.melonButtonEnableSprite;
                    break;
                default:
                    break;
            }
        }

        private void ResetGarageCarItemSelectedBoxIndex()
        {
            for (int i = 0; i < this.selectedGarageCarItemIndex.Count; i++)
            {
                this.selectedGarageCarItemIndex[i] = Constant.UnSelectedPositionIndex;
            }

            List<GarageCarItemButton> garageCarItemButtonList = this.garageCarItemButtons[(int)this.currentCarItemType - 1];
            for (int i = 0; i < garageCarItemButtonList.Count; i++)
            {
                CheckAndSetGarageCarItemSelectedItemIndex(garageCarItemButtonList[i], i);
            }

            ChangeGarageCarItemSelectedBoxPosition(this.selectedGarageCarItemIndex[(int)this.currentCarItemType]);
        }

        static readonly float[] ItemXPosition = { -55.5f, -18.5f, 18.5f, 55.5f };
        private void ChangeGarageCarItemSelectedBoxPosition(int itemIndex)
        {
            if (itemIndex == Constant.UnSelectedPositionIndex)
            {
                this.garageCarItemSelectedBox.enabled = false;
                return;
            }

            Vector2 v = new Vector2();

            v.x = ItemXPosition[itemIndex];
            v.y = this.garageCarItemSelectedBox.rectTransform.anchoredPosition.y;

            this.garageCarItemSelectedBox.rectTransform.anchoredPosition = v;
            this.garageCarItemSelectedBox.enabled = true;
        }

        private void ResetUserCoinQuantityText()
        {
            this.userCoinQuantityText.text = User.User.Instance.GetUserCoin_1().ToString();
        }
    }
}