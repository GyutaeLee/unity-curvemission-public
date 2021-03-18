using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopUICarItemManager : MonoBehaviour, ICMInterface
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

        public List<List<CarItemUI>> goodsItemUI;
        public CarItemUI selectedItemUI;

        public string GetMyCarItemSpriteName()
        {
            return "security-related" + this.currentMyCarInfoID + "/" + currentMyCarPaintID;
        }
    }

    public class CarItemUI
    {
        public ShopCarItemInfo.CarItemInfo carItemInfo;

        public bool isPurchasable;

        public Button BTN_Item;
        public Image IMG_Item;
        public Text TXT_Name;
        public Text TXT_Price;

        public Sprite SPT_Item;
    }

    //////////////////
    // end of class //
    //////////////////

    private ShopUICarItemInformation info;

    private CarItemUI myCarUI;

    private GameObject shopGoodsObject;
    private GameObject shopSelectedObject;

    private GameObject shopGoodsUIObject;
    private GameObject shopSelectedUIObject;

    private GameObject purchaseAskPopupObject;

    private Text TXT_CoinQuantity;

    private void Start()
    {
        PrepareBaseObjects();
        InitShopCarItemManager();
    }

    private void Update()
    {

    }

    public void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");
        GameObject shopCarItemPopup = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "shopCarItemPopup", true);

        GameObject myCarUI = CMObjectManager.FindGameObjectInAllChild(shopCarItemPopup, "myCarUI", true);

        GameObject shopGoods = CMObjectManager.FindGameObjectInAllChild(shopCarItemPopup, "shopGoods", true);
        GameObject shopSelected = CMObjectManager.FindGameObjectInAllChild(shopCarItemPopup, "shopSelected", true);

        GameObject shopGoodsUI = CMObjectManager.FindGameObjectInAllChild(shopGoods, "shopGoodsUI", true);
        GameObject shopSelectedUI = CMObjectManager.FindGameObjectInAllChild(shopSelected, "shopSelectedUI", true);

        this.shopGoodsObject = shopGoods;
        this.shopGoodsUIObject = shopGoodsUI;

        this.shopSelectedObject = shopSelected;
        this.shopSelectedUIObject = shopSelectedUI;

        /* My Car UI */
        this.myCarUI = new CarItemUI();

        if (this.myCarUI.IMG_Item == null)
        {
            this.myCarUI.IMG_Item = CMObjectManager.FindGameObjectInAllChild(myCarUI, "IMG_Item", true).GetComponent<Image>();
        }

        if (this.myCarUI.TXT_Name == null)
        {
            this.myCarUI.TXT_Name = CMObjectManager.FindGameObjectInAllChild(myCarUI, "TXT_Name", true).GetComponent<Text>();
        }

        if (this.TXT_CoinQuantity == null)
        {
            this.TXT_CoinQuantity = CMObjectManager.FindGameObjectInAllChild(shopCarItemPopup, "TXT_CoinQuantity", true).GetComponent<Text>();
        }

        /* Shop UI */
        this.info = new ShopUICarItemInformation();

        // popup
        if (this.purchaseAskPopupObject == null)
        {
            this.purchaseAskPopupObject = CMObjectManager.FindGameObjectInAllChild(shopSelected, "purchaseAskPopup", true);
        }

        // shopSelectedUI
        this.info.selectedItemUI = new CarItemUI();

        this.info.selectedItemUI.IMG_Item = this.shopSelectedUIObject.transform.Find("IMG_Item").GetComponent<Image>();
        this.info.selectedItemUI.TXT_Name = this.shopSelectedUIObject.transform.Find("TXT_Name").GetComponent<Text>();

        GameObject shopSelectedButton = CMObjectManager.FindGameObjectInAllChild(this.shopSelectedObject, "Button", true);
        this.info.selectedItemUI.TXT_Price = shopSelectedButton.transform.Find("Purchase").transform.Find("TXT_Price").GetComponent<Text>();

    }

    private void InitShopCarItemManager()
    {
        /* init ShopCarItemInformation */
        this.info.eCurrentCarItemType = ECarItemType.Car;
        this.info.currentMyCarInfoID = SecurityPlayerPrefs.GetInt("security-related", 0);
        this.info.currentMyCarPaintID = SecurityPlayerPrefs.GetInt("security-related", 0);

        /* init UI */
        // my car
        this.myCarUI.SPT_Item = Resources.LoadAll<Sprite>(this.info.GetMyCarItemSpriteName())[kUICarSpriteIndex];
        this.myCarUI.IMG_Item.sprite = this.myCarUI.SPT_Item;
        this.myCarUI.TXT_Name.text = TextManager.instance.GetText(ETextType.Car, this.info.currentMyCarInfoID);

        // shopGoodsUI
        SetShopGoodsItemInfo();
        SetShopGoodsItemUI();

        // coin
        ResetCoinQunatityText();
    }


    /* prepare & setting function */

    private void SetShopGoodsItemInfo()
    {
        this.info.goodsItemUI = new List<List<CarItemUI>>();

        for (int i = 0; i < this.shopGoodsUIObject.transform.childCount; i++)
        {
            List<CarItemUI> tList = new List<CarItemUI>();
            Transform child = this.shopGoodsUIObject.transform.GetChild(i);

            for (int j = 0; j < child.childCount; j++)
            {
                Transform grandson = child.GetChild(j);

                // info
                ECarItemType eCarItemType = (ECarItemType)(i + 1);
                ShopCsvReader.ShopCarItemCommonInformation commonInfo = ShopCsvReader.instance.GetItemInfo(eCarItemType, j);

                if (commonInfo == null)
                {
                    continue;
                }

                CarItemUI iUI = new CarItemUI();

                iUI.carItemInfo = grandson.GetComponent<ShopCarItemInfo>().info;
                iUI.carItemInfo.eCarItemType = commonInfo.eCarItemType;
                iUI.carItemInfo.currentCarID = this.info.currentMyCarInfoID;
                iUI.carItemInfo.carItemInfoID = commonInfo.carItemInfoID;
                iUI.carItemInfo.carItemPrice = commonInfo.carItemPrice;

                // ui
                iUI.BTN_Item = grandson.GetComponent<Button>();
                iUI.IMG_Item = grandson.Find("IMG_Item").GetComponent<Image>();
                iUI.TXT_Name = grandson.Find("TXT_Name").GetComponent<Text>();
                iUI.TXT_Price = grandson.Find("TXT_Price").GetComponent<Text>();

                tList.Add(iUI);
            }

            this.info.goodsItemUI.Add(tList);
        }
    }

    private void SetShopGoodsItemUI()
    {
        for (int i = 0; i < this.info.goodsItemUI.Count; i++)
        {
            ECarInventoryType eCarInventoryType = (ECarInventoryType)(i + 1);
            string textKey = InventoryInformation.GetCarInvetoryTextKey(eCarInventoryType);

            for (int j = 0; j < this.info.goodsItemUI[i].Count; j++)
            {
                CarItemUI copyCarItemUI = this.info.goodsItemUI[i][j];
                int carItemKey = copyCarItemUI.carItemInfo.carItemInfoID;

                // image
                SetCarItemUIImage(ref copyCarItemUI);

                // text
                copyCarItemUI.TXT_Name.text = TextManager.instance.GetText(copyCarItemUI.carItemInfo.GetCarItemTextType(), copyCarItemUI.carItemInfo.carItemInfoID);
                copyCarItemUI.TXT_Price.text = copyCarItemUI.carItemInfo.carItemPrice.ToString();

                if (UserManager.instance.GetCarInventory(textKey, carItemKey, copyCarItemUI.carItemInfo.carItemInfoID) == true)
                {
                    copyCarItemUI.isPurchasable = false;
                    copyCarItemUI.BTN_Item.targetGraphic.enabled = false;
                    copyCarItemUI.BTN_Item.enabled = false;
                }
            }
        }
    }


    private void SetCarItemUIImage(ref CarItemUI carItemUI)
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

    /* */

    private void ResetCoinQunatityText()
    {
        this.TXT_CoinQuantity.text = UserManager.instance.GetUserCoin_1().ToString();
    }

    public void ChangeShopGoodsUI(int carItemType)
    {
        ECarItemType eCarItemType = (ECarItemType)carItemType;

        if (eCarItemType == this.info.eCurrentCarItemType || eCarItemType == ECarItemType.None)
        {
            return;
        }

        int selectedIndex = (int)eCarItemType - 1;

        for (int i = 0; i < this.shopGoodsUIObject.transform.childCount; i++)
        {
            if (i == selectedIndex)
            {
                this.shopGoodsUIObject.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                this.shopGoodsUIObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        this.info.eCurrentCarItemType = eCarItemType;
    }

    public void OpenShopGoodsUI(int carItemType)
    {
        ECarItemType eCarItemType = (ECarItemType)carItemType;

        if (eCarItemType <= ECarItemType.None || eCarItemType >= ECarItemType.Max)
        {
            eCarItemType = this.info.eCurrentCarItemType;
        }

        // on & off
        this.shopGoodsObject.SetActive(true);
        this.shopSelectedObject.SetActive(false);

        this.info.selectedItemUI.carItemInfo = null;

        ChangeShopGoodsUI((int)eCarItemType);
    }

    public void OpenShopSelectedUI(int selectedIndex)
    {
        ECarItemType eCarItemType = this.info.eCurrentCarItemType;

        if (eCarItemType == ECarItemType.None)
        {
            return;
        }

        // on & off
        this.shopGoodsObject.SetActive(false);
        this.shopSelectedObject.SetActive(true);

        // copy info
        this.info.selectedItemUI.carItemInfo = this.info.goodsItemUI[(int)eCarItemType - 1][selectedIndex].carItemInfo;

        // image
        SetCarItemUIImage(ref this.info.selectedItemUI);

        // text
        this.info.selectedItemUI.TXT_Name.text = TextManager.instance.GetText(this.info.selectedItemUI.carItemInfo.GetCarItemTextType(), this.info.selectedItemUI.carItemInfo.carItemInfoID);
        this.info.selectedItemUI.TXT_Price.text = "(  " + this.info.selectedItemUI.carItemInfo.carItemPrice.ToString() + ")";
    }

    public void OpenPurchaseAskPopup()
    {
        this.purchaseAskPopupObject.SetActive(true);
        this.purchaseAskPopupObject.transform.Find("Text").GetComponent<Text>().text = this.info.selectedItemUI.carItemInfo.carItemPrice + " 코인으로\n구매하시겠습니까?"; // TO DO : 다국어 적용
    }

    public void ClosePurchaseAskPopup()
    {
        this.purchaseAskPopupObject.SetActive(false);
    }

    /* 구매 관련 */

    public void PurchaseSelectedCarItem()
    {
        ECarInventoryType eCarInventoryType = InventoryInformation.ConvertTypeCarItemToCarInventory(this.info.eCurrentCarItemType);
        EPurchaseErrorType ePurchaseErrorType;

        ePurchaseErrorType = ShopManager.instance.PurchaseCarItem(eCarInventoryType, this.info.currentMyCarInfoID, this.info.selectedItemUI.carItemInfo);

        PopupManager.instance.OpenCheckPopup(ShopManager.instance.GetPurchaseText(ePurchaseErrorType));

        if (ePurchaseErrorType == EPurchaseErrorType.Success)
        {
           ResetCoinQunatityText();
        }
    }
}
