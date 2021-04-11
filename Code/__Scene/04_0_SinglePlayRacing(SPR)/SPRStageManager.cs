using UnityEngine;

public enum ESPRStageType
{ 
    None = 0,

    BalsanVillage = 1001,

    Max
}

public class SPRStageManager : MonoBehaviour
{
    public class SPRStageInformation
    {
        public int currentStageID;
    }

    private SPRStageInformation info;

    private GameObject collisionLapHalf;
    private GameObject collisionLapFinish;

    private void Awake()
    {
        this.info = new SPRStageInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
    }

    private void PrepareBaseObjects()
    {
        this.info.currentStageID = SecurityPlayerPrefs.GetInt("security-related", GetDefaultStageID());

        GameObject game = GameObject.Find("Game");
        GameObject mapObject = Resources.Load("security-related" + this.info.currentStageID) as GameObject;

        Instantiate(mapObject, game.transform);

        if (this.collisionLapHalf == null)
        {
            this.collisionLapHalf = CMObjectManager.FindGameObjectInAllChild(game, "COLLISION_LAP_HALF", true);
            this.collisionLapHalf.SetActive(false);
        }

        if (this.collisionLapFinish == null)
        {
            this.collisionLapFinish = CMObjectManager.FindGameObjectInAllChild(game, "COLLISION_LAP_FINISH", true);
            this.collisionLapFinish.SetActive(false);
        }
    }

    /* etc */
    public static int GetDefaultStageID()
    {
        return (int)ESPRStageType.BalsanVillage;
    }

    public void ActiveCollisionLapHalf(bool isActive)
    {
        this.collisionLapHalf.SetActive(isActive);
    }

    public void ActiveCollisionLapFinish(bool isActive)
    {
        this.collisionLapFinish.SetActive(isActive);
    }

    public int GetCurrentStageID()
    {
        return this.info.currentStageID;
    }

    public static Sprite GetMapIcon(int stageID)
    {
        int mapIconIndex = stageID - GetDefaultStageID();
        const string mapIconSpriteSheetName = "security-related";
        Sprite mapIcon = Resources.LoadAll<Sprite>(mapIconSpriteSheetName)[mapIconIndex];

        return mapIcon;
    }
}
