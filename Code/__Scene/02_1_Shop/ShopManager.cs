using UnityEngine;

public enum EPurchaseErrorType
{
    None = 0,

    Success = 1,
    Fail = 2,
    NotEnoughCoin = 2,
    AlreadyOwned = 3,

    Max,
}

public class ShopManager : MonoBehaviour
{
    private static ShopManager _instance = null;
    public static ShopManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    private void Awake()
    {
        InitInstance();
    }

    private void InitInstance()
    {
        if (ShopManager.instance == null)
        {
            ShopManager.instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public EPurchaseErrorType PurchaseCarItem(ECarInventoryType eCarInventoryType, int currentMyCarInfoID, ShopItemInfo.CarItemInfo carItemInfo, delegatePurchaseResult delegatePR)
    {
        string textKey = InventoryInformation.GetCarInvetoryTextKey(eCarInventoryType);
        int firstKey = currentMyCarInfoID;
        int secondKey = carItemInfo.carItemInfoID;

        if (eCarInventoryType == ECarInventoryType.Car)
        {
            firstKey = secondKey;
        }

        // 1. 이미 보유하고 있는지 확인
        if (UserManager.instance.GetCarInventory(textKey, firstKey, secondKey) == true)
        {
            return EPurchaseErrorType.AlreadyOwned;
        }

        // 2. 보유하고 있는 coin_1이 아이템 가격보다 많은지 확인
        if (UserManager.instance.GetUserCoin_1() < carItemInfo.carItemPrice)
        {
            return EPurchaseErrorType.NotEnoughCoin;
        }

        // 3. 구매 진행
        ServerManager.instance.PostPurchaseCarItemToFirebaseDB(eCarInventoryType, carItemInfo.carItemPrice * -1, firstKey, secondKey, true, delegatePR);

        return EPurchaseErrorType.Success;
    }

    public string GetPurchaseText(EPurchaseErrorType ePurchaseErrorType)
    {
        string text = "";

        switch (ePurchaseErrorType)
        {
            case EPurchaseErrorType.Success:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Shop_0);
                break;
            case EPurchaseErrorType.AlreadyOwned:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Shop_1);
                break;
            case EPurchaseErrorType.NotEnoughCoin:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Shop_2);
                break;
            default:
                text = string.Format(TextManager.instance.GetText(ETextType.Game, (int)EGameText.Error), EnumError.GetEGameErrorCodeString(EGameError.ShopPurchaseError));
                break;
        }

        return text;
    }
}
