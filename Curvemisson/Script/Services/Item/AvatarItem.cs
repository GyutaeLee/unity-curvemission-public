using UnityEngine;
using UnityEngine.UI;

using Services.Useful;

using Services.Enum.Item;
using Services.Enum.GameText;

namespace Services.Item.Avatar
{
    public abstract class AvatarItem
    {
        public AvatarItemType Type { get; protected set; }
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

    public class Head : AvatarItem
    {
        public Head(int headInfoID)
        {
            this.Type = AvatarItemType.Head;
            this.InfoID = headInfoID;
            this.Price = 0;
        }

        public Head(int headInfoID, int price)
        {
            this.Type = AvatarItemType.Head;
            this.InfoID = headInfoID;
            this.Price = price;
        }
    
        public override TextType GetTextType()
        {
            return TextType.AvatarItemHead;
        }

        protected override Sprite GetSprite()
        {
            string avatarItemSpritePath = "Texture/Item/Avatar/Head/Head_0";
            int avatarItemSpriteIndex = this.InfoID - Constants.AvatarItem.DefaultAvatarItemHeadInfoID;

            Sprite[] avatarItemSpriteSheet = Resources.LoadAll<Sprite>(avatarItemSpritePath);
            if (avatarItemSpriteSheet.Length <= avatarItemSpriteIndex)
                return null;

            return avatarItemSpriteSheet[avatarItemSpriteIndex];
        }
    }

    public class Top : AvatarItem
    {
        public Top(int topInfoID)
        {
            this.Type = AvatarItemType.Top;
            this.InfoID = topInfoID;
            this.Price = 0;
        }

        public Top(int topInfoID, int price)
        {
            this.Type = AvatarItemType.Top;
            this.InfoID = topInfoID;
            this.Price = price;
        }

        public override TextType GetTextType()
        {
            return TextType.AvatarItemTop;
        }

        protected override Sprite GetSprite()
        {
            string avatarItemSpritePath = "Texture/Item/Avatar/Top/Top_0";
            int avatarItemSpriteIndex = this.InfoID - Constants.AvatarItem.DefaultAvatarItemTopInfoID;

            Sprite[] avatarItemSpriteSheet = Resources.LoadAll<Sprite>(avatarItemSpritePath);
            if (avatarItemSpriteSheet.Length <= avatarItemSpriteIndex)
                return null;

            return avatarItemSpriteSheet[avatarItemSpriteIndex];
        }
    }

    public class Bottom : AvatarItem
    {
        public Bottom(int bottomInfoID)
        {
            this.Type = AvatarItemType.Bottom;
            this.InfoID = bottomInfoID;
            this.Price = 0;
        }

        public Bottom(int bottomInfoIDInfoID, int price)
        {
            this.Type = AvatarItemType.Bottom;
            this.InfoID = bottomInfoIDInfoID;
            this.Price = price;
        }

        public override TextType GetTextType()
        {
            return TextType.AvatarItemBottom;
        }

        protected override Sprite GetSprite()
        {
            string avatarItemSpritePath = "Texture/Item/Avatar/Bottom/Bottom_0";
            int avatarItemSpriteIndex = this.InfoID - Constants.AvatarItem.DefaultAvatarItemBottomInfoID;

            Sprite[] avatarItemSpriteSheet = Resources.LoadAll<Sprite>(avatarItemSpritePath);
            if (avatarItemSpriteSheet.Length <= avatarItemSpriteIndex)
                return null;

            return avatarItemSpriteSheet[avatarItemSpriteIndex];
        }
    }

    public class AvatarItemUI
    {
        public AvatarItem AvatarItem { get; set; }

        protected Image ItemImage;
        protected UnityEngine.UI.Text ItemText;

        public AvatarItemUI(GameObject gameObject)
        {
            ObjectFinder.FindComponentInAllChild(ref this.ItemImage, gameObject, "ItemImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.ItemText, gameObject, "ItemText", true);
        }

        public void SetImage()
        {
            this.ItemImage.sprite = this.AvatarItem.Sprite;
            this.ItemImage.SetNativeSize();
        }

        public virtual void SetText()
        {
            this.ItemText.text = GameText.Manager.Instance.GetText(this.AvatarItem.GetTextType(), this.AvatarItem.InfoID);
        }
    }
}
