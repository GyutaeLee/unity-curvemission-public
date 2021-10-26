using UnityEngine;

using Services.Enum.Collision;

namespace Services.Game.Collision
{
    public class Direction : Collision
    {
        [SerializeField]
        private Enum.Collision.Direction direction;

        [SerializeField]
        private SpriteRenderer directionSpriteRenderer;

        public new Enum.Collision.Direction Collide()
        {
            if (this.directionSpriteRenderer != null)
            {
                StartCoroutineBlinkSpriteByColor(this.directionSpriteRenderer, new Color(0.3f, 0.3f, 0.3f, 0.3f), 3.0f);
            }

            return this.direction;
        }
    }
}