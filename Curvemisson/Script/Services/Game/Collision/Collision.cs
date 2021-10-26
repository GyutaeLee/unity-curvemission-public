using System.Collections;

using UnityEngine;

namespace Services.Game.Collision
{
    public abstract class Collision : MonoBehaviour
    {
        [SerializeField]
        private Enum.Collision.Type type;
        public Enum.Collision.Type Type
        {
            get
            {
                return this.type;
            }
        }

        public virtual void Collide()
        {

        }

        public virtual void PlaySound()
        {

        }

        public void StartCoroutineBlinkSpriteByColor(SpriteRenderer spriteRenderer, Color color, float blinkTerm)
        {
            StartCoroutine(CoroutineBlinkSpriteByColor(spriteRenderer, color, blinkTerm));
        }

        public void StartCoroutineBlinkSpriteBySprite(SpriteRenderer spriteRenderer, Sprite originSprite, Sprite blinkSprite, float blinkTerm)
        {
            StartCoroutine(CoroutineBlinkSpriteBySprite(spriteRenderer, originSprite, blinkSprite, blinkTerm));
        }

        public void StartCoroutineSetActiveGameObject(GameObject gameObject, bool isEnabled, float activeTerm)
        {
            StartCoroutine(CoroutineSetActiveGameObject(gameObject, isEnabled, activeTerm));
        }

        private IEnumerator CoroutineBlinkSpriteByColor(SpriteRenderer spriteRenderer, Color color, float blinkTerm)
        {
            if (spriteRenderer.color == color)
                yield break;

            Color originColor = spriteRenderer.color;

            spriteRenderer.color = color;
            yield return new WaitForSeconds(blinkTerm);
            spriteRenderer.color = originColor;
        }

        private IEnumerator CoroutineBlinkSpriteBySprite(SpriteRenderer spriteRenderer, Sprite originSprite, Sprite blinkSprite, float blinkTerm)
        {
            if (spriteRenderer.sprite == blinkSprite)
                yield break;

            spriteRenderer.sprite = blinkSprite;
            yield return new WaitForSeconds(blinkTerm);
            spriteRenderer.sprite = originSprite;
        }

        private IEnumerator CoroutineSetActiveGameObject(GameObject gameObject, bool isEnable, float activeTerm)
        {
            yield return new WaitForSeconds(activeTerm);

            gameObject.SetActive(isEnable);
        }
    }
}