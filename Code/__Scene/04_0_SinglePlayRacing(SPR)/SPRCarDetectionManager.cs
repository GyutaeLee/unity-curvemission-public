using UnityEngine;

public class SPRCarDetectionManager : MonoBehaviour
{
    private SPRCarManager sprCarManager;
    private SPRLapManager sprLapManager;
    private SPRStageManager sprStageManager;
    private SPRUIManager sprUIManager;

    private GameObject carObject;

    private void Start()
    {
        PrepareBaseObjects();
    }

    private void PrepareBaseObjects()
    {
        GameObject manager = GameObject.Find("Manager");
        if (this.carObject == null)
        {
            this.carObject = this.gameObject.transform.parent.gameObject;
        }

        if (this.sprCarManager == null)
        {
            this.sprCarManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Game"), "SPRCar", true).GetComponent<SPRCarManager>();
        }

        if (this.sprLapManager == null)
        {
            this.sprLapManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRLapManager", true).GetComponent<SPRLapManager>();
        }

        if (this.sprStageManager == null)
        {
            this.sprStageManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRStageManager", true).GetComponent<SPRStageManager>();
        }

        if (this.sprUIManager == null)
        {
            this.sprUIManager = CMObjectManager.FindGameObjectInAllChild(manager, "SPRUIManager", true).GetComponent<SPRUIManager>();
        }


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

                    this.sprCarManager.SetReadyTurnCar((ECarState)e);
                }
                break;

            case ESPRCollision.Wall:
                {
                    SPRCollisionManager.CollisionWall cw = (SPRCollisionManager.CollisionWall)m.info;
                    ESPRCollisionWall e = cw.Collide();

                    this.sprCarManager.ResetCarPosition(cw.resetPosition);
                }
                break;

            case ESPRCollision.Booster:
                {
                    SPRCollisionManager.CollisionBooster cb = (SPRCollisionManager.CollisionBooster)m.info;
                    ESPRCollisionBooster e = cb.Collide();

                    this.sprCarManager.BoostCar((EBoosterLevel)e);
                }
                break;

            case ESPRCollision.Obstacle:
                {
                    SPRCollisionManager.CollisionObstacle co = (SPRCollisionManager.CollisionObstacle)m.info;
                    ESPRCollisionObstacle e = co.Collide();

                    this.sprCarManager.ObstacleDecelerateCar(e);
                }
                break;

            case ESPRCollision.Lap:
                {
                    SPRCollisionManager.CollisionLap cl = (SPRCollisionManager.CollisionLap)m.info;
                    ESPRCollisionLap e = cl.Collide();

                    CollideWithCollisionLap(otherObject.gameObject, e);
                    this.sprUIManager.UpdateUILapCount();
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
                    this.sprLapManager.AddCurrentLapCount(1);

                    if (this.sprLapManager.IsLastLap() == true)
                    {
                        this.sprLapManager.StartCoroutineBlinkLastLapUI();
                        this.sprStageManager.ActiveCollisionLapHalf(true);
                        otherObject.gameObject.SetActive(false);
                    }
                }
                break;

            case ESPRCollisionLap.Half:
                {
                    if (this.sprLapManager.IsLastLap() == false)
                    {
                        return;
                    }

                    this.sprStageManager.ActiveCollisionLapFinish(true);
                }
                break;

            case ESPRCollisionLap.Finish:
                {
                    if (this.sprLapManager.IsLastLap() == false)
                    {
                        return;
                    }

                    SPRGameManager.instance.FinishGame();
                    this.sprCarManager.SetCarFinish();
                }
                break;

            default:
                break;
        }
    }
}
