using System.Collections.Generic;
using UnityEngine;

public class SPRCsvReader : MonoBehaviour
{
    public static SPRCsvReader instance = null;

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

        public ECarState initialCarState;
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
        InitSPRCsvReader();
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

    private void InitSPRCsvReader()
    {
        
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

        this.csvCarInfo.carInfoID = SecurityPlayerPrefs.GetInt("security-related", 0);
        this.csvCarInfo.carPaintID = SecurityPlayerPrefs.GetInt("security-related", 0);

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

        List<float> position;
        int currentStageID = SecurityPlayerPrefs.GetInt("security-related", 0);

        this.csvStageInfo = new SPRCsvStageInformation();

        // initial car setting
        position = csvReader.GetCsvFloatListData(currentStageID, "security-related");
        this.csvStageInfo.initialCarState = (ECarState)csvReader.GetCsvIntData(currentStageID, "security-related");
        this.csvStageInfo.initialCarPosition = new Vector3(position[0], position[1], position[2]);

        if (this.csvStageInfo.initialCarState <= ECarState.None ||
            this.csvStageInfo.initialCarState >= ECarState.Max)
        {
            this.csvStageInfo.initialCarState = ECarState.Forward;
        }

        // camera position
        position = csvReader.GetCsvFloatListData(currentStageID, "security-related");

        // lap
        this.csvStageInfo.finishLapCount = csvReader.GetCsvIntData(currentStageID, "security-related"); 
    }
}
