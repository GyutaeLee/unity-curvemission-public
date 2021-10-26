using UnityEngine;

using Services.Enum.Collision;

namespace Services.Game.Collision
{
    public class Wall : Collision
    {
        [SerializeField]
        private Vector3 resetPosition;
        public Vector3 ResetPosition
        {
            get
            {
                return this.resetPosition;
            }
        }

        [SerializeField]
        private Enum.Collision.Wall wall;

        public new Vector3 Collide()
        {
            return this.resetPosition;
        }

        public override void PlaySound()
        {
            int soundIndex = (int)Enum.Sound.Effect.Collision.Rock_1;
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Collision, soundIndex);
        }
    }
}