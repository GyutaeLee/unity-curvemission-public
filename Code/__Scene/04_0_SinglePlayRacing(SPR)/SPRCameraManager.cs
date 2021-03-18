using UnityEngine;

public class SPRCameraManager : MonoBehaviour, ICMInterface
{
    public class SPRCameraInformation
    {
        public Transform carCurrentTransform;
        public Vector3 cameraNextPositionVector;
    }

    private SPRCameraInformation info;

    private SPRCarManager sprCarManager;

    private GameObject cameraObject;
    private GameObject carObject;
    private Camera _camera;

    private void Awake()
    {
        this.info = new SPRCameraInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRCameraManager();
    }

    private void FixedUpdate()
    {
        MoveCamera();
    }

    public void PrepareBaseObjects()
    {
        if (this.cameraObject == null)
        {
            this.cameraObject = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Folder"), "MainCamera", true);
        }

        if (this._camera == null)
        {
            this._camera = this.cameraObject.GetComponent<Camera>();
        }

        if (this.carObject == null)
        {
            this.carObject = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Game"), "SPRCar", true);
        }

        if (this.info.carCurrentTransform == null)
        {
            this.info.carCurrentTransform = this.carObject.transform;
        }

        if (this.sprCarManager == null)
        {
            this.sprCarManager = this.carObject.GetComponent<SPRCarManager>();
        }
    }

    private void InitSPRCameraManager()
    {
        this.cameraObject.transform.position = SPRCsvReader.instance.csvStageInfo.initialCameraPosition;
    }

    private void MoveCamera()
    {
        float deltaTimeWeight = 7.0f;

        if (SPRGameManager.instance.GetCurrentSPRGameState() == ESPRGameState.Intro)
        {
            deltaTimeWeight = 1.0f;
        }
        else if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        // TO DO : 나중에 가중치들 따로 변수에 넣어서 열거체와 동일한 인덱스로 대입하기
        float xCarStateWeight = 0.0f, xSpeedWeight = 0.0f;
        float yCarStateWeight = 0.0f, ySpeedWeight = 0.0f;

        switch (sprCarManager.GetCurrentCarState())
        {
            case ECarState.Forward:
                xCarStateWeight = 0.75f;
                yCarStateWeight = 1.0f;

                xSpeedWeight = 0.75f;
                ySpeedWeight = 0.5f;
                break;
            case ECarState.Left:
                xCarStateWeight = -0.75f;
                yCarStateWeight = 1.0f;

                xSpeedWeight = -1.0f;
                ySpeedWeight = 0.5f;
                break;
            case ECarState.Back:
                xCarStateWeight = -0.75f;
                yCarStateWeight = -1.0f;

                xSpeedWeight = -0.75f;
                ySpeedWeight = -0.5f;
                break;
            case ECarState.Right:
                xCarStateWeight = 0.75f;
                yCarStateWeight = -1.0f;

                xSpeedWeight = 1.0f;
                ySpeedWeight = -0.5f;
                break;
            default:
                break;
        }

        // TO DO : 효과 좀 더 고민하기 -> 베지어 곡선 써보자
        //float speedRate = _SPRCarManager.info.currentSpeed / _SPRCarManager.info.maxSpeed;

        //xCarStateWeight += xSpeedWeight * speedRate;
        //yCarStateWeight += ySpeedWeight * speedRate;

        this.info.cameraNextPositionVector.x = this.info.carCurrentTransform.position.x + xCarStateWeight;
        this.info.cameraNextPositionVector.y = this.info.carCurrentTransform.position.y + yCarStateWeight;

        this.cameraObject.transform.position = Vector3.Lerp(this.cameraObject.transform.position, this.info.cameraNextPositionVector, deltaTimeWeight * Time.deltaTime);
    }
}
