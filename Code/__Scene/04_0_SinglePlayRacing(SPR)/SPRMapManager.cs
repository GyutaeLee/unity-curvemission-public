using UnityEngine;

public class SPRMapManager : MonoBehaviour, ICMInterface
{
    public class MapInformation
    {
        public int currentStageID;
    }

    private MapInformation info;

    private GameObject collisionLapHalf;
    private GameObject collisionLapFinish;

    private void Awake()
    {
        this.info = new MapInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRMapManager();
    }

    public void PrepareBaseObjects()
    {
        this.info.currentStageID = SecurityPlayerPrefs.GetInt("security-related", 0);

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

    private void InitSPRMapManager()
    {

    }

    /* etc */

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
}
