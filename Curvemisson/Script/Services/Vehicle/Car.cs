using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Services.Useful;

namespace Services.Vehicle
{
    public class Car
    {
        private Item.Vehicle.Car car;
        private Item.Vehicle.Paint paint;
        private List<Item.Vehicle.Parts> parts;

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

        public int CarInfoID
        {
            get
            {
                return this.car.InfoID;
            }
        }

        public int PaintInfoID
        {
            get
            {
                return this.paint.InfoID;
            }
        }

        public Car(Item.Vehicle.Car car, Item.Vehicle.Paint paint)
        {
            this.car = car;
            this.paint = paint;
            this.parts = null;
        }

        public Car(Item.Vehicle.Car car, Item.Vehicle.Paint paint, List<Item.Vehicle.Parts> parts)
        {
            this.car = car;
            this.paint = paint;
            this.parts = parts;
        }

        private Sprite GetSprite()
        {
            string carItemSpritePath = "Texture/Item/Car/Car/" + this.CarInfoID + "/" + this.PaintInfoID;
            int carItemSpriteIndex = Constants.CarItem.UISpriteIndex;

            Sprite[] carItemSpriteSheet = Resources.LoadAll<Sprite>(carItemSpritePath);
            if (carItemSpriteSheet.Length <= carItemSpriteIndex)
                return null;

            return carItemSpriteSheet[carItemSpriteIndex];
        }

        public void Change(Item.Vehicle.Car car, Item.Vehicle.Paint paint, List<Item.Vehicle.Parts> parts)
        {
            if (car != null)
            {
                this.car = car;
            }

            if (paint != null)
            {
                this.paint = paint;
            }
            else
            {
                this.paint = new Item.Vehicle.Paint(Constants.CarItem.DefaultCarItemPaintInfoID);
            }

            if (parts != null)
            {
                this.parts = parts;
            }

            this.sprite = GetSprite();
        }
    }

    public class CarUI
    {
        public Vehicle.Car Car { get; set; }

        private Image carImage;
        private UnityEngine.UI.Text carText;

        public CarUI(Vehicle.Car car, GameObject gameObject)
        {
            this.Car = car;

            ObjectFinder.FindComponentInAllChild(ref this.carImage, gameObject, "CarImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.carText, gameObject, "CarText", true);
        }

        public void SetImage()
        {
            this.carImage.sprite = this.Car.Sprite;
            this.carImage.SetNativeSize();
        }

        public void SetText()
        {
            this.carText.text = GameText.Manager.Instance.GetText(Enum.GameText.TextType.CarItemCar, this.Car.CarInfoID);
        }
    }

}