using System.Collections.Generic;
using UnityEngine;

using Services.Enum.Collision;

namespace Services.Game.Collision
{
    public class Booster : Collision
    {
        private const int OriginBoosterSpriteIndex = 0;
        private const int BlinknBoosterSpriteIndex = 1;

        [SerializeField]
        private Enum.Collision.Booster booster;

        [SerializeField]
        private List<Sprite> boosterSprites;

        [SerializeField]
        private SpriteRenderer boosterSpriteRenderer;

        public new Enum.Collision.Booster Collide()
        {
            if (this.boosterSpriteRenderer != null)
            {
                StartCoroutineBlinkSpriteBySprite(this.boosterSpriteRenderer, this.boosterSprites[OriginBoosterSpriteIndex], this.boosterSprites[BlinknBoosterSpriteIndex], 1.5f);
            }

            return this.booster;
        }

        public override void PlaySound()
        {
            int soundIndex;

            switch (this.booster)
            {
                case Enum.Collision.Booster.Booster_1:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Booster_1;
                    break;
                case Enum.Collision.Booster.Booster_2:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Booster_2;
                    break;
                default:
                    soundIndex = (int)Enum.Sound.Effect.Collision.Booster_1;
                    break;
            }

            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Collision, soundIndex);
        }
    }
}