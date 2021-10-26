using UnityEngine;
using System.Collections.Generic;

using Services.Item.Vehicle;
using Services.Item.Avatar;
using Services.Enum.Inventory;
using Services.Enum.Shop;

using Services.Delegate;

namespace Services.Scene.Shop
{
    public class Main : Util.Singleton<Main>
    {
        [SerializeField]
        private GameObject carItemShopObject;
        [SerializeField]
        private GameObject avatarItemShopObject;

        public void OpenCarItemShop()
        {
            CarItemShop carItemShop = this.carItemShopObject.AddComponent<CarItemShop>();
            carItemShop.Open();
        }

        public void OpenAvatarItemShop()
        {
            AvatarItemShop avatarItemShop = this.avatarItemShopObject.AddComponent<AvatarItemShop>();
            avatarItemShop.Open();
        }

        public PurchaseResultType PurchaseCarItem(CarInventoryType carInventoryType, Item.Vehicle.Car carItemCar, CarItem carItem, delegatePurchaseResult delegatePR)
        {
            if (User.User.Instance.IsOwnedCarItemInCarInventory(carItemCar, carItem) == true)
                return PurchaseResultType.AlreadyOwned;

            if (User.User.Instance.IsUserHaveEnoughCoin_1(carItem.Price) == false)
                return PurchaseResultType.NotEnoughCoin;

            int carItemInfoID = (carInventoryType == CarInventoryType.Car) ? Constants.CarItem.DefaultCarItemPaintInfoID : carItem.InfoID;
            Server.Poster.PostPurchaseCarItemToFirebaseDB(carInventoryType, carItem.Price, carItemCar.InfoID, carItemInfoID, delegatePR);

            return PurchaseResultType.Success;
        }

        public PurchaseResultType PurchaseAvatarItem(List<AvatarItem> avatarItemList, delegatePurchaseResult delegatePR)
        {
            for (int i = 0; i < avatarItemList.Count; i++)
            {
                if (User.User.Instance.IsOwnedAvatarItemInAvatarInventory(avatarItemList[i]) == true)
                    return PurchaseResultType.AlreadyOwned;                
            }

            int avatarItemPriceSum = 0;
            for (int i = 0; i < avatarItemList.Count; i++)
            {
                avatarItemPriceSum += avatarItemList[i].Price;
            }

            if (User.User.Instance.IsUserHaveEnoughCoin_1(avatarItemPriceSum) == false)
                return PurchaseResultType.NotEnoughCoin;

            Server.Poster.PostPurchaseAvatarItemToFirebaseDB(avatarItemList, delegatePR);

            return PurchaseResultType.Success;
        }

        public string GetPurchaseResultText(PurchaseResultType purchaseResultType)
        {
            string text;

            switch (purchaseResultType)
            {
                case PurchaseResultType.Success:
                    text = GameText.Manager.Instance.GetText(Enum.GameText.TextType.Shop, (int)Enum.GameText.Shop.Shop_0);
                    break;
                case PurchaseResultType.AlreadyOwned:
                    text = GameText.Manager.Instance.GetText(Enum.GameText.TextType.Shop, (int)Enum.GameText.Shop.Shop_1);
                    break;
                case PurchaseResultType.NotEnoughCoin:
                    text = GameText.Manager.Instance.GetText(Enum.GameText.TextType.Shop, (int)Enum.GameText.Shop.Shop_2);
                    break;
                default:
                    text = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Game, (int)Enum.GameText.Game.Error), Enum.Error.GameError.ShopPurchaseError);
                    break;
            }

            return text;
        }
    }
}