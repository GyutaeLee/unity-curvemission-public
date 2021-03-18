public enum EInventoryType
{
    None = 0,

    Avatar = 1,
    Car = 2,

    Max,
}

public enum EAvatarInventoryType
{
    None = 0,

    Hair = 1,
    Head = 2,
    Earring = 3,
    Face = 4,
    Top = 5,
    Hand = 6,
    Bottom = 7,
    Shoe = 8,

    Max,
}

public enum ECarInventoryType
{
    None = 0,

    Car = 1,
    Paint = 2,
    Parts = 3,

    Max,
}

public class InventoryInformation
{
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
}
