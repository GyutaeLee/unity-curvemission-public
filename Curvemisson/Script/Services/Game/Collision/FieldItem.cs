using UnityEngine;

namespace Services.Game.Collision
{
    public class FieldItem : Collision
    {
        public System.Action collideAction { set; private get; }

        [SerializeField]
        private Enum.Collision.FieldItem fieldItem;

        [SerializeField]
        private Animator animator;

        [SerializeField]
        private int quantity;

        public override void Collide()
        {
            switch (this.fieldItem)
            {
                case Enum.Collision.FieldItem.Coin:
                    this.animator.SetBool("isCollide", true);
                    StartCoroutineSetActiveGameObject(this.gameObject, false, 0.8f);
                    break;
                default:
                    break;
            }

            collideAction();
        }

        public override void PlaySound()
        {
            int soundIndex = (int)Enum.Sound.Effect.Collision.Coin_1;
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Collision, soundIndex);
        }
    }
}