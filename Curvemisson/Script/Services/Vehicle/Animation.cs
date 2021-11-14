using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Services.Enum.Vehicle;

namespace Services.Vehicle
{
    public class Animation : MonoBehaviour
    {
        public int CarItemCarInfoID { get; private set; }
        public int CarItemPaintInfoID { get; private set; }
        private Dictionary<string, Sprite> carSpriteSheet;

        private bool isBoosterAnimationEnabled;
        private int currentBoosterAnimationIndex;

        private float collisionAnimationTerm = 0.2f;
        private int collisionAnimationRepeatCount = 5;

        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private Animator animator;

        private List<List<GameObject>> boosterObject;

        private void Start()
        {
            if (Static.Replay.IsReplayMode == false)
            {
                InitializeAnimation(User.User.Instance.CurrentCar.CarInfoID, User.User.Instance.CurrentCar.PaintInfoID);
            }
        }

        private void LateUpdate()
        {
            LateUpdateAnimationSprite();
            UpdateAnimationSpeed(1.0f);
        }

        public void InitializeAnimation(int carItemCarInfoID, int carItemPaintInfoID)
        {
            this.CarItemCarInfoID = carItemCarInfoID;
            this.CarItemPaintInfoID = carItemPaintInfoID;
            LoadAnimationResource(this.CarItemCarInfoID, this.CarItemPaintInfoID);

            SetBoosterObjects();

            this.collisionAnimationTerm = 0.2f;
            this.collisionAnimationRepeatCount = 5;
        }

        // reference : https://www.erikmoberg.net/article/unity3d-replace-sprite-programmatically-in-animation
        private void LoadAnimationResource(int carItemCarInfoID, int carItemPaintInfoID)
        {
            string carSpriteSheetName = "Texture/Item/Car/Car/" + carItemCarInfoID + "/" + carItemPaintInfoID;

            // Load the sprites from a sprite sheet file (png).
            var sprites = Resources.LoadAll<Sprite>(carSpriteSheetName);
            this.carSpriteSheet = sprites.ToDictionary(x => x.name, x => x);
        }

        private void SetBoosterObjects()
        {
            this.boosterObject = new List<List<GameObject>>();
            GameObject boosterObject = Useful.ObjectFinder.GetGameObjectInAllChild(this.gameObject, "BoosterObject", true);
            for (int i = 0; i < boosterObject.transform.childCount; i++)
            {
                List<GameObject> boosterObjects = new List<GameObject>();
                Transform boosterLevelObjectTransform = boosterObject.transform.GetChild(i);
                for (int j = 0; j < boosterLevelObjectTransform.childCount; j++)
                {
                    boosterObjects.Add(boosterLevelObjectTransform.GetChild(j).gameObject);
                }
                this.boosterObject.Add(boosterObjects);
            }
        }

        public void SetAnimationState(VehicleState vehicleState)
        {
            this.animator.SetInteger("vehicleState", (int)vehicleState);
        }

        public void UpdateAnimationSpeed(float animationSpeed)
        {   
            if (Static.Game.IsGameProceeding() == false)
            {
                this.animator.speed = 0;
                return;
            }

            if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Move") == true)
            {
                // TODO : 0 ~ 3 사이가 애니메이션 속도가 적당하다?
                this.animator.speed = animationSpeed;
            }
            else if (this.animator.GetCurrentAnimatorStateInfo(0).IsTag("Drift") == true)
            {
                this.animator.speed = 1.0f;
            }
            else
            {
                this.animator.speed = 1.0f;
            }
        }

        private void LateUpdateAnimationSprite()
        {
            // Swap out the sprite to be rendered by its name
            // Important: The name of the sprite must be the same!
            this.spriteRenderer.sprite = this.carSpriteSheet[this.spriteRenderer.sprite.name];
        }

        public void PlayBoosterAnimation(BoosterLevel boosterLevel, VehicleState currentVehicleState)
        {
            this.boosterObject[this.currentBoosterAnimationIndex][(int)currentVehicleState - 1].SetActive(false);

            this.currentBoosterAnimationIndex = (int)boosterLevel;
            this.isBoosterAnimationEnabled = true;

            this.boosterObject[this.currentBoosterAnimationIndex][(int)currentVehicleState - 1].SetActive(true);
            this.animator.SetBool("isCarBooster", true);
        }

        public void StopBoosterAnimation(VehicleState currentVehicleState)
        {
            if (this.isBoosterAnimationEnabled == false)
                return;

            this.boosterObject[this.currentBoosterAnimationIndex][(int)currentVehicleState - 1].SetActive(false);

            this.currentBoosterAnimationIndex = 0;
            this.isBoosterAnimationEnabled = false;

            this.animator.SetBool("isCarBooster", false);
        }

        public void PlayCollisionAnimation()
        {
            StartCoroutine(CoroutineCollisionAnimation(this.collisionAnimationTerm, this.collisionAnimationRepeatCount));
        }

        public void PlayCollisionAnimationWithTerm(float collisionAnimationTerm, int collisionAnimationRepeateCount)
        {
            StartCoroutine(CoroutineCollisionAnimation(collisionAnimationTerm, collisionAnimationRepeateCount));
        }

        private IEnumerator CoroutineCollisionAnimation(float collisionAnimationTerm, int collisionAnimationRepeatCount)
        {
            WaitForSeconds WFS = new WaitForSeconds(collisionAnimationTerm);

            for (int count = 0; count < collisionAnimationRepeatCount; count++)
            {
                this.spriteRenderer.color = Color.gray;
                yield return WFS;

                this.spriteRenderer.color = Color.white;
                yield return WFS;
            }
        }
    }
}