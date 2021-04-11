public class InventoryInformation
{
    private const int kDefaultCarInfoID = security-related;
    private const int kDefaultPaintInfoID = security-related;
    private const int kDefaultPartsInfoID = security-related;

    public static ECarInventoryType ConvertTypeCarItemToCarInventory(ECarItemType eCarItemType)
    {
        ECarInventoryType eCarInventoryType;

        switch (eCarItemType)
        {
            case ECarItemType.Car:
                eCarInventoryType = ECarInventoryType.Car;
                break;
            case ECarItemType.Paint:
                eCarInventoryType = ECarInventoryType.Paint;
                break;
            case ECarItemType.Parts:
                eCarInventoryType = ECarInventoryType.Parts;
                break;
            default:
                eCarInventoryType = ECarInventoryType.None;
                break;
        }

        return eCarInventoryType;
    }

    public static string GetCarInvetoryTextKey(ECarInventoryType carInventoryType)
    {
        string key = "";

        switch (carInventoryType)
        {
            case ECarInventoryType.Car:
                key = "security-related";
                break;
            case ECarInventoryType.Paint:
                key = "security-related";
                break;
            case ECarInventoryType.Parts:
                key = "security-related";
                break;
        }

        return key;
    }

    public static int GetDefaultCarInfoID()
    {
        return kDefaultCarInfoID;
    }

    public static int GetDefaultPaintInfoID()
    {
        return kDefaultPaintInfoID;
    }

    public static int GetDefaultPartsInfoID()
    {
        return kDefaultPartsInfoID;
    }
}
