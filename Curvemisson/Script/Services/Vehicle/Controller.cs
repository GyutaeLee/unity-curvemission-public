using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Util.Csv.Data;
using Services.Enum.Vehicle;

namespace Services.Vehicle
{
    public class Controller : MonoBehaviour
    {
        public static readonly Vector2[] DirectionVector =
        {
            new Vector2((float)Math.Cos(45.0f * Mathf.PI / 180.0f), (float)Math.Sin(45.0f * Mathf.PI / 180.0f)).normalized,
            new Vector2((float)Math.Cos(153.5f * Mathf.PI / 180.0f), (float)Math.Sin(153.5f * Mathf.PI / 180.0f)).normalized,
            new Vector2((float)Math.Cos(225.0f * Mathf.PI / 180.0f), (float)Math.Sin(225.0f * Mathf.PI / 180.0f)).normalized,
            new Vector2((float)Math.Cos(333.5f * Mathf.PI / 180.0f), (float)Math.Sin(333.5f * Mathf.PI / 180.0f)).normalized
        };

        public const float AbsoluteBoosterMaxSpeed = 10.0f;
        public const int SkipTurnAnimationIndexWeight = 100;

        [SerializeField]
        private Transform carTransform;

        private VehicleState currentVehicleState;
        private VehicleState nextVehicleState;

        private int currentDirectionVectorIndex;

        private bool isEnable;
        public bool IsEnable
        {
            get
            {
                return this.isEnable;
            }
            set
            {
                this.isEnable = value;
            }
        }

        private bool isReadyToCurve;
        private bool isBoosterOn;
        private int nestedBoosterCount;

        private float currentSpeed;
        private float startSpeed;
        private float minSpeed;
        private float maxSpeed;

        private float normalAccelerationValue;
        private float normalDecelerationValue;
        private float curveDecelerationValue;

        private List<float> boosterValue;
        private List<float> boosterTime;

        private List<float> obstacleValue;

        [SerializeField]
        private Movement movement;
        [SerializeField]
        private new Animation animation;

        private void Start()
        {
            InitializeController();
        }

        private void InitializeController()
        {
            this.IsEnable = true;

            InitializeControllerDataByCarCsv();
            InitializeControllerDataByStageCsv();

            this.animation.SetAnimationState(this.currentVehicleState);
        }

        private void InitializeControllerDataByCarCsv()
        {
            int carInfoID = User.User.Instance.CurrentCar.CarInfoID;
            Util.Csv.Data.Class.Car carData = Storage<Util.Csv.Data.Class.Car>.Instance.GetDataByInfoID(carInfoID);

            this.startSpeed = carData.StartSpeed;
            this.currentSpeed = this.startSpeed;

            this.minSpeed = carData.MinSpeed;
            this.maxSpeed = carData.MaxSpeed;

            this.normalAccelerationValue = carData.NormalAccelerationValue;
            this.normalDecelerationValue = carData.NormalDecelerationValue;
            this.curveDecelerationValue = carData.CurveDecelerationValue;

            this.boosterValue = carData.BoosterValue;
            this.boosterTime = carData.BoosterTime;

            this.obstacleValue = carData.ObstacleValue;
        }

        private void InitializeControllerDataByStageCsv()
        {
            int stageID = User.User.Instance.CurrentStageID;
            Util.Csv.Data.Class.Stage stageData = Storage<Util.Csv.Data.Class.Stage>.Instance.GetDataByInfoID(stageID);

            this.carTransform.position = new Vector3(stageData.InitialVehiclePosition[0], stageData.InitialVehiclePosition[1], stageData.InitialVehiclePosition[2]);
            this.currentVehicleState = (VehicleState)stageData.InitialVehicleState;

            this.currentDirectionVectorIndex = GetDirectionVectorIndex(this.currentVehicleState);
        }

        private int GetDirectionVectorIndex(VehicleState vehicleState)
        {
            return (int)vehicleState - 1 > 0 ? (int)vehicleState - 1 : 0;
        }

        public void Acceleration()
        {
            if (IsMovable() == false)
                return;

            this.currentSpeed += this.normalAccelerationValue;

            float accelerationMaxSpeed = (this.isBoosterOn == true) ? AbsoluteBoosterMaxSpeed : this.maxSpeed;
            this.currentSpeed = Mathf.Clamp(this.currentSpeed, this.minSpeed, accelerationMaxSpeed);

            this.movement.MoveTransform(this.carTransform, DirectionVector[this.currentDirectionVectorIndex], this.currentSpeed);
        }

        public void Deceleration()
        {
            if (IsMovable() == false)
                return;

            this.currentSpeed -= this.normalDecelerationValue;
            this.currentSpeed = Mathf.Clamp(this.currentSpeed, this.minSpeed, this.maxSpeed);

            this.movement.MoveTransform(this.carTransform, DirectionVector[this.currentDirectionVectorIndex], this.currentSpeed);
        }

        public void Curve(bool isCurveAnimation)
        {
            if (CanCurve() == false)
                return;

            InActiveBooster();
            ChangeCurrentVehicleStateToNextState();
            DecelerationByCurve();

            if (isCurveAnimation == true)
            {
                PlayCurveAnimation();
                Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Car, (int)Enum.Sound.Effect.Car.Curve_1);
            }
            else
            {
                PlayNextVehicleStateAnimationWithoutCurveAnimation();
            }
        }

        private bool CanCurve()
        {
            if (IsMovable() == false)
                return false;

            if (this.isReadyToCurve == false)
                return false;

            if (this.nextVehicleState == this.currentVehicleState)
                return false;

            return true;
        }

        private void InActiveBooster()
        {
            if (this.isBoosterOn == false)
                return;

            this.isBoosterOn = false;
            this.nestedBoosterCount = 0;

            this.animation.StopBoosterAnimation(this.currentVehicleState);
        }

        private void ChangeCurrentVehicleStateToNextState()
        {
            this.currentDirectionVectorIndex = (int)this.nextVehicleState - 1;
            this.currentVehicleState = this.nextVehicleState;
        }

        private void DecelerationByCurve()
        {
            this.currentSpeed *= this.curveDecelerationValue;
            this.isReadyToCurve = false;
        }

        private void PlayCurveAnimation()
        {
            this.animation.SetAnimationState(this.nextVehicleState);
        }

        private void PlayNextVehicleStateAnimationWithoutCurveAnimation()
        {
            VehicleState vehicleState = this.nextVehicleState + SkipTurnAnimationIndexWeight;
            this.animation.SetAnimationState(vehicleState);
        }

        public void ResetPosition(Vector3 position)
        {
            this.carTransform.position = position;
            this.currentSpeed = 0.0f;

            Curve(false);
            this.animation.PlayCollisionAnimation();

            this.IsEnable = false;
            StartCoroutine(CoroutineActiveCar(0.5f));
        }

        private IEnumerator CoroutineActiveCar(float delayTerm)
        {
            yield return new WaitForSeconds(delayTerm);
            this.IsEnable = true;
        }

        public void SetCurvableState(VehicleState vehicleState)
        {
            if (IsMovable() == false)
                return;

            this.nextVehicleState = vehicleState;
            this.isReadyToCurve = true;
        }

        private bool IsMovable()
        {
            if (this.IsEnable == false)
                return false;

            // TODO : 추후 다른 모드의 게임이 나오게 되면, 각 게임 별 처리가 필요할듯.
            if (Services.Scene.SingleRacing.GameLogic.Instance.IsGameStatePlaying() == false)
                return false;

            return true;
        }

        public void Booster(BoosterLevel boosterLevel)
        {
            int boosterIndex = (int)boosterLevel;
            float boosterValue = this.boosterValue[boosterIndex];
            float boosterTime = this.boosterTime[boosterIndex];

            StartCoroutine(CoroutineBooster(boosterLevel, boosterValue, boosterTime));
        }

        private IEnumerator CoroutineBooster(BoosterLevel boosterLevel, float boosterValue, float boosterTime)
        {
            this.currentSpeed += boosterValue;
            this.isBoosterOn = true;
            this.nestedBoosterCount++;

            this.animation.PlayBoosterAnimation(boosterLevel, this.currentVehicleState);

            yield return new WaitForSeconds(boosterTime);

            if (this.isBoosterOn == false)
                yield break;

            this.nestedBoosterCount--;
            this.currentSpeed -= boosterValue * 0.5f;

            if (this.nestedBoosterCount > 0)
                yield break;

            this.isBoosterOn = false;
            this.animation.StopBoosterAnimation(this.currentVehicleState);
        }

        public void ObstacleDeceleration(ObstacleLevel obstacleLevel)
        {
            float obstacleDecelerationValue = this.obstacleValue[(int)obstacleLevel];

            this.currentSpeed -= obstacleDecelerationValue;
            this.currentSpeed = Mathf.Clamp(this.currentSpeed, this.minSpeed, this.maxSpeed);

            this.animation.PlayCollisionAnimation();
        }

        public void SetDeath()
        {
            InActiveBooster();

            this.IsEnable = false;
            this.currentVehicleState = VehicleState.Death;

            this.animation.SetAnimationState(VehicleState.Death);
        }

        public void SetFinish()
        {
            InActiveBooster();

            this.IsEnable = false;
            this.currentVehicleState = VehicleState.Finish;
        }

        // TODO : 카메라에서 가져가면 안 좋을거 같다. 로직 생각 필요
        public VehicleState GetCurrentVehicleState()
        {
            return this.currentVehicleState;
        }
    }
}