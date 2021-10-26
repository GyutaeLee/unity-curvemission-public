using UnityEngine;

using Services.Enum.Inventory;
using Services.Enum.Item;

namespace Services.Static
{
    public static class CarItem
    {
        public static Enum.Inventory.CarInventoryType ConvertCarItemTypeToCarInventoryType(CarItemType carItemType)
        {
            Enum.Inventory.CarInventoryType carInventoryType;

            switch (carItemType)
            {
                case CarItemType.Car:
                    carInventoryType = Enum.Inventory.CarInventoryType.Car;
                    break;
                case CarItemType.Paint:
                    carInventoryType = Enum.Inventory.CarInventoryType.Paint;
                    break;
                case CarItemType.Parts:
                    carInventoryType = Enum.Inventory.CarInventoryType.Parts;
                    break;
                default:
                    carInventoryType = Enum.Inventory.CarInventoryType.None;
                    break;
            }

            return carInventoryType;
        }

    }

    public static class AvatarItem
    {
        public static Enum.Inventory.AvatarInventoryType ConvertAvatarItemTypeToAvatarInventoryType(AvatarItemType avatarItemType)
        {
            Enum.Inventory.AvatarInventoryType avatarInventoryType;

            switch (avatarItemType)
            {
                case AvatarItemType.Head:
                    avatarInventoryType = Enum.Inventory.AvatarInventoryType.Head;
                    break;
                case AvatarItemType.Top:
                    avatarInventoryType = Enum.Inventory.AvatarInventoryType.Top;
                    break;
                case AvatarItemType.Bottom:
                    avatarInventoryType = Enum.Inventory.AvatarInventoryType.Bottom;
                    break;
                default:
                    avatarInventoryType = Enum.Inventory.AvatarInventoryType.None;
                    break;
            }

            return avatarInventoryType;
        }

        public static Sprite GetAvatarItemSprite(AvatarItemType avatarItemType, int avatarItemInfoID)
        {
            string avatarItemSpritePath;
            int avatarItemSpriteIndex;

            switch (avatarItemType)
            {
                case AvatarItemType.Head:
                    avatarItemSpritePath = "Texture/Item/Avatar/Head/Head_0";
                    avatarItemSpriteIndex = avatarItemInfoID - Constants.AvatarItem.DefaultAvatarItemHeadInfoID;
                    break;
                case AvatarItemType.Top:
                    avatarItemSpritePath = "Texture/Item/Avatar/Top/Top_0";
                    avatarItemSpriteIndex = avatarItemInfoID - Constants.AvatarItem.DefaultAvatarItemTopInfoID;
                    break;
                case AvatarItemType.Bottom:
                    avatarItemSpritePath = "Texture/Item/Avatar/Bottom/Bottom_0";
                    avatarItemSpriteIndex = avatarItemInfoID - Constants.AvatarItem.DefaultAvatarItemBottomInfoID;
                    break;
                default:
                    return null;
            }

            Sprite[] avatarItemSpriteSheet = Resources.LoadAll<Sprite>(avatarItemSpritePath);
            if (avatarItemSpriteSheet.Length <= avatarItemSpriteIndex)
            {
                return null;
            }

            return avatarItemSpriteSheet[avatarItemSpriteIndex];
        }
    }
}