using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUICarItemManager : MonoBehaviour
{
    const int kUICarSpriteIndex = 9;
    const int kUICarItemInitialID = 1001;

    public class ShopUICarItemInformation
    {
        private ECarItemType _eCurrentCarItemType;
        public ECarItemType eCurrentCarItemType
        {
            get
            {
                return _eCurrentCarItemType;
            }
            set
            {
                if (value <= ECarItemType.None || value >= ECarItemType.Max)
                {
                    _eCurrentCarItemType = ECarItemType.None;
                }
                else
                {
                    _eCurrentCarItemType = value;
                }
            }
        }

        public int currentMyCarInfoID;
        public int currentMyCarPaintID;
        public Image IMG_MyCar;
        public Text TXT_MyCar;

        public List<List<CarItemUI>> goodsItemUI;
        public SelectedItemUI selectedItemUI;

        public string GetCurrentMyCarSpriteName()
        {
            return "security-related" + this.currentMyCarInfoID + "/" + currentMyCarPaintID;
        }

        public CarItemUI GetGoodsItemUI(ECarItemType eCarItemType, int selectedIndex)
        {
            return this.goodsItemUI[(int)eCarItemType - 1][selectedIndex];
        }
    }

    public class CarItemUI
    {
        public ShopItemInfo.CarItemInfo carItemInfo;

        public bool isPurchasable;

        public Button BTN_Item;
        public Image IMG_Item;
        public Text TXT_Name;
        public Text TXT_Price;

        public Sprite SPT_Item;
    }

    public class SelectedItemUI
    {
        public ShopItemInfo.CarItemInfo carItemInfo;

        public int selectedIndex;

        public Image IMG_Car;
        public Image IMG_Paint;
        public List<Image> IMG_Parts;

        public Text TXT_CarName;
        public Text TXT_ItemName;
        public Text TXT_ItemPrice;
    }

    //////////////////
    // end of class //
    //////////////////

    private ShopUICarItemInformation info;

    private GameObject goodsObject;
    private GameObject selectedObject;

    private GameObject goodsUIObject;
    private GameObject selectedUIObject;

    private GameObject selectedParts;
    private GameObject selectedStats;
    private GameObject selectedPaint;

    private GameObject purchaseAskPopupObject;

    private Text TXT_CoinQuantity;

    private bool thread_wait_purchaseResult;
    private EPurchaseErrorType thread_ePurchaseErrorType;

    private void Start()
    {
        PrepareBaseObjects();
        InitShopCarItemManager();
    }

    private void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");
        GameObject carItemPopup = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "carItemPopup", true);

        GameObject myCarUI = CMObjectManager.FindGameObjectInAllChild(carItemPopup, "myCarUI", true);

        GameObject goods = CMObjectManager.FindGameObjectInAllChild(carItemPopup, "goods", true);
        GameObject selected = CMObjectManager.FindGameObjectInAllChild(carItemPopup, "selected", true);

        this.goodsObject = goods;
        this.selectedObject = selected;

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.goodsUIObject, goods, "goodsUI", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.selectedUIObject, selected, "selectedUI", true);

        this.info = new ShopUICarItemInformation();

        /* My Car UI */
        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.info.IMG_MyCar, myCarUI, "IMG_Item", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.info.TXT_MyCar, myCarUI, "TXT_Name", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_CoinQuantity, carItemPopup, "TXT_CoinQuantity", true);

        /* Shop UI */

        // popup
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.purchaseAskPopupObject, selected, "purchaseAskPopup", true);

        // selectedUI
        this.info.selectedItemUI = new SelectedItemUI();
        this.info.selectedItemUI.IMG_Parts = new List<Image>();

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.selectedParts, selected, "selectedParts", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.selectedStats, selected, "selectedStats", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.selectedPaint, selected, "selectedPaint", true);

        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.info.selectedItemUI.IMG_Car, this.selectedUIObject, "IMG_Item", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.info.selectedItemUI.TXT_CarName, this.selectedUIObject, "TXT_Name", true);

        CMObjectManager.CheckNullAndFindImageInAllChild(ref this.info.selectedItemUI.IMG_Paint, this.selectedPaint, "IMG_Paint", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.info.selectedItemUI.TXT_ItemName, this.selectedPaint, "TXT_PaintName", true);

        GameObject selectedButton = CMObjectManager.FindGameObjectInAllChild(this.selectedObject, "Button", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.info.selectedItemUI.TXT_ItemPrice, selectedButton, "TXT_Price", true);
    }

    private void InitShopCarItemManager()
    {
        this.info.eCurrentCarItemType = ECarItemType.Car;

        InitMyCar();
        InitGoodsItemInfo();
        InitGoodsItemUI();

        ResetCoinQunatityText();
    }

    private void InitMyCar()
    {
        this.info.currentMyCarInfoID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultCarInfoID());
        this.info.currentMyCarPaintID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultPaintInfoID());

        Sprite sprite = Resources.LoadAll<Sprite>(this.info.GetCurrentMyCarSpriteName())[kUICarSpriteIndex];
        this.info.IMG_MyCar.sprite = sprite;
        this.info.IMG_MyCar.SetNativeSize();
        this.info.TXT_MyCar.text = TextManager.instance.GetText(ETextType.Car, this.info.currentMyCarInfoID);
    }

    private void InitGoodsItemInfo()
    {
        this.info.goodsItemUI = new List<List<CarItemUI>>();

        for (int i = 0; i < this.goodsUIObject.transform.childCount; i++)
        {
            List<CarItemUI> tList = new List<CarItemUI>();
            Transform child = this.goodsUIObject.transform.GetChild(i);

            for (int j = 0; j < child.childCount; j++)
            {
                Transform grandson = child.GetChild(j);

                ECarItemType eCarItemType = (ECarItemType)(i + 1);
                ShopCsvReader.ShopCarItemCommonInformation commonInfo = ShopCsvReader.instance.GetItemInfo(eCarItemType, j);

                if (commonInfo == null)
                {
                    continue;
                }

                CarItemUI goodsItemUI = new CarItemUI();
                SetGoodsItemInfo(goodsItemUI, grandson.GetComponent<ShopItemInfo>().info, commonInfo);
                SetGoodsItemUI(goodsItemUI, grandson);

                tList.Add(goodsItemUI);
            }

            this.info.goodsItemUI.Add(tList);
        }
    }

    private void SetGoodsItemInfo(CarItemUI goodsItemUI, ShopItemInfo.CarItemInfo carItemInfo, ShopCsvReader.ShopCarItemCommonInformation commonInfo)
    {
        goodsItemUI.carItemInfo = carItemInfo;
        goodsItemUI.carItemInfo.eCarItemType = commonInfo.eCarItemType;
        goodsItemUI.carItemInfo.currentCarID = this.info.currentMyCarInfoID;
        goodsItemUI.carItemInfo.carItemInfoID = commonInfo.carItemInfoID;
        goodsItemUI.carItemInfo.carItemPrice = commonInfo.carItemPrice;
    }

    private void SetGoodsItemUI(CarItemUI goodsItemUI, Transform transform)
    {
        goodsItemUI.BTN_Item = transform.GetComponent<Button>();
        goodsItemUI.IMG_Item = transform.Find("IMG_Item").GetComponent<Image>();
        goodsItemUI.TXT_Name = transform.Find("TXT_Name").GetComponent<Text>();
        goodsItemUI.TXT_Price = transform.Find("TXT_Price").GetComponent<Text>();
    }

    private void InitGoodsItemUI()
    {
        for (int i = 0; i < this.info.goodsItemUI.Count; i++)
        {
            ECarInventoryType eCarInventoryType = (ECarInventoryType)(i + 1);

            for (int j = 0; j < this.info.goodsItemUI[i].Count; j++)
            {
                CarItemUI copyCarItemUI = this.info.goodsItemUI[i][j];

                SetCarItemUIImageAndText(copyCarItemUI);
                CheckAndInActiveOwnedCarItemUI(eCarInventoryType, copyCarItemUI);
            }
        }
    }

    /* setting function */

    private void CheckAndInActiveOwnedCarItemUI(ECarInventoryType eCarInventoryType, CarItemUI carItemUI)
    {
        if (CheckOwnedCarItem(eCarInventoryType, carItemUI.carItemInfo) == false)
        {
            return;
        }

        InActiveOwnedCarItemUI(carItemUI);
    }

    private bool CheckOwnedCarItem(ECarInventoryType eCarInventoryType, ShopItemInfo.CarItemInfo carItemInfo)
    {
        string textKey = InventoryInformation.GetCarInvetoryTextKey(eCarInventoryType);

        switch (eCarInventoryType)
        {
            case ECarInventoryType.Car:
                if (UserManager.instance.GetCarInventory(textKey, carItemInfo.carItemInfoID, carItemInfo.carItemInfoID) == false)
                {
                    return false;
                }
                break;
            case ECarInventoryType.Paint:
            case ECarInventoryType.Parts:
                if (UserManager.instance.GetCarInventory(textKey, this.info.currentMyCarInfoID, carItemInfo.carItemInfoID) == false)
                {
                    return false;
                }
                break;
            default:
                break;
        }

        return true;
    }

    private void InActiveOwnedCarItemUI(CarItemUI carItemUI)
    {
        carItemUI.isPurchasable = false;
        carItemUI.BTN_Item.targetGraphic.enabled = false;
        carItemUI.BTN_Item.enabled = false;
    }

    private void SetCarItemUIImageAndText(CarItemUI carItemUI)
    {
        SetCarItemUIImage(carItemUI);
        SetCarItemUIText(carItemUI);
    }

    private void SetCarItemUIImage(CarItemUI carItemUI)
    {
        string spriteName = carItemUI.carItemInfo.GetCarItemSpriteName();
        Sprite[] sprites = Resources.LoadAll<Sprite>(spriteName);
        int spriteIndex = 0;

        switch (carItemUI.carItemInfo.eCarItemType)
        {
            case ECarItemType.Car:
                spriteIndex = kUICarSpriteIndex;
                break;
            case ECarItemType.Paint:
            case ECarItemType.Parts:
                spriteIndex = carItemUI.carItemInfo.carItemInfoID - kUICarItemInitialID;
                break;
            default:
                break;
        }

        if (sprites.Length > spriteIndex)
        {
            carItemUI.SPT_Item = sprites[spriteIndex];
            carItemUI.IMG_Item.sprite = carItemUI.SPT_Item;
            carItemUI.IMG_Item.SetNativeSize();
        }
    }

    private void SetCarItemUIText(CarItemUI carItemUI)
    {
        if (carItemUI.TXT_Name != null)
        {
            carItemUI.TXT_Name.text = TextManager.instance.GetText(carItemUI.carItemInfo.GetCarItemTextType(), carItemUI.carItemInfo.carItemInfoID);
        }

        if (carItemUI.TXT_Price != null)
        {
            carItemUI.TXT_Price.text = carItemUI.carItemInfo.carItemPrice.ToString();
        }
    }

    private void SetSelectedItemUIImageAndText()
    {
        SetSelectedItemUIImage();
        SetSelectedItemUIText();
    }

    private void SetSelectedItemUIImage()
    {
        SelectedItemUI selectedItemUI = this.info.selectedItemUI;

        switch (selectedItemUI.carItemInfo.eCarItemType)
        {
            case ECarItemType.Car:
                SetSelectedItemCarImage();
                break;
            case ECarItemType.Paint:
                SetSelectedItemPaintImage();
                break;
            case ECarItemType.Parts:
                SetSelectedItemPartsIamge();
                break;
            default:
                break;
        }
    }
    
    private void SetSelectedItemCarImage()
    {
        SelectedItemUI selectedItemUI = this.info.selectedItemUI;
        string currentCarSpriteName;
        Sprite currentCarSprite;

        currentCarSpriteName = selectedItemUI.carItemInfo.GetCarItemSpriteName();
        currentCarSprite = Resources.LoadAll<Sprite>(currentCarSpriteName)[kUICarSpriteIndex];

        selectedItemUI.IMG_Car.sprite = currentCarSprite;
    }

    private void SetSelectedItemPaintImage()
    {
        SelectedItemUI selectedItemUI = this.info.selectedItemUI;
        string currentCarSpriteName, PaintSpriteName;
        Sprite currentCarSprite, paintSprite;

        currentCarSpriteName = "security-related" + this.info.currentMyCarInfoID + "/" + selectedItemUI.carItemInfo.carItemInfoID;
        currentCarSprite = Resources.LoadAll<Sprite>(currentCarSpriteName)[kUICarSpriteIndex];

        PaintSpriteName = selectedItemUI.carItemInfo.GetCarItemSpriteName();
        paintSprite = Resources.LoadAll<Sprite>(PaintSpriteName)[selectedItemUI.carItemInfo.carItemInfoID - kUICarItemInitialID];

        selectedItemUI.IMG_Car.sprite = currentCarSprite;
        selectedItemUI.IMG_Paint.sprite = paintSprite;
    }

    private void SetSelectedItemPartsIamge()
    {
        SelectedItemUI selectedItemUI = this.info.selectedItemUI;
        string currentCarSpriteName, partsSpriteName;
        Sprite currentCarSprite, partsSprite;

        currentCarSpriteName = this.info.GetCurrentMyCarSpriteName();
        currentCarSprite = Resources.LoadAll<Sprite>(currentCarSpriteName)[kUICarSpriteIndex];

        partsSpriteName = selectedItemUI.carItemInfo.GetCarItemSpriteName();
        partsSprite = Resources.LoadAll<Sprite>(partsSpriteName)[selectedItemUI.carItemInfo.carItemInfoID - kUICarItemInitialID];

        selectedItemUI.IMG_Car.sprite = currentCarSprite;
        // TO DO : Parts는 어떻게 할지 고민
        //selectedItemUI.IMG_Parts.sprite = carItemSprite;
    }

    private void SetSelectedItemUIText()
    {
        SelectedItemUI selectedItemUI = this.info.selectedItemUI;

        if (selectedItemUI.TXT_CarName != null)
        {
            switch (selectedItemUI.carItemInfo.eCarItemType)
            {
                case ECarItemType.Car:
                    selectedItemUI.TXT_CarName.text = TextManager.instance.GetText(ETextType.Car, selectedItemUI.carItemInfo.carItemInfoID);
                    break;
                case ECarItemType.Paint:
                case ECarItemType.Parts:
                    selectedItemUI.TXT_CarName.text = TextManager.instance.GetText(ETextType.Car, this.info.currentMyCarInfoID);
                    break;
                default:
                    break;
            }
        }

        
        selectedItemUI.TXT_ItemName.text = TextManager.instance.GetText(selectedItemUI.carItemInfo.GetCarItemTextType(), selectedItemUI.carItemInfo.carItemInfoID);
        selectedItemUI.TXT_ItemPrice.text = "(  " + selectedItemUI.carItemInfo.carItemPrice.ToString() + ")";
    }

    private void ResetCoinQunatityText()
    {
        this.TXT_CoinQuantity.text = UserManager.instance.GetUserCoin_1().ToString();
    }

    /* Button Function */

    public void ChangeGoodsUI(int carItemType)
    {
        ECarItemType eCarItemType = (ECarItemType)carItemType;

        if (eCarItemType == this.info.eCurrentCarItemType || eCarItemType == ECarItemType.None)
        {
            return;
        }

        int selectedIndex = (int)eCarItemType - 1;

        for (int i = 0; i < this.goodsUIObject.transform.childCount; i++)
        {
            if (i == selectedIndex)
            {
                this.goodsUIObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                this.goodsUIObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        this.info.eCurrentCarItemType = eCarItemType;
    }

    public void OpenGoods()
    {
        this.goodsObject.SetActive(true);
    }

    public void OpenGoods(int carItemType)
    {
        ECarItemType eCarItemType = (ECarItemType)carItemType;

        if (eCarItemType <= ECarItemType.None || eCarItemType >= ECarItemType.Max)
        {
            eCarItemType = this.info.eCurrentCarItemType;
        }

        this.goodsObject.SetActive(true);

        ChangeGoodsUI((int)eCarItemType);
    }

    public void OpenSelected(int selectedIndex)
    {
        ECarItemType eCarItemType = this.info.eCurrentCarItemType;

        if (eCarItemType == ECarItemType.None)
        {
            return;
        }

        SetActiveOpenSelectedObject(eCarItemType);
        SetSelectedUICarItemInfo(eCarItemType, selectedIndex);
        SetSelectedItemUIImageAndText();
    }

    private void SetActiveOpenSelectedObject(ECarItemType eCarItemType)
    {
        this.goodsObject.SetActive(false);
        this.selectedObject.SetActive(true);

        switch (eCarItemType)
        {
            case ECarItemType.Car:
                this.selectedStats.SetActive(true);
                break;
            case ECarItemType.Paint:
                this.selectedPaint.SetActive(true);
                break;
            case ECarItemType.Parts:
                this.selectedStats.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void CloseSelected()
    {
        this.selectedStats.SetActive(false);
        this.selectedPaint.SetActive(false);
        this.selectedObject.SetActive(false);
    }

    public void SetSelectedUICarItemInfo(ECarItemType eCarItemType, int selectedIndex)
    {
        CarItemUI carItemUI = this.info.GetGoodsItemUI(eCarItemType, selectedIndex);
        this.info.selectedItemUI.carItemInfo = carItemUI.carItemInfo;
        this.info.selectedItemUI.selectedIndex = selectedIndex;
    }

    public void OpenPurchaseAskPopup()
    {
        this.purchaseAskPopupObject.SetActive(true);
        this.purchaseAskPopupObject.transform.Find("Text").GetComponent<Text>().text = string.Format(TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Shop_3), this.info.selectedItemUI.carItemInfo.carItemPrice);
    }

    public void ClosePurchaseAskPopup()
    {
        this.purchaseAskPopupObject.SetActive(false);
    }

    public void PurchaseSelectedCarItem()
    {
        ECarInventoryType eCarInventoryType = InventoryInformation.ConvertTypeCarItemToCarInventory(this.info.eCurrentCarItemType);
        EPurchaseErrorType ePurchaseErrorType;
        
        delegatePurchaseResult delegatePR = new delegatePurchaseResult(ActiveThreadPurchaseResult);
        InActiveThreadPurchaseResult();
        ePurchaseErrorType = ShopManager.instance.PurchaseCarItem(eCarInventoryType, this.info.currentMyCarInfoID, this.info.selectedItemUI.carItemInfo, delegatePR);

        if (ePurchaseErrorType == EPurchaseErrorType.Success)
        {
            StartCoroutine(CoroutineSuccessPurchaseItem());
        }
        else
        {
            PopupManager.instance.OpenCheckPopup(ShopManager.instance.GetPurchaseText(ePurchaseErrorType));
        }
    }

    private void ActiveThreadPurchaseResult(EPurchaseErrorType ePurchaseErrorType)
    {
        this.thread_wait_purchaseResult = true;
        this.thread_ePurchaseErrorType = ePurchaseErrorType;
    }

    private void InActiveThreadPurchaseResult()
    {
        this.thread_wait_purchaseResult = false;
        this.thread_ePurchaseErrorType = EPurchaseErrorType.None;
    }

    private bool GetThreadPurchaseResult()
    {
        return this.thread_wait_purchaseResult;
    }

    private IEnumerator CoroutineSuccessPurchaseItem()
    {
        delegateGetFlag delegateGetFlag = new delegateGetFlag(GetThreadPurchaseResult);
        yield return StartCoroutine(CMDelegate.CoroutineThreadWait(delegateGetFlag));

        if (GetThreadPurchaseResult() == false)
        {
            string errorText = string.Format(TextManager.instance.GetText(ETextType.Game, (int)EGameText.Error), EnumError.GetEGameErrorCodeString(EGameError.ThreadWaitTimeOver));
            PopupManager.instance.OpenCheckPopup(errorText);
            yield break;
        }

        PopupManager.instance.OpenCheckPopup(ShopManager.instance.GetPurchaseText(this.thread_ePurchaseErrorType));

        if (this.thread_ePurchaseErrorType == EPurchaseErrorType.Success)
        {
            CarItemUI selectedCarItemUI = this.info.GetGoodsItemUI(this.info.eCurrentCarItemType, this.info.selectedItemUI.selectedIndex);
            InActiveOwnedCarItemUI(selectedCarItemUI);
            ResetCoinQunatityText();
            CloseSelected();
            OpenGoods((int)this.info.eCurrentCarItemType);
        }
    }
}
