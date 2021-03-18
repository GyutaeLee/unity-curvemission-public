using UnityEngine;

public class SPRCarDetectionManager : MonoBehaviour, ICMInterface
{
    public class CarDetectionInformation
    {

    }

    private CarDetectionInformation info;

    private SPRGameManager sprGameManager;

    private GameObject carObject;

    private void Awake()
    {
        this.info = new CarDetectionInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRCarDetectionManager();
    }

    public void PrepareBaseObjects()
    {
        if (this.carObject == null)
        {
            this.carObject = this.gameObject.transform.parent.gameObject;
        }

        if (this.sprGameManager == null)
        {
            this.sprGameManager = SPRGameManager.instance;
        }
    }

    private void InitSPRCarDetectionManager()
    {

    }

    // TO DO : switch가 너무 길다. 추후 로직 수정 필요
    private void OnTriggerEnter2D(Collider2D otherObject)
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        SPRCollisionManager m = otherObject.transform.GetComponent<SPRCollisionManager>();

        if (m == null)
        {
            return;
        }

        if (m.info.isActive == false)
        {
            return;
        }

        switch (m.eSPRCollision)
        {
            case ESPRCollision.Direction:
                {
                    SPRCollisionManager.CollisionDirection cd = (SPRCollisionManager.CollisionDirection)m.info;
                    ESPRCollisionDirection e = cd.Collide();

                    this.sprGameManager.sprCarManager.SetReadyTurnCar((ECarState)e);
                }
                break;

            case ESPRCollision.Wall:
                {
                    SPRCollisionManager.CollisionWall cw = (SPRCollisionManager.CollisionWall)m.info;
                    ESPRCollisionWall e = cw.Collide();

                    this.sprGameManager.sprCarManager.ResetCarPosition(cw.resetPosition);
                }
                break;

            case ESPRCollision.Booster:
                {
                    SPRCollisionManager.CollisionBooster cb = (SPRCollisionManager.CollisionBooster)m.info;
                    ESPRCollisionBooster e = cb.Collide();

                    this.sprGameManager.sprCarManager.BoostCar((EBoosterLevel)e);
                }
                break;

            case ESPRCollision.Obstacle:
                {
                    SPRCollisionManager.CollisionObstacle co = (SPRCollisionManager.CollisionObstacle)m.info;
                    ESPRCollisionObstacle e = co.Collide();

                    this.sprGameManager.sprCarManager.ObstacleDecelerateCar(e);
                }
                break;

            case ESPRCollision.Lap:
                {
                    SPRCollisionManager.CollisionLap cl = (SPRCollisionManager.CollisionLap)m.info;
                    ESPRCollisionLap e = cl.Collide();

                    CollideWithCollisionLap(otherObject.gameObject, e);
                    SPRGameManager.instance.sprUIManager.UpdateUILapCount();
                }
                break;

            case ESPRCollision.Item:
            default:
                m.info.Collide();
                break;
        }

        m.info.PlaySound();
    }

    private void CollideWithCollisionLap(GameObject otherObject, ESPRCollisionLap eSPRCollisionLap)
    {
        switch (eSPRCollisionLap)
        {
            case ESPRCollisionLap.Normal:
                {
                    this.sprGameManager.sprLapManager.AddCurrentLapCount(1);

                    if (this.sprGameManager.sprLapManager.IsLastLap() == true)
                    {
                        this.sprGameManager.sprLapManager.StartCoroutineBlinkLastLapUI(null);
                        this.sprGameManager.sprMapManager.ActiveCollisionLapHalf(true);
                        otherObject.gameObject.SetActive(false);
                    }
                }
                break;

            case ESPRCollisionLap.Half:
                {
                    if (this.sprGameManager.sprLapManager.IsLastLap() == false)
                    {
                        return;
                    }

                    this.sprGameManager.sprMapManager.ActiveCollisionLapFinish(true);
                }
                break;

            case ESPRCollisionLap.Finish:
                {
                    if (this.sprGameManager.sprLapManager.IsLastLap() == false)
                    {
                        return;
                    }

                    SPRGameManager.instance.FinishGame();
                    this.sprGameManager.sprCarManager.SetCarFinish();
                }
                break;

            default:
                break;
        }
    }
}
