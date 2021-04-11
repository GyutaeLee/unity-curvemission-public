using System.Collections.Generic;
using UnityEngine;

public class SPRCsvReader : MonoBehaviour
{
    private static SPRCsvReader _instance = null;
    public static SPRCsvReader instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public class SPRCsvCarInformation
    {
        public int carInfoID;
        public int carPaintID;

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
    }

    public class SPRCsvStageInformation
    {
        public int stageInfoID;

        private ECarState _initialCarState;
        public ECarState initialCarState
        {
            get
            {
                return _initialCarState;
            }
            set
            {
                if (value <= ECarState.None || value >= ECarState.Max)
                {
                    _initialCarState = ECarState.Forward;
                }
                else
                {
                    _initialCarState = value;
                }
            }
        }

        public Vector3 initialCarPosition;
        public Vector3 initialCameraPosition;

        public int finishLapCount;
    }

    public SPRCsvCarInformation csvCarInfo;
    public SPRCsvStageInformation csvStageInfo;

    private void Awake()
    {
        InitInstance();
    }

    private void Start()
    {
        ReadCsvData();
    }

    private void InitInstance()
    {
        if (SPRCsvReader.instance == null)
        {
            SPRCsvReader.instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void ReadCsvData()
    {
        ReadCsvCarInfo();
        ReadCsvStageInfo();
    }

    private void ReadCsvCarInfo()
    {
        CMCsvReader csvReader = new CMCsvReader("security-related");
        csvReader.ReadCsvFile();

        this.csvCarInfo = new SPRCsvCarInformation();

        this.csvCarInfo.carInfoID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultCarInfoID());
        this.csvCarInfo.carPaintID = SecurityPlayerPrefs.GetInt("security-related", InventoryInformation.GetDefaultPaintInfoID());

        this.csvCarInfo.startSpeed = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");

        this.csvCarInfo.minSpeed = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");
        this.csvCarInfo.maxSpeed = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");

        this.csvCarInfo.closeToMaxSpeedValue = this.csvCarInfo.maxSpeed * 0.85f;

        this.csvCarInfo.normalAccelerationValue = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");
        this.csvCarInfo.normalDecelerationValue = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");
        this.csvCarInfo.turnDecelerationValue = csvReader.GetCsvFloatData(this.csvCarInfo.carInfoID, "security-related");

        this.csvCarInfo.boosterValue = csvReader.GetCsvFloatListData(this.csvCarInfo.carInfoID, "security-related");
        this.csvCarInfo.boosterTime = csvReader.GetCsvFloatListData(this.csvCarInfo.carInfoID, "security-related");

        this.csvCarInfo.obstacleValue = csvReader.GetCsvFloatListData(this.csvCarInfo.carInfoID, "security-related");
    }

    private void ReadCsvStageInfo()
    {
        CMCsvReader csvReader = new CMCsvReader("security-related");
        csvReader.ReadCsvFile();

        // 1. initial car setting
        int currentStageID = SecurityPlayerPrefs.GetInt("security-related", SPRStageManager.GetDefaultStageID());
        List<float> position = csvReader.GetCsvFloatListData(currentStageID, "security-related");

        this.csvStageInfo = new SPRCsvStageInformation();
        this.csvStageInfo.initialCarPosition = new Vector3(position[0], position[1], position[2]);
        this.csvStageInfo.initialCarState = (ECarState)csvReader.GetCsvIntData(currentStageID, "security-related");

        // 2. camera position
        position = csvReader.GetCsvFloatListData(currentStageID, "security-related");

        // 3. lap
        this.csvStageInfo.finishLapCount = csvReader.GetCsvIntData(currentStageID, "security-related"); 
    }
}
