using UnityEngine;
using UnityEngine.UI;

using Services.Useful;

using Services.Enum.Item;
using Services.Enum.GameText;

namespace Services.Item.Vehicle
{
    public abstract class CarItem
    {
        public CarItemType Type { get; protected set; }
        public int InfoID { get; protected set; }
        public int Price { get; protected set; }

        private Sprite sprite;
        public Sprite Sprite
        { 
            get
            {
                if (this.sprite == null)
                {
                    this.sprite = GetSprite();
                }
                return this.sprite;
            }
        }

        public abstract TextType GetTextType();
        protected abstract Sprite GetSprite();
    }

    public class Car : CarItem
    {
        public Car(int carInfoID)
        {
            this.Type = CarItemType.Car;

            this.InfoID = carInfoID;
            this.Price = 0;
        }

        public Car(int carInfoID, int price)
        {
            this.Type = CarItemType.Car;

            this.InfoID = carInfoID;
            this.Price = price;
        }
        public override TextType GetTextType()
        {
            return TextType.CarItemCar;
        }

        protected override Sprite GetSprite()
        {
            string carItemSpritePath = "Texture/Item/Car/Car/" + this.InfoID + "/" + Constants.CarItem.DefaultCarItemPaintInfoID;
            int carItemSpriteIndex = Constants.CarItem.UISpriteIndex;

            Sprite[] carItemSpriteSheet = Resources.LoadAll<Sprite>(carItemSpritePath);
            if (carItemSpriteSheet.Length <= carItemSpriteIndex)
                return null;

            return carItemSpriteSheet[carItemSpriteIndex];
        }
    }

    public class Paint : CarItem
    {
        public Paint(int paintInfoID)
        {
            this.Type = CarItemType.Paint;

            this.InfoID = paintInfoID;
            this.Price = 0;
        }

        public Paint(int paintInfoID, int carItemPrice)
        {
            this.Type = CarItemType.Paint;

            this.InfoID = paintInfoID;
            this.Price = carItemPrice;
        }
        public override TextType GetTextType()
        {
            return TextType.CarItemPaint;
        }

        protected override Sprite GetSprite()
        {
            string carItemSpritePath = "Texture/Item/Car/Paint/" + Constants.CarItem.DefaultCarItemCarInfoID;
            int carItemSpriteIndex = this.InfoID - Constants.CarItem.DefaultCarItemPaintInfoID;

            Sprite[] carItemSpriteSheet = Resources.LoadAll<Sprite>(carItemSpritePath);
            if (carItemSpriteSheet.Length <= carItemSpriteIndex)
                return null;

            return carItemSpriteSheet[carItemSpriteIndex];
        }
    }

    public class Parts : CarItem
    {
        public Parts(int partsInfoID)
        {
            this.Type = CarItemType.Paint;

            this.InfoID = partsInfoID;
            this.Price = 0;
        }

        public Parts(int partsInfoID, int carItemPrice)
        {
            this.Type = CarItemType.Paint;

            this.InfoID = partsInfoID;
            this.Price = carItemPrice;
        }
        public override TextType GetTextType()
        {
            return TextType.CarItemParts;
        }

        protected override Sprite GetSprite()
        {
            string carItemSpritePath = "Texture/Item/Car/Parts/Parts_0";
            int carItemSpriteIndex = this.InfoID - Constants.CarItem.DefaultCarItemPartsInfoID;

            Sprite[] carItemSpriteSheet = Resources.LoadAll<Sprite>(carItemSpritePath);
            if (carItemSpriteSheet.Length <= carItemSpriteIndex)
                return null;

            return carItemSpriteSheet[carItemSpriteIndex];
        }
    }

    public class CarItemUI
    {
        public CarItem CarItem { get; set; }

        protected Image ItemImage;
        protected UnityEngine.UI.Text ItemText;

        public CarItemUI(GameObject gameObject)
        {
            ObjectFinder.FindComponentInAllChild(ref this.ItemImage, gameObject, "ItemImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.ItemText, gameObject, "ItemText", true);
        }

        public void SetImage()
        {
            this.ItemImage.sprite = this.CarItem.Sprite;
            this.ItemImage.SetNativeSize();
        }

        public virtual void SetText()
        {
            this.ItemText.text = GameText.Manager.Instance.GetText(this.CarItem.GetTextType(), this.CarItem.InfoID);
        }
    }
}