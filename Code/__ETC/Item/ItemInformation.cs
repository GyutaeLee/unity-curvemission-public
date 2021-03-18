using UnityEngine;

public enum EItemType
{
    None = 0,

    Car = 1,

    Max,
}

public enum ECarItemType
{
    None = 0,

    Car = 1,
    Paint = 2,
    Parts = 3,

    Max,
}

public class ItemInformation : MonoBehaviour
{
    public static ECarItemType ConvertTypeCarInventroyToCarItem(ECarInventoryType eCarInventoryType)
    {
        ECarItemType eCarItemType;

        switch (eCarInventoryType)
        {
            case ECarInventoryType.Car:
                eCarItemType = ECarItemType.Car;
                break;
            case ECarInventoryType.Paint:
                eCarItemType = ECarItemType.Paint;
                break;
            case ECarInventoryType.Parts:
                eCarItemType = ECarItemType.Parts;
                break;
            default:
                eCarItemType = ECarItemType.None;
                break;
        }
        
        return eCarItemType;
    }
}
