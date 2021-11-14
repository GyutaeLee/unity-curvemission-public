using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Replay
{
    public class Player : Util.Singleton<Player>
    {
        public bool IsReplay { get; private set; }

        private bool isReplayInitialized;
        private MemoryStream memoryStream;
        private BinaryReader binaryReader;

        private int frameTimer;

        [SerializeField]
        private List<Transform> vehicleTransforms;

        [SerializeField]
        private List<Vehicle.Controller> controllers;

        [SerializeField]
        private List<Vehicle.Animation> animations;

        private void Start()
        {
            if (Static.Replay.IsReplayMode == false)
            {
                Destroy(this);
                return;
            }

            InitializeReplay();
            LoadVehicleAnimation();
        }

        private void FixedUpdate()
        {
            FixedUpdateReplay();
        }

        private void FixedUpdateReplay()
        {
            if (this.IsReplay == false)
                return;

            if (this.memoryStream.Position >= memoryStream.Length)
            {
                StopReplay();
                return;
            }

            if (this.frameTimer == 0)
            {
                LoadVehicleState();
                ResetFrameTimer();
            }

            --frameTimer;
        }

        private void OnDestroy()
        {
            Static.Replay.InActiveReplayMode();
        }

        private void LoadVehicleState()
        {
            LoadVehicleTransforms();
            LoadVehicleIsCurveTiming();
        }

        private void LoadVehicleTransforms()
        {
            foreach (Transform transform in this.vehicleTransforms)
            {
                float x = binaryReader.ReadSingle();
                float y = binaryReader.ReadSingle();
                transform.localPosition = new Vector3(x, y, transform.localPosition.z);
            }
        }

        private void LoadVehicleIsCurveTiming()
        {
            foreach (Vehicle.Controller controller in this.controllers)
            {
                bool isCurveTiming = binaryReader.ReadBoolean();
                if (isCurveTiming == true)
                {
                    controller.Curve(true);
                }
            }
        }

        private void LoadVehicleAnimation()
        {
            foreach (Vehicle.Animation animation in this.animations)
            {
                int carItemCarInfoID = binaryReader.ReadInt32();
                int carItemPaintInfoID = binaryReader.ReadInt32();
                animation.InitializeAnimation(carItemCarInfoID, carItemPaintInfoID);
            }
        }

        private void InitializeReplay()
        {
            string filePath;
            if (Static.Replay.IsUserReplay == true)
            {
                filePath = Static.Replay.GetUserSingleRacingReplayFilePath(User.User.Instance.CurrentStageID);
            }
            else
            {
                filePath = Static.Replay.GetOtherUserSingleRacingReplayFilePath();
            }

            this.memoryStream = new MemoryStream(File.ReadAllBytes(filePath));
            this.binaryReader = new BinaryReader(this.memoryStream);

            this.isReplayInitialized = true;
        }

        public void StartReplay()
        {
            if (this.isReplayInitialized == false)
            {
                InitializeReplay();
            }

            StartFrameTimer();
            this.IsReplay = true;

            Debug.Log("리플레이 시작");
        }

        public void StopReplay()
        {
            this.IsReplay = false;
            ResetSeek();

            Debug.Log("리플레이 끝");
        }

        public void PauseReplay()
        {
            this.IsReplay = false;
            Debug.Log("리플레이 일시 중지");
        }

        public void ResumeReplay()
        {
            this.IsReplay = true;
            Debug.Log("리플레이 재개");
        }

        private void ResetSeek()
        {
            this.memoryStream.Seek(0, SeekOrigin.Begin);
        }

        private void ResetFrameTimer()
        {
            this.frameTimer = Constants.Replay.RecordingFrameTerm;
        }

        private void StartFrameTimer()
        {
            this.frameTimer = 0;
        }
    }
}