using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Services.Scene.SingleRacing
{
    public class Result : MonoBehaviour
    {
        private bool isBestRecord;

        private int stageID;
        private int userRanking;
        private int acquiredCoinQuantity;
        private float lapTime;

        private GameObject finishObject;

        private List<GameObject> rankEffectObjects;
        private Image mapIconImage;

        private List<Text> recordTexts;
        private List<bool> isRecordTextSet;

        private Text coinText;

        private List<int> recordNumbers;
        private float recordEffectTime;

        public void Set(bool isBestRecord, int stageID, int userRanking, int acquiredCoinQuantity, float lapTime)
        {
            this.isBestRecord = isBestRecord;

            this.stageID = stageID;
            this.userRanking = userRanking;
            this.acquiredCoinQuantity = acquiredCoinQuantity;
            this.lapTime = lapTime;
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnLoaded;
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
        {
            if (scene.name != Constants.SceneName.SingleRacingResult)
                return;

            PrepareFinishObject();
            PrepareRankEffectObject();
            PrepareRecordText();
            PrepareCoinText();

            SetRecordNumber();
            SetCoinText();

            StartCoroutine(CoroutineRecordTextEffectAndChangeRankEffectObject());
        }

        private void OnSceneUnLoaded(UnityEngine.SceneManagement.Scene scene)
        {
            if (scene.name == Constants.SceneName.SingleRacingResult)
            {
                Destroy(this.gameObject);
            }
        }

        private void PrepareFinishObject()
        {
            string finishObject;

            if (this.isBestRecord == true)
            {
                finishObject = "FinishBest";
            }
            else
            {
                finishObject = "FinishNormal";
            }

            Useful.ObjectFinder.FindGameObjectInAllChild(ref this.finishObject, GameObject.Find("Canvas"), finishObject, true);
            this.finishObject.SetActive(true);
        }

        private void PrepareRankEffectObject()
        {

            this.rankEffectObjects = new List<GameObject>();

            GameObject rankEffectObject = Useful.ObjectFinder.GetGameObjectInAllChild(this.finishObject, "RankEffectObject", true);


            const int RankEffectVillageIndex = 0;
            for (int i = 0; i < rankEffectObject.transform.childCount; i++)
            {
                GameObject childObject = rankEffectObject.transform.GetChild(i).gameObject;

                if (childObject == null)
                    continue;

                if (i == RankEffectVillageIndex)
                {
                    Useful.ObjectFinder.FindComponentInAllChild(ref this.mapIconImage, childObject, "MapIconImage", true);
                    //this.mapIconImage.sprite = ; TODO : 맵 아이콘 추가 필요
                }
                this.rankEffectObjects.Add(childObject);
            }
        }

        private void PrepareRecordText()
        {
            this.recordTexts = new List<Text>();
            this.isRecordTextSet = new List<bool>();

            GameObject recordObject = Useful.ObjectFinder.GetGameObjectInAllChild(this.finishObject, "RecordObject", true);

            const int RecordDotIndex = 3;
            for (int i = 0; i < recordObject.transform.childCount; i++)
            {
                Text childText = recordObject.transform.GetChild(i).GetComponent<Text>();

                if (childText == null)
                    continue;

                if (i == RecordDotIndex)
                    continue;
                
                this.recordTexts.Add(childText);
                this.isRecordTextSet.Add(false);
            }
        }

        private void PrepareCoinText()
        {
            GameObject coinObject = Useful.ObjectFinder.GetGameObjectInAllChild(this.finishObject, "CoinObject", true);
            GameObject coinText = Useful.ObjectFinder.GetGameObjectInAllChild(coinObject, "CoinText", true);

            if (coinText == null || coinText.GetComponent<Text>() == null)
                return;
            
            this.coinText = coinText.GetComponent<Text>();
        }


        private void SetRecordNumber()
        {
            const float MaxLapTime = 999.999f;
            if (this.lapTime >= MaxLapTime)
            {
                this.recordNumbers = GetMaxTimeNumberList();
            }
            else
            {
                this.recordNumbers = GetLapTimeNumberList();
            }
        }

        private List<int> GetMaxTimeNumberList()
        {
            List<int> recordNumber = new List<int>();
            recordNumber.Capacity = this.recordTexts.Count;

            const int RecordTimeMaxNumber = 9;
            for (int i = 0; i < recordTexts.Count; i++)
            {
                recordNumber.Add(RecordTimeMaxNumber);
            }

            return recordNumber;
        }

        private List<int> GetLapTimeNumberList()
        {
            List<int> recordNumber = new List<int>();
            recordNumber.Capacity = this.recordTexts.Count;

            // 1. 정수 자리
            int subtractValue = 0;
            recordNumber.Add((int)(this.lapTime / 100));
            subtractValue = recordNumber[0] * 10;

            recordNumber.Add((int)(this.lapTime / 10) - subtractValue);
            subtractValue += recordNumber[1];
            subtractValue *= 10;

            recordNumber.Add((int)(this.lapTime / 1) - subtractValue);

            // 2. 소수 자리
            recordNumber.Add((int)((this.lapTime - (int)this.lapTime) * 10));
            recordNumber.Add((int)((this.lapTime * 10 - (int)(this.lapTime * 10)) * 10));
            recordNumber.Add((int)((this.lapTime * 100 - (int)(this.lapTime * 100)) * 10));

            return recordNumber;
        }
        private void SetCoinText()
        {
            this.coinText.text = this.acquiredCoinQuantity.ToString();
        }

        private IEnumerator CoroutineRecordTextEffectAndChangeRankEffectObject()
        {
            Gui.FadeEffect.Instance.StartCoroutineFadeEffect(true);

            yield return StartCoroutine(CoroutineRecordTextEffect());
            yield return StartCoroutine(CoroutineChangeRankEffectObject());
        }

        private IEnumerator CoroutineRecordTextEffect()
        {
            const float NumberChangeWaitTerm = 0.01f;
            const float MaxRecordEffectTime = 10.0f;

            WaitForSeconds WFS = new WaitForSeconds(NumberChangeWaitTerm);

            int audioIndex = Sound.Effect.Manager.Instance.PlayLoop(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.FinishLoopTick);
            while (this.recordEffectTime <= MaxRecordEffectTime)
            {
                this.recordEffectTime += NumberChangeWaitTerm;
                yield return WFS;

                if (CheckWaitTimeOverAndSetRecordText() == true)
                    break;
            }

            Sound.Effect.Manager.Instance.Stop(audioIndex);
            SetRecordText();
        }

        private bool CheckWaitTimeOverAndSetRecordText()
        {
            const float NumberTickWaitTime = 0.3f;
            const float StartWaitTime = 0.5f;

            for (int textIndex = this.recordTexts.Count - 1; textIndex >= 0; textIndex--)
            {
                float numberWaitTime = (this.recordTexts.Count - textIndex) * NumberTickWaitTime;

                if (this.isRecordTextSet[textIndex] == false)
                {
                    if (IsOverWaitTime(this.recordEffectTime, StartWaitTime + numberWaitTime) == true)
                    {
                        this.isRecordTextSet[textIndex] = true;
                        this.recordTexts[textIndex].text = this.recordNumbers[textIndex].ToString();
                        Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.NumberTick);
                    }
                    else
                    {
                        int number = Random.Range(0, 9);
                        this.recordTexts[textIndex].text = number.ToString();
                    }
                }
            }

            float lastNumberWaitTime = this.recordTexts.Count * NumberTickWaitTime;
            if (IsOverWaitTime(this.recordEffectTime, StartWaitTime + lastNumberWaitTime) == true)
                return true;
            else
                return false;
        }

        private bool IsOverWaitTime(float currentTime, float waitTime)
        {
            return (currentTime >= waitTime);
        }

        private IEnumerator CoroutineChangeRankEffectObject()
        {
            const float RankEffectLoopWaitTime = 1.5f;
            WaitForSeconds WFS = new WaitForSeconds(RankEffectLoopWaitTime);

            this.rankEffectObjects[0].SetActive(false);

            const int RankEffectStartIndex = 1;
            for (int i = RankEffectStartIndex; i < this.rankEffectObjects.Count; i++)
            {
                this.rankEffectObjects[i].SetActive(true);

                yield return WFS;

                if (i != this.rankEffectObjects.Count - 1)
                {
                    this.rankEffectObjects[i].SetActive(false);
                }
            }
        }

        private void SetRecordText()
        {
            for (int i = 0; i < recordTexts.Count; i++)
            {
                this.recordTexts[i].text = this.recordNumbers[i].ToString();
            }
        }
    }
}