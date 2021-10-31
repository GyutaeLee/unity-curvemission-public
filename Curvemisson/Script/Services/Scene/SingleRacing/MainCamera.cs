using UnityEngine;

using Services.Enum.Vehicle;

namespace Services.Scene.SingleRacing
{
    public class MainCamera : MonoBehaviour
    {
        [SerializeField]
        private Transform mainCameraTransform;
        [SerializeField]
        private Transform vehicleTransform;

        [SerializeField]
        private Vehicle.Controller controller;

        private void Start()
        {
            Util.Csv.Data.Class.Stage stageData = Util.Csv.Data.Storage<Util.Csv.Data.Class.Stage>.Instance.GetDataByInfoID(User.User.Instance.CurrentStageID);
            this.mainCameraTransform.position = new Vector3(stageData.InitialCameraPosition[0], stageData.InitialCameraPosition[1], stageData.InitialCameraPosition[2]);
        }

        private void FixedUpdate()
        {
            const float DefaultLerpTimeWeight = 7.0f;
            float defaultLerpTimeWeight = DefaultLerpTimeWeight;

            if (GameLogic.Instance.CurrentGameState == Enum.SingleRacing.GameState.Intro)
            {
                defaultLerpTimeWeight = 1.0f;
            }
            else if (GameLogic.Instance.IsGameStatePlaying() == false)
            {
                return;
            }

            float xPositionWeight = 0.0f, yPositionWeight = 0.0f;
            switch (controller.CurrentVehicleState)
            {
                case VehicleState.Forward:
                    xPositionWeight = 0.75f;
                    yPositionWeight = 1.0f;
                    break;
                case VehicleState.Left:
                    xPositionWeight = -0.75f;
                    yPositionWeight = 1.0f;
                    break;
                case VehicleState.Back:
                    xPositionWeight = -0.75f;
                    yPositionWeight = -1.0f;
                    break;
                case VehicleState.Right:
                    xPositionWeight = 0.75f;
                    yPositionWeight = -1.0f;
                    break;
                default:
                    xPositionWeight = 0.0f;
                    yPositionWeight = 0.0f;
                    break;
            }

            Vector2 nextPositionVector = new Vector2();
            nextPositionVector.x = this.vehicleTransform.position.x + xPositionWeight;
            nextPositionVector.y = this.vehicleTransform.position.y + yPositionWeight;

            this.mainCameraTransform.position = Vector3.Lerp(this.mainCameraTransform.position, nextPositionVector, defaultLerpTimeWeight * Time.deltaTime);
        }
    }
}