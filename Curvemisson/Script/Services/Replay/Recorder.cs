using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Replay
{
    public class Recorder : Util.Singleton<Recorder>
    {
        public bool IsRecording { get; private set; }

        private bool isRecordingInitialized;
        private MemoryStream memoryStream;
        private BinaryWriter binaryWriter;

        private int currentRecordingFrames;
        private int frameTimer;

        [SerializeField]
        private List<Transform> vehicleTransforms;

        [SerializeField]
        private List<Vehicle.Controller> controllers;

        [SerializeField]
        private List<Vehicle.Animation> animations;

        private void Start()
        {
            if (Static.Replay.IsReplayMode == true)
            {
                Destroy(this);
                return;
            }
        }

        private void FixedUpdate()
        {
            FixedUpdateRecording();
        }

        private void FixedUpdateRecording()
        {
            if (this.IsRecording == false)
                return;

            if (this.currentRecordingFrames > Constants.Replay.MaxRecordingFrames)
            {
                StopRecording();
                this.currentRecordingFrames = 0;
                return;
            }

            if (this.frameTimer == 0)
            {
                SaveVehicleState();
                ResetFrameTimer();
            }

            --this.frameTimer;
            ++this.currentRecordingFrames;
        }

        private void SaveVehicleState()
        {
            SaveVehicleTransforms();
            SaveVehicleIsCurveTiming();
        }

        private void SaveVehicleTransforms()
        {
            foreach (Transform transform in this.vehicleTransforms)
            {
                this.binaryWriter.Write(transform.localPosition.x);
                this.binaryWriter.Write(transform.localPosition.y);
            }
        }

        private void SaveVehicleIsCurveTiming()
        {
            for (int i = 0; i < this.controllers.Count; i++)
            {
                this.binaryWriter.Write(controllers[i].IsCurveAnimationInCurrentFrame);
            }
        }

        private void SaveVehicleAnimation()
        {
            for (int i = 0; i < this.animations.Count; i++)
            {
                this.binaryWriter.Write(animations[i].CarItemCarInfoID);
                this.binaryWriter.Write(animations[i].CarItemPaintInfoID);
            }
        }

        public void StartRecording()
        {
            if (this.isRecordingInitialized == false)
            {
                InitializeRecording();
            }
            else
            {
                this.memoryStream.SetLength(0);
            }

            StartFrameTimer();
            this.IsRecording = true;

            SaveVehicleAnimation();

            Debug.Log("녹화 시작");
        }

        public void StopRecording()
        {
            this.IsRecording = false;
            ResetSeek();

            Debug.Log("녹화 끝");
        }

        public void PauseRecording()
        {
            this.IsRecording = false;
            Debug.Log("녹화 일시 중지");
        }

        public void ResumeRecording()
        {
            this.IsRecording = true;
            Debug.Log("녹화 재개");
        }

        public void SaveRecording()
        {
            CheckAndCreateDirectory(Constants.Replay.ReplayDirectoryPath);
            
            string filePath = Static.Replay.GetUserSingleRacingReplayFilePath(User.User.Instance.CurrentStageID);
            File.WriteAllBytes(filePath, this.memoryStream.ToArray());
        }

        private void InitializeRecording()
        {
            this.memoryStream = new MemoryStream();
            this.binaryWriter = new BinaryWriter(this.memoryStream);

            this.isRecordingInitialized = true;
        }

        private void ResetSeek()
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            binaryWriter.Seek(0, SeekOrigin.Begin);
        }

        private void ResetFrameTimer()
        {
            this.frameTimer = Constants.Replay.RecordingFrameTerm;
        }

        private void StartFrameTimer()
        {
            this.frameTimer = 0;
        }

        private void CheckAndCreateDirectory(string directoryPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);

            if (directoryInfo.Exists == false)
            {
                directoryInfo.Create();
            }
        }
    }
}