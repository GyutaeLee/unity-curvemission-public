using UnityEngine;

public enum EPurchaseErrorType
{
    None = 0,

    Success = 1,
    NotEnoughCoin = 2,
    AlreadyOwned = 3,

    Max,
}

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance = null;

    public class ShopInformation
    {

    }

    private ShopInformation info;

    private void Awake()
    {
        InitInstance();

        this.info = new ShopInformation();
    }

    private void Start()
    {
        InitShopManager();   
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

    private void InitShopManager()
    {

    }

    public EPurchaseErrorType PurchaseCarItem(ECarInventoryType eCarInventoryType, int currentMyCarInfoID, ShopCarItemInfo.CarItemInfo carItemInfo)
    {
        string textKey = InventoryInformation.GetCarInvetoryTextKey(eCarInventoryType);
        int firstKey = currentMyCarInfoID;
        int secondKey = carItemInfo.carItemInfoID;

        // 차량은 [carItemInfoID]를 기준으로 통일
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
        // TO DO : firebase를 믿어야하나? -> 중간에 전송 끊기면 어떻게 할지에 대한 대책 필요
        ServerManager.instance.PostUserCoinToFirebaseDB(carItemInfo.carItemPrice * -1);
        ServerManager.instance.PostUserCarInventoryToFirebaseDB(ECarInventoryType.Car, carItemInfo.carItemInfoID, carItemInfo.carItemInfoID, true);

        return EPurchaseErrorType.Success;
    }

    public string GetPurchaseText(EPurchaseErrorType ePurchaseErrorType)
    {
        string text = "";

        switch (ePurchaseErrorType)
        {
            case EPurchaseErrorType.Success:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Text_PurchaseSuccess);
                break;
            case EPurchaseErrorType.AlreadyOwned:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Text_AlreadyPossessed);
                break;
            case EPurchaseErrorType.NotEnoughCoin:
                text = TextManager.instance.GetText(ETextType.Shop, (int)EShopText.Text_NotEnoughCoin);
                break;
            default:
                text = string.Format(TextManager.instance.GetText(ETextType.Game, (int)EGameText.Text_Error), EnumError.GetEGameErrorCode(EGameError.ShopPurchaseError));
                break;
        }

        return text;
    }
}
