using UnityEngine;

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
