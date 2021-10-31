using System;
using System.Collections;
using UnityEngine;

namespace Services.Scene.Village
{
    public class Village : MonoBehaviour
    {
        private static class Constants
        {
            public const float VillageWidth = 180.0f;
        }

        [SerializeField]
        private float villageMoveSpeed = 3.0f;

        [SerializeField]
        private RectTransform villageRectTransform;

        private bool isVillageMove;

        private void Start()
        {
            Sound.Bgm.Manager.Instance.Play(Enum.Sound.Bgm.Type.Village, (int)Enum.Sound.Bgm.Village.Village);
            CheckAndSetVillagePosition();
        }

        static readonly string[] RightVillageSceneNames = { Services.Constants.SceneName.RankingStation, Services.Constants.SceneName.Shop, Services.Constants.SceneName.StageSelection };
        private void CheckAndSetVillagePosition()
        {
            for (int i = 0; i < RightVillageSceneNames.Length; i++)
            {
                if (User.User.Instance.BeforeSceneName == RightVillageSceneNames[i])
                {
                    SetVillagePositionToRight();
                    break;
                }
            }
        }

        private void SetVillagePositionToRight()
        {
            Vector2 v = new Vector2();

            v.x = this.villageRectTransform.anchoredPosition.x - Constants.VillageWidth;
            v.y = this.villageRectTransform.anchoredPosition.y;

            this.villageRectTransform.anchoredPosition = v;
        }
        public void ClickMoveVillageToRightButton()
        {
            StartCoroutine(CoroutineMoveVillages(false));
        }

        public void ClickMoveVillageToLeftButton()
        {
            StartCoroutine(CoroutineMoveVillages(true));
        }

        private bool IsValidVillagesPosition()
        {
            const float VillageInitialXposition = 0.0f;
            if (this.villageRectTransform.anchoredPosition.x > VillageInitialXposition)
                return false;

            if (this.villageRectTransform.anchoredPosition.x < -Constants.VillageWidth)
                return false;

            return true;
        }

        private IEnumerator CoroutineMoveVillages(bool isLeftMove)
        {
            if (this.villageRectTransform == null || this.isVillageMove == true)
            {
                yield break;
            }

            this.isVillageMove = true;

            float moveWeight = (isLeftMove == true) ? 1.0f : -1.0f;
            moveWeight *= this.villageMoveSpeed;

            float currentTravelDistance = 0.0f;
            float targetTravelDistance = Constants.VillageWidth;

            Vector2 v = Vector2.zero;
            WaitForSeconds WFS = new WaitForSeconds(0.01f);
            while (Math.Abs(currentTravelDistance) < targetTravelDistance)
            {
                v.x = this.villageRectTransform.anchoredPosition.x + moveWeight;
                v.y = this.villageRectTransform.anchoredPosition.y;
                this.villageRectTransform.anchoredPosition = v;

                currentTravelDistance += moveWeight;

                if (IsValidVillagesPosition() == false)
                {
                    yield break;
                }
                else
                {
                    yield return WFS;
                }
            }

            this.villageRectTransform.anchoredPosition = MoveRemainingDistanceInError(this.villageRectTransform.anchoredPosition, isLeftMove, currentTravelDistance, targetTravelDistance);
            this.isVillageMove = false;
        }

        private Vector2 MoveRemainingDistanceInError(Vector2 anchoredPosition, bool isLeftMove, float currentTravelDistance, float targetTravelDistance)
        {
            float lastWeight = Math.Abs(currentTravelDistance) - targetTravelDistance;
            lastWeight *= (isLeftMove == true) ? -1.0f : 1.0f;

            Vector2 v = new Vector2();
            v.x = anchoredPosition.x + lastWeight;
            v.y = anchoredPosition.y;

            return v;
        }
    }
}