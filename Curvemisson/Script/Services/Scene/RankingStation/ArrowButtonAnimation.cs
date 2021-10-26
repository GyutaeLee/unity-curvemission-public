using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Services.Scene.RankingStation
{
    public class ArrowButtonAnimation : MonoBehaviour
    {
        [SerializeField]
        private Image animationImage;
        [SerializeField]
        private Sprite[] animationSprite;

        [SerializeField]
        private bool isLoop;
        [SerializeField]
        private bool isMoveVertical;

        [SerializeField]
        private float animationMoveHeight;
        [SerializeField]
        private float animationPlayTerm;

        private void Awake()
        {
            if (this.animationImage == null || this.animationSprite.Length == 0)
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            StartCoroutine(CoroutineAnimation());
        }

        private IEnumerator CoroutineAnimation()
        {
            WaitForSeconds WFS = new WaitForSeconds(this.animationPlayTerm);
            int currentAnimationSpriteIndex = 0;
            int moveWeight = -1;

            while (true)
            {
                this.animationImage.sprite = this.animationSprite[currentAnimationSpriteIndex];
                this.animationImage.SetNativeSize();

                if (this.isMoveVertical == true)
                {
                    Vector2 v = new Vector2();

                    v.x = this.animationImage.rectTransform.anchoredPosition.x;
                    v.y = this.animationImage.rectTransform.anchoredPosition.y + moveWeight * this.animationMoveHeight;

                    this.animationImage.rectTransform.anchoredPosition = v;
                    moveWeight *= -1;
                }

                currentAnimationSpriteIndex++;
                if (currentAnimationSpriteIndex == this.animationSprite.Length)
                {
                    if (this.isLoop == false)
                        yield break;

                    currentAnimationSpriteIndex = 0;
                }

                yield return WFS;
            }
        }
    }
}