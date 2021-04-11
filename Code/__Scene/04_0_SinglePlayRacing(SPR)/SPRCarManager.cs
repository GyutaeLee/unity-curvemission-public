using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Forward-Left-Back-Right 순서 변경 금지
// ESPRCollisionDirection과 동일하게 맞추기
public enum ECarState
{
    None = 0,

    Forward,
    Left,
    Back,
    Right,

    Death,
    Finish,

    Max,
}

public class SPRCarManager : MonoBehaviour
{    
    public class CarInformation
    {
        public ECarState eCurrentCarState;
        public ECarState eCarNextState;

        public Vector2 currentDirectionVector;
        public Vector2[] directionVectorStorage;

        public bool isEnable;
        public bool isTurnReady;
        public bool isScreenTouched;
        public bool isAcceleration;
        public int accelerationCount;

        public float currentSpeed;
        public float startSpeed;
        public float minSpeed;
        public float maxSpeed;
        public float closeToMaxSpeedValue;

        public float normalAccelerationValue;
        public float normalDecelerationValue;
        public float turnDecelerationValue;

        public List<float> boosterValue;
        public List<float> boosterTime;

        public List<float> obstacleValue;

        public int carInfoID;
        public int carPaintID;
    }

    private CarInformation info;

    private SPRCarAnimationManager sprCarAnimationManager;

    private GameObject carObject;
    private Transform carTransform;

    private void Awake()
    {
        this.info = new CarInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRCarManager();
    }

    private void FixedUpdate()
    {
        GetUserInput();
        FixedUpdateCarPhysics();
    }

    private void PrepareBaseObjects()
    {
        GameObject game = GameObject.Find("Game");

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.carObject, game, "SPRCar", true);
        CMObjectManager.CheckNullAndFindTransformInAllChild(ref this.carTransform, game, "SPRCar", true);

        if (this.sprCarAnimationManager == null)
        {
            this.sprCarAnimationManager = CMObjectManager.FindGameObjectInAllChild(this.carObject, "SPRCarAnimation", true).GetComponent<SPRCarAnimationManager>();
        }
    }

    private void InitSPRCarManager()
    {
        this.info.isEnable = true;

        InitCarDataByCsvCarInfo();
        InitCarDataByCsvStageInfo();
        InitCarDirectionData();
    }

    private void InitCarDataByCsvCarInfo()
    {
        this.info.carInfoID = SPRCsvReader.instance.csvCarInfo.carInfoID;
        this.info.carPaintID = SPRCsvReader.instance.csvCarInfo.carPaintID;

        this.info.currentSpeed = 0.0f;
        this.info.startSpeed = SPRCsvReader.instance.csvCarInfo.startSpeed;

        this.info.minSpeed = SPRCsvReader.instance.csvCarInfo.minSpeed;
        this.info.maxSpeed = SPRCsvReader.instance.csvCarInfo.maxSpeed;

        this.info.closeToMaxSpeedValue = this.info.maxSpeed * 0.85f;

        this.info.normalAccelerationValue = SPRCsvReader.instance.csvCarInfo.normalAccelerationValue;
        this.info.normalDecelerationValue = SPRCsvReader.instance.csvCarInfo.normalDecelerationValue;
        this.info.turnDecelerationValue = SPRCsvReader.instance.csvCarInfo.turnDecelerationValue;

        this.info.boosterValue = SPRCsvReader.instance.csvCarInfo.boosterValue;
        this.info.boosterTime = SPRCsvReader.instance.csvCarInfo.boosterTime;

        this.info.obstacleValue = SPRCsvReader.instance.csvCarInfo.obstacleValue;
    }

    private void InitCarDataByCsvStageInfo()
    {
        this.carObject.transform.position = SPRCsvReader.instance.csvStageInfo.initialCarPosition;

        this.info.eCurrentCarState = SPRCsvReader.instance.csvStageInfo.initialCarState;
        this.info.eCarNextState = this.info.eCurrentCarState;
    }

    private void InitCarDirectionData()
    {
        this.info.directionVectorStorage = new Vector2[4];
        this.info.directionVectorStorage[0] = (new Vector2((float)Math.Cos(45.0f * Mathf.PI / 180.0f), (float)Math.Sin(45.0f * Mathf.PI / 180.0f))).normalized;
        this.info.directionVectorStorage[1] = (new Vector2((float)Math.Cos(153.5f * Mathf.PI / 180.0f), (float)Math.Sin(153.5f * Mathf.PI / 180.0f))).normalized;
        this.info.directionVectorStorage[2] = (new Vector2((float)Math.Cos(225.0f * Mathf.PI / 180.0f), (float)Math.Sin(225.0f * Mathf.PI / 180.0f))).normalized;
        this.info.directionVectorStorage[3] = (new Vector2((float)Math.Cos(333.5f * Mathf.PI / 180.0f), (float)Math.Sin(333.5f * Mathf.PI / 180.0f))).normalized;

        int directionIndex = (int)this.info.eCurrentCarState - 1 > 0 ? (int)this.info.eCurrentCarState - 1 : 0;
        this.info.currentDirectionVector = this.info.directionVectorStorage[directionIndex];

    }

    /* Car Physics */
    public void GetUserInput()
    {
        if (IsCarMovable() == false)
        {
            return;
        }

#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        if (Input.touchCount > 0)
        {
            this.info.isScreenTouched = true;
        }
        else
        {
            this.info.isScreenTouched = false;
        }
#else
        if (Input.GetMouseButton(0) == true || Input.GetKey(KeyCode.Space) == true)
        {
            this.info.isScreenTouched = true;
        }
        else
        {
            this.info.isScreenTouched = false;
        }
#endif

        if (IsCarReadyToTurn() == true)
        {
            TurnCarWithAnimation();
        }
    }

    private void FixedUpdateCarPhysics()
    {
        if (IsCarMovable() == false)
        {
            return;
        }

        if (this.info.isScreenTouched == true)
        {
            this.info.currentSpeed += this.info.normalAccelerationValue;
        }
        else
        {         
            this.info.currentSpeed -= this.info.normalDecelerationValue;
        }

        float maxSpeed = 0.0f;
        if (this.info.isAcceleration == true)
        {
            const float ABSOLUTE_MAX_SPEED = 10.0f;
            maxSpeed = ABSOLUTE_MAX_SPEED;
        }
        else
        {
            maxSpeed = this.info.maxSpeed;
        }

        // clamp car speed
        this.info.currentSpeed = Mathf.Clamp(this.info.currentSpeed, this.info.minSpeed, maxSpeed);

        // calculate next position
        Vector3 positionVector = this.info.currentSpeed * this.info.currentDirectionVector * Time.deltaTime;
        this.carTransform.position += positionVector;
    }

    public void ResetCarPosition(Vector3 position)
    {
        this.carTransform.position = position;
        this.info.currentSpeed = 0.0f;

        TurnCarWithoutAnimation();
        this.sprCarAnimationManager.PlayCarCollisionAnimation();

        this.info.isEnable = false;
        Invoke("ActiveCar", 0.5f);
    }

    /* Car Turn */
    private void TurnCarWithAnimation()
    {
        TurnCar(true);
    }

    private void TurnCarWithoutAnimation()
    {
        TurnCar(false);
    }

    private void TurnCar(bool isTurnAnimation)
    {
        if (IsCarMovable() == false)
        {
            return;
        }

        InActiveCarBooster();
        ChangeCurrentCarStateToNextState();
        DecelerationByTurn();
        ChangeCarAnimationByTurn(isTurnAnimation);

        if (isTurnAnimation == true)
        {
            SoundManager.instance.PlaySound(ESoundType.Car, (int)ESoundCar.Drift_1);
        }
    }

    private void ChangeCurrentCarStateToNextState()
    {
        this.info.currentDirectionVector = this.info.directionVectorStorage[(int)this.info.eCarNextState - 1];
        this.info.eCurrentCarState = this.info.eCarNextState;
    }

    private void DecelerationByTurn()
    {
        this.info.currentSpeed *= this.info.turnDecelerationValue;
        this.info.isTurnReady = false;
    }

    private void ChangeCarAnimationByTurn(bool isTurnAnimation)
    {
        const int kSkipTurnAnimationIndexWeight = 100;
        int carState = (int)this.info.eCarNextState;

        carState = (isTurnAnimation == false) ? (carState + kSkipTurnAnimationIndexWeight) : carState;
        this.sprCarAnimationManager.SetCaraAnimationStateByCarState(carState);
    }

    public void SetReadyTurnCar(ECarState eState)
    {
        if (IsCarMovable() == false)
        {
            return;
        }

        if (IsCarStateValid(eState) == false)
        {
            return;
        }

        this.info.eCarNextState = eState;
        this.info.isTurnReady = true;
    }

    /* Car Booster */
    public void BoostCar(EBoosterLevel eBoosterLevel)
    {
        float av = this.info.boosterValue[(int)eBoosterLevel];
        float at = this.info.boosterTime[(int)eBoosterLevel];

        BoostCarByValue(eBoosterLevel, av, at);
    }

    public void BoostCarByValue(EBoosterLevel eBoosterLevel, float accelerationValue, float accelerationTime)
    {
        StartCoroutine(CoroutineBoostCar(eBoosterLevel, accelerationValue, accelerationTime));
    }

    public void ObstacleDecelerateCar(ESPRCollisionObstacle eSPRCollisionObstacle)
    {
        float dv = this.info.obstacleValue[(int)eSPRCollisionObstacle];

        ObstacleDecelerateCarByValue(dv);
    }

    public void ObstacleDecelerateCarByValue(float obstacleDecelerationValue)
    {
        this.info.currentSpeed -= obstacleDecelerationValue;
        this.info.currentSpeed = Mathf.Clamp(this.info.currentSpeed, this.info.minSpeed, this.info.maxSpeed);

        this.sprCarAnimationManager.PlayCarCollisionAnimation();
    }

    private IEnumerator CoroutineBoostCar(EBoosterLevel eBoosterLevel, float boosterValue, float boosterTime)
    {
        this.info.currentSpeed += boosterValue;
        this.info.isAcceleration = true;
        this.info.accelerationCount++;
        
        this.sprCarAnimationManager.PlayCarBoosterAniamtion(eBoosterLevel, true);

        yield return new WaitForSeconds(boosterTime);

        if (this.info.isAcceleration == false)
        {
            yield break;
        }

        this.info.accelerationCount--;
        this.info.currentSpeed -= boosterValue * 0.5f;

        if (this.info.accelerationCount > 0)
        {
            yield break;
        }

        this.info.isAcceleration = false;
        this.sprCarAnimationManager.PlayCarBoosterAniamtion(eBoosterLevel, false);
    }

    private void InActiveCarBooster()
    {
        if (this.info.isAcceleration == false)
        {
            return;
        }

        this.info.isAcceleration = false;
        this.info.accelerationCount = 0;
        this.sprCarAnimationManager.StopCarBoosterAnimation();
    }

    /* Condition */
    private bool IsCarMovable()
    {
        if (this.info.isEnable == false || SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private bool IsCarReadyToTurn()
    {
        if (this.info.isScreenTouched == true)
        {
            return false;
        }

        if (this.info.isTurnReady == false)
        {
            return false;
        }            

        if (this.info.eCarNextState == this.info.eCurrentCarState)
        {
            return false;
        }

        return true;
    }

    private bool IsCarStateValid(ECarState eCarState)
    {
        if (eCarState <= ECarState.None || eCarState >= ECarState.Max)
        {
            return false;
        }

        return true;
    }

    public bool IsCarCloseToMaxSpeed()
    {
        if (this.info.currentSpeed >= this.info.maxSpeed * this.info.closeToMaxSpeedValue)
        {
            return true;
        }

        return false;
    }

    /* etc */

    public void SetCarDeath()
    {
        InActiveCarBooster();

        this.info.isEnable = false;
        this.info.eCurrentCarState = ECarState.Death;

        this.sprCarAnimationManager.SetCaraAnimationStateByCarState((int)ECarState.Death);
    }

    public void SetCarFinish()
    {
        InActiveCarBooster();

        this.info.isEnable = false;
        this.info.eCurrentCarState = ECarState.Finish;
    }

    private void ActiveCar()
    {
        SetCarEnable(true);
    }

    public void SetCarEnable(bool isEnable)
    {
        this.info.isEnable = isEnable;
    }

    public ECarState GetCurrentCarState()
    {
        return this.info.eCurrentCarState;
    }

    public float GetCurrentSpeed()
    {
        return this.info.currentSpeed;
    }

    public int GetCarInfoID()
    {
        return this.info.carInfoID;
    }

    public int GetCarPaintID()
    {
        return this.info.carPaintID;
    }
}
