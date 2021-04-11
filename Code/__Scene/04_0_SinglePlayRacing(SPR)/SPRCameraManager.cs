using UnityEngine;

public class SPRCameraManager : MonoBehaviour
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

    private void PrepareBaseObjects()
    {
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.cameraObject, GameObject.Find("Folder"), "MainCamera", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.carObject, GameObject.Find("Game"), "SPRCar", true);
        CMObjectManager.CheckNullAndFindCameraInAllChild(ref this._camera, GameObject.Find("Folder"), "MainCamera", true);
        CMObjectManager.CheckNullAndFindTransformInAllChild(ref this.info.carCurrentTransform, GameObject.Find("Game"), "SPRCar", true);
        
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

        float xCarStateWeight = 0.0f, yCarStateWeight = 0.0f;
        switch (sprCarManager.GetCurrentCarState())
        {
            case ECarState.Forward:
                xCarStateWeight = 0.75f;
                yCarStateWeight = 1.0f;
                break;
            case ECarState.Left:
                xCarStateWeight = -0.75f;
                yCarStateWeight = 1.0f;
                break;
            case ECarState.Back:
                xCarStateWeight = -0.75f;
                yCarStateWeight = -1.0f;
                break;
            case ECarState.Right:
                xCarStateWeight = 0.75f;
                yCarStateWeight = -1.0f;
                break;
            default:
                xCarStateWeight = 0.0f;
                yCarStateWeight = 0.0f;
                break;
        }

        this.info.cameraNextPositionVector.x = this.info.carCurrentTransform.position.x + xCarStateWeight;
        this.info.cameraNextPositionVector.y = this.info.carCurrentTransform.position.y + yCarStateWeight;

        this.cameraObject.transform.position = Vector3.Lerp(this.cameraObject.transform.position, this.info.cameraNextPositionVector, deltaTimeWeight * Time.deltaTime);
    }
}
