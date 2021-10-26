using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Services.Scene.SingleRacing
{
    public class Lap : Util.Singleton<Lap>
    {
        private int currentLapCount;
        public int CurrentLapCount
        {
            get
            {
                return this.currentLapCount;
            }
            set
            {
                this.currentLapCount = value;
            }
        }

        private int finishLapCount;
        public int FinishLapCount
        {
            get
            {
                return this.finishLapCount;
            }
        }

        public bool IsLastLap
        {
            get
            {
                return (this.finishLapCount == this.CurrentLapCount);
            }
        }

        private float currentLapTime;
        public float CurrentLapTime
        {
            get
            {
                return this.currentLapTime;
            }
        }

        private void Awake()
        {
            if (Lap.Instance == null)
            {
                Lap.Instance = this;
            }
        }

        private void Start()
        {
            this.currentLapCount = 1;
            this.currentLapTime = 0;

            Util.Csv.Data.Class.Stage stage = Util.Csv.Data.Storage<Util.Csv.Data.Class.Stage>.Instance.GetDataByInfoID(User.User.Instance.CurrentStageID);
            this.finishLapCount = stage.FinishLapCount;
        }

        private void Update()
        {
            if (GameLogic.Instance.IsGameStatePlaying() == false)
                return;

            this.currentLapTime += Time.deltaTime;
        }
    }
}