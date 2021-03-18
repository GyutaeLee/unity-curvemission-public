using System;
using UnityEngine;

public class ShopCarItemInfo : MonoBehaviour
{
    [Serializable]
    public class CarItemInfo
    {
        public ECarItemType eCarItemType;

        public int currentCarID;
        public int carItemInfoID;
        public int carItemPrice;

        public string GetCarItemSpriteName()
        {
            string spriteName = "";

            switch (this.eCarItemType)
            {
                case ECarItemType.Car:
                    spriteName = "security-related" + this.carItemInfoID + "/" + "security-related";
                    break;
                case ECarItemType.Paint:
                    spriteName = "security-related" + this.currentCarID;
                    break;
                case ECarItemType.Parts:
                    spriteName = "security-related";
                    break;
                default:
                    break;
            }

            return spriteName;
        }

        public ETextType GetCarItemTextType()
        {
            ETextType eTextType;

            switch (this.eCarItemType)
            {
                case ECarItemType.Car:
                    eTextType = ETextType.Car;
                    break;
                case ECarItemType.Paint:
                    eTextType = ETextType.Paint;
                    break;
                case ECarItemType.Parts:
                    eTextType = ETextType.Parts;
                    break;
                default:
                    eTextType = ETextType.Car;
                    break;
            }

            return eTextType;
        }
    }

    public CarItemInfo info;
}
