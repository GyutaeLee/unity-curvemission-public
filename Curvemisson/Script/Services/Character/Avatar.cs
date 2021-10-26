using UnityEngine;
using UnityEngine.UI;

using Services.Item;
using Services.Enum.Item;

using Services.Useful;

namespace Services.Character
{
    public class Avatar
    {
        public Item.Avatar.Head Head { get; private set; }
        public Item.Avatar.Top Top { get; private set; }
        public Item.Avatar.Bottom Bottom { get; private set; }

        public Avatar()
        {
            this.Head = new Item.Avatar.Head(Constants.AvatarItem.DefaultAvatarItemHeadInfoID);
            this.Top = new Item.Avatar.Top(Constants.AvatarItem.DefaultAvatarItemTopInfoID);
            this.Bottom = new Item.Avatar.Bottom(Constants.AvatarItem.DefaultAvatarItemBottomInfoID);
        }

        public Avatar(Item.Avatar.Head head, Item.Avatar.Top top, Item.Avatar.Bottom bottom)
        {
            this.Head = head;
            this.Top = top;
            this.Bottom = bottom;
        }

        public void ChangeHead(Item.Avatar.Head head)
        {
            this.Head = head;
        }

        public void ChangeTop(Item.Avatar.Top top)
        {
            this.Top = top;
        }

        public void ChangeBottom(Item.Avatar.Bottom bottom)
        {
            this.Bottom = bottom;
        }
    }

    public class AvatarUI
    {
        private Avatar avatar;

        private Image headImage;
        private Image topImage;
        private Image bottomImage;

        public AvatarUI(GameObject gameObject)
        {
            ObjectFinder.FindComponentInAllChild(ref this.headImage, gameObject, "HeadImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.topImage, gameObject, "TopImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.bottomImage, gameObject, "BottomImage", true);
        }

        public AvatarUI(Avatar avatar, GameObject gameObject)
        {
            this.avatar = avatar;

            ObjectFinder.FindComponentInAllChild(ref this.headImage, gameObject, "HeadImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.topImage, gameObject, "TopImage", true);
            ObjectFinder.FindComponentInAllChild(ref this.bottomImage, gameObject, "BottomImage", true);
        }

        public void CreateAvatar(int headInfoID, int topInfoID, int bottomInfoID)
        {
            Item.Avatar.Head head = new Item.Avatar.Head(headInfoID);
            Item.Avatar.Top top = new Item.Avatar.Top(topInfoID);
            Item.Avatar.Bottom bottom = new Item.Avatar.Bottom(bottomInfoID);

            this.avatar = new Avatar(head, top, bottom);
        }

        public void SetImage()
        {
            SetHeadImage();
            SetTopImage();
            SetBottomImage();
        }

        private void SetHeadImage()
        {
            this.headImage.sprite = this.avatar.Head.Sprite;
            this.headImage.SetNativeSize();
        }

        private void SetTopImage()
        {
            this.topImage.sprite = this.avatar.Top.Sprite;
            this.topImage.SetNativeSize();
        }

        private void SetBottomImage()
        {
            this.bottomImage.sprite = this.avatar.Bottom.Sprite;
            this.bottomImage.SetNativeSize();
        }

        public void ChangeAvatar(Avatar avatar)
        {
            if (this.avatar == null)
            {
                this.avatar = new Avatar(avatar.Head, avatar.Top, avatar.Bottom);
            }
            else
            {
                ChangeHead(avatar.Head.InfoID);
                ChangeTop(avatar.Top.InfoID);
                ChangeBottom(avatar.Bottom.InfoID);
            }
        }

        public void ChangeHead(int headInfoID)
        {
            this.avatar.ChangeHead(new Item.Avatar.Head(headInfoID));

            this.headImage.sprite = this.avatar.Head.Sprite;
            this.headImage.SetNativeSize();
        }

        public void ChangeTop(int topInfoID)
        {
            this.avatar.ChangeTop(new Item.Avatar.Top(topInfoID));

            this.topImage.sprite = this.avatar.Top.Sprite;
            this.topImage.SetNativeSize();
        }

        public void ChangeBottom(int bottomInfoID)
        {
            this.avatar.ChangeBottom(new Item.Avatar.Bottom(bottomInfoID));

            this.bottomImage.sprite = this.avatar.Bottom.Sprite;
            this.bottomImage.SetNativeSize();
        }

        public int GetHeadInfoID()
        {
            return this.avatar.Head.InfoID;
        }

        public int GetTopInfoID()
        {
            return this.avatar.Top.InfoID;
        }

        public int GetBottomInfoID()
        {
            return this.avatar.Bottom.InfoID;
        }
    }
}