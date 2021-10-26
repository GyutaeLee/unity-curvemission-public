using UnityEngine;

using Services.Enum.Collision;

namespace Services.Game.Collision
{
    public class Obstacle : Collision
    {
        [SerializeField]
        private Enum.Collision.Obstacle obstacle;

        public new Enum.Collision.Obstacle Collide()
        {
            return this.obstacle;
        }

        public override void PlaySound()
        {
            int soundIndex;

            switch (this.obstacle)
            {
                case Enum.Collision.Obstacle.Obstacle_1:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Rock_1;
                    break;
                case Enum.Collision.Obstacle.Obstacle_2:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Rock_1;
                    break;
                default:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Rock_1;
                    break;
            }

            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Collision, soundIndex);
        }
    }
}