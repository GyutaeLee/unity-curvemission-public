using Services.Enum.Inventory;
using Services.Enum.Item;

namespace Services.Static
{
    public static class Inventory
    {
        public static string ConvertInventoryTypeToTextKey(InventoryType inventoryType)
        {
            string textKey;

            switch (inventoryType)
            {
                case InventoryType.Avatar:
                    textKey = "security-related";
                    break;
                case InventoryType.Car:
                    textKey = "security-related";
                    break;
                default:
                    textKey = "";
                    break;
            }

            return textKey;
        }
    }

    public static class CarInventory
    {
        public static CarItemType ConvertCarInventroyTypeToCarItemType(Enum.Inventory.CarInventoryType carInventoryType)
        {
            CarItemType carItemType;

            switch (carInventoryType)
            {
                case CarInventoryType.Car:
                    carItemType = CarItemType.Car;
                    break;
                case CarInventoryType.Paint:
                    carItemType = CarItemType.Paint;
                    break;
                case CarInventoryType.Parts:
                    carItemType = CarItemType.Parts;
                    break;
                default:
                    carItemType = CarItemType.None;
                    break;
            }

            return carItemType;
        }

        public static string GetCarInvetoryTypeTextKey(CarInventoryType carInventoryType)
        {
            string textKey;

            switch (carInventoryType)
            {
                case CarInventoryType.Car:
                    textKey = "cars";
                    break;
                case CarInventoryType.Paint:
                    textKey = "paints";
                    break;
                case CarInventoryType.Parts:
                    textKey = "parts";
                    break;
                default:
                    textKey = "";
                    break;
            }

            return textKey;
        }
    }

    public static class AvatarInventory
    {
        public static AvatarItemType ConvertAvatarInventoryTypeToAvatarItemType(Enum.Inventory.AvatarInventoryType avatarInventoryType)
        {
            AvatarItemType avatarItemType;

            switch (avatarInventoryType)
            {
                case AvatarInventoryType.Head:
                    avatarItemType = AvatarItemType.Head;
                    break;
                case AvatarInventoryType.Top:
                    avatarItemType = AvatarItemType.Top;
                    break;
                case AvatarInventoryType.Bottom:
                    avatarItemType = AvatarItemType.Bottom;
                    break;
                default:
                    avatarItemType = AvatarItemType.None;
                    break;
            }

            return avatarItemType;
        }

        public static string GetAvatarInventoryTypeTextKey(Enum.Inventory.AvatarInventoryType avatarInventoryType)
        {
            string textKey;

            switch (avatarInventoryType)
            {
                case AvatarInventoryType.Head:
                    textKey = "heads";
                    break;
                case AvatarInventoryType.Top:
                    textKey = "tops";
                    break;
                case AvatarInventoryType.Bottom:
                    textKey = "bottoms";
                    break;
                default:
                    textKey = "";
                    break;
            }

            return textKey;
        }
    }
}