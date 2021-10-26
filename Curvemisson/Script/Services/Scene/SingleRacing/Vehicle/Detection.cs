using UnityEngine;

using Services.Scene.SingleRacing;

namespace Services.SingleRacing.Vehicle
{
    public class Detection : MonoBehaviour
    {
        [SerializeField]
        private Services.Vehicle.Controller controller;

        private void OnTriggerEnter2D(Collider2D otherObject)
        {
            if (this.controller.IsEnable == false)
                return;

            Game.Collision.Collision collision = otherObject.transform.GetComponent<Game.Collision.Collision>();
            if (collision == null)
                return;

            if (collision.enabled == false)
                return;

            switch (collision.Type)
            {
                case Enum.Collision.Type.Direction:
                    {
                        Game.Collision.Direction direction = (Game.Collision.Direction)collision;
                        this.controller.SetCurvableState((Enum.Vehicle.VehicleState)direction.Collide());
                    }
                    break;
                case Enum.Collision.Type.Wall:
                    {
                        Game.Collision.Wall wall = (Game.Collision.Wall)collision;
                        this.controller.ResetPosition(wall.Collide());
                    }
                    break;
                case Enum.Collision.Type.Booster:
                    {
                        Game.Collision.Booster booster = (Game.Collision.Booster)collision;
                        this.controller.Booster((Enum.Vehicle.BoosterLevel)booster.Collide());
                    }
                    break;
                case Enum.Collision.Type.Obstacle:
                    {
                        Game.Collision.Obstacle obstacle = (Game.Collision.Obstacle)collision;
                        this.controller.ObstacleDeceleration((Enum.Vehicle.ObstacleLevel)obstacle.Collide());
                    }
                    break;
                case Enum.Collision.Type.Lap:
                    {
                        Game.Collision.Lap lap = (Game.Collision.Lap)collision;
                        CollideWithLap(otherObject.gameObject, lap.Collide());
                    }
                    break;

                case Enum.Collision.Type.FieldItem:
                    {
                        // TODO : fieldItem 에서 collide를 인식할지 고민해보기
                        Game.Collision.FieldItem fieldItem = (Game.Collision.FieldItem)collision;
                        fieldItem.Collide();
                    }
                    break;
                default:
                    break;
            }

            collision.PlaySound();
        }

        private void CollideWithLap(GameObject lapObject, Enum.Collision.Lap lap)
        {
             switch (lap)
            {
                case Enum.Collision.Lap.Normal:
                    {
                        Lap.Instance.CurrentLapCount++;

                        if (Lap.Instance.IsLastLap == true)
                        {
                            IngameUI.Instance.StartCoroutineBlinkLastLapUI();
                            Map.Instance.ActiveHalfOfFinishLap(true);
                            lapObject.gameObject.SetActive(false);
                        }
                    }
                    break;
                case Enum.Collision.Lap.HalfOfFinish:
                    {
                        if (Lap.Instance.IsLastLap == true)
                        {
                            Map.Instance.ActiveFinishLap(true);
                        }
                    }
                    break;
                case Enum.Collision.Lap.Finish:
                    {
                        if (Lap.Instance.IsLastLap == true)
                        {
                            this.controller.SetFinish();
                            GameLogic.Instance.FinishGame();
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}