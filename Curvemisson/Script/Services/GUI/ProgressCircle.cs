using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Services.Gui
{
    public class ProgressCircle : MonoBehaviour
    {
        public static ProgressCircle Instance { get; private set; }

        private Dictionary<string, bool> _progressFlagDictionary;
        private Dictionary<string, int> _progressKeyDictionary;

        [SerializeField]
        private GameObject progressCircleObject;

        private Dictionary<string,bool> progressFlagDictionary
        {
            get
            {
                if (this._progressFlagDictionary == null)
                {
                    this._progressFlagDictionary = new Dictionary<string, bool>();
                }
                return this._progressFlagDictionary;
            }
        }

        private Dictionary<string, int> progressKeyDictionary
        {
            get
            {
                if (this._progressKeyDictionary == null)
                {
                    this._progressKeyDictionary = new Dictionary<string, int>();
                }
                return this._progressKeyDictionary;
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                ProgressCircle.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void Open(string progressKey)
        {
            IncreaseProgressKey(progressKey);

            if (IsProgressCircleOpend() == true)
                return;

            this.progressCircleObject.SetActive(true);
        }

        private void Close(string progressKey)
        {
            if (this.progressKeyDictionary.ContainsKey(progressKey) == false)
                return;

            DecreaseProgressKey(progressKey);

            if (this.progressKeyDictionary.Count == 0)
            {
                this.progressCircleObject.SetActive(false);
            }
        }

        private object lock_object_increaseProgressKey = new object();
        private void IncreaseProgressKey(string progressKey)
        {
            lock (lock_object_increaseProgressKey)
            {
                if (this.progressKeyDictionary.ContainsKey(progressKey) == false)
                {
                    this.progressKeyDictionary.Add(progressKey, 1);
                }
                else
                {
                    this.progressKeyDictionary[progressKey] = this.progressKeyDictionary[progressKey] + 1;
                }
            }
        }

        private object lock_object_decreaseProgressKey = new object();
        private void DecreaseProgressKey(string progressKey)
        {
            lock (lock_object_decreaseProgressKey)
            {
                if (this.progressKeyDictionary[progressKey] <= 1)
                {
                    this.progressKeyDictionary.Remove(progressKey);
                }
                else
                {
                    this.progressKeyDictionary[progressKey] = this.progressKeyDictionary[progressKey] - 1;
                }
            }
        }

        private bool IsProgressCircleOpend()
        {
            if (this.progressCircleObject.activeSelf == true)
                return true;

            return false;
        }

        public string GetProgressFlagKey(string progressKey)
        {
            string progressFlagKey = progressKey + "_" + this.progressKeyDictionary.Count;
            return progressFlagKey;
        }

        public void ScheduleClose(string progressKey, string progressFlagKey)
        {
            SetProgressFlag(progressFlagKey, true);
            StartCoroutine(CoroutineScheduleClose(progressKey, progressFlagKey));
        }

        private IEnumerator CoroutineScheduleClose(string progressKey, string progressFlagKey)
        {
            while (true)
            {
                if (this.progressFlagDictionary.ContainsKey(progressFlagKey) == false)
                    yield break;

                if (this.progressFlagDictionary[progressFlagKey] == false)
                    break;

                yield return null;
            }

            this.progressFlagDictionary.Remove(progressFlagKey);
            Close(progressKey);
        }

        public void CloseScheduled(string progressFlagKey)
        {
            SetProgressFlag(progressFlagKey, false);
        }

        private void SetProgressFlag(string progressFlagKey, bool isEnable)
        {
            if (this.progressFlagDictionary.ContainsKey(progressFlagKey) == false)
            {
                this.progressFlagDictionary.Add(progressFlagKey, isEnable);
            }
            else
            {
                this.progressFlagDictionary[progressFlagKey] = isEnable;
            }
        }
    }
}