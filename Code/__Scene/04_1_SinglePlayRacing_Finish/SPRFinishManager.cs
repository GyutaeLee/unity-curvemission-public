using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SPRFinishManager : MonoBehaviour
{
    private const string _kFinishSceneName = "security-related";

    public static string kFinishSceneName
    {
        get
        {
            return _kFinishSceneName;
        }
    }


    public class SPRFinishInformation
    {
        public bool isBestRecord;

        public int currentStageID;
        public int currentUserRanking;

        public int resultCoin;
        public float resultLapTime;
    }

    private SPRFinishInformation info;

    private GameObject CANVAS_Finish;

    private List<GameObject> rankEffectObjects;
    private Image IMG_MapIcon;

    private List<Text> TXT_Records;
    private List<bool> isRecordTextSet;
    private Text TXT_Coin;

    private List<int> recordNumber;
    private float recordEffectTime;

    private FadeEffectManager fadeEffectManager;
    private FadeEffectManager.FadeEffectInformation fadeEffectInfo;

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != kFinishSceneName)
        {
            return;
        }

        PrepareBaseObjects();
        InitSPRFinishManager();
        
        StartCoroutine(CoroutineRecordTextEffectAndChangeRankEffectObject());
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        if (scene.name == kFinishSceneName)
        {
            Destroy(this.gameObject);
        }

    }

    public void SetSPRFinishInformation(SPRFinishInformation info)
    {
        this.info = info;
    }

    public void PrepareBaseObjects()
    {
        if (this.info == null)
        {
            return;
        }

        if (this.info.isBestRecord == true)
        {
            PrepareBestFinishObject();
        }
        else
        {
            PrepareNormalFinsihObject();
        }

        PrepareFadeEffectObject();
        PrepareRankEffectObject();
        PrepareRecordText();
        PrepareCoinText();
    }

    public void InitSPRFinishManager()
    {
        SetRecordNumber();
        SetCoinText();

        this.CANVAS_Finish.SetActive(true);
    }

    private void PrepareBestFinishObject()
    {
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Finish, GameObject.Find("MainCanvas"), "Finish_Best", true);
    }

    private void PrepareNormalFinsihObject()
    {
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Finish, GameObject.Find("MainCanvas"), "Finish_Normal", true);
    }

    private void PrepareFadeEffectObject()
    {
        GameObject fadeEffectObject = null;

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref fadeEffectObject, GameObject.Find("Manager"), "FadeEffectManager", true);

        this.fadeEffectManager = fadeEffectObject.GetComponent<FadeEffectManager>();
        this.fadeEffectInfo = new FadeEffectManager.FadeEffectInformation();
        this.fadeEffectManager.InitFadeEffectInformation(ref this.fadeEffectInfo, true);
    }

    private void PrepareRankEffectObject()
    {
        this.rankEffectObjects = new List<GameObject>();

        GameObject rankEffectObject = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Finish, "rankEffect", true);

        const int kVillageIndex = 0;
        for (int i = 0; i < rankEffectObject.transform.childCount; i++)
        {
            GameObject childObject = rankEffectObject.transform.GetChild(i).gameObject;

            if (childObject == null)
            {
                continue;
            }

            if (i == kVillageIndex)
            {
                CMObjectManager.CheckNullAndFindImageInAllChild(ref this.IMG_MapIcon, childObject, "IMG_MapIcon", true);
                this.IMG_MapIcon.sprite = SPRStageManager.GetMapIcon(this.info.currentStageID);
            }

            this.rankEffectObjects.Add(childObject);
        }
    }

    private void PrepareRecordText()
    {
        this.TXT_Records = new List<Text>();
        this.isRecordTextSet = new List<bool>();

        GameObject recordText = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Finish, "recordText", true);
        
        const int kDotIndex = 3;
        for (int i = 0; i < recordText.transform.childCount; i++)
        {
            Text childText = recordText.transform.GetChild(i).GetComponent<Text>();

            if (childText == null)
            {
                continue;
            }

            if (i == kDotIndex)
            {
                continue;
            }

            this.TXT_Records.Add(childText);
            this.isRecordTextSet.Add(false);
        }
    }

    private void PrepareCoinText()
    {
        GameObject coinObject = CMObjectManager.FindGameObjectInAllChild(this.CANVAS_Finish, "Coin", true);
        GameObject coinText = CMObjectManager.FindGameObjectInAllChild(coinObject, "TXT_Coin", true);

        if (coinText == null || coinText.GetComponent<Text>() == null)
        {
            return;
        }

        this.TXT_Coin = coinText.GetComponent<Text>();
    }

    private void SetRecordNumber()
    {
        const float kMaxLapTime = 999.999f;

        if (this.info.resultLapTime >= kMaxLapTime)
        {
            this.recordNumber = GetMaxTimeNumberList();
        }
        else
        { 
            this.recordNumber = GetLapTimeNumberList();
        }
    }

    private List<int> GetMaxTimeNumberList()
    {
        const int kMaxNumber = 9;

        List<int> recordNumber = new List<int>();
        recordNumber.Capacity = this.TXT_Records.Count;

        for (int i = 0; i < TXT_Records.Count; i++)
        {
            recordNumber.Add(kMaxNumber);
        }

        return recordNumber;
    }

    private List<int> GetLapTimeNumberList()
    {
        List<int> recordNumber = new List<int>();
        recordNumber.Capacity = this.TXT_Records.Count;

        // 1. 정수 자리
        int subtractValue = 0;
        recordNumber.Add((int)(this.info.resultLapTime / 100));
        subtractValue = recordNumber[0] * 10;

        recordNumber.Add((int)(this.info.resultLapTime / 10) - subtractValue);
        subtractValue += recordNumber[1];
        subtractValue *= 10;

        recordNumber.Add((int)(this.info.resultLapTime / 1) - subtractValue);

        // 2. 소수 자리
        recordNumber.Add((int)((this.info.resultLapTime - (int)this.info.resultLapTime) * 10));
        recordNumber.Add((int)((this.info.resultLapTime * 10 - (int)(this.info.resultLapTime * 10)) * 10));
        recordNumber.Add((int)((this.info.resultLapTime * 100 - (int)(this.info.resultLapTime * 100)) * 10));

        return recordNumber;
    }

    private IEnumerator CoroutineRecordTextEffectAndChangeRankEffectObject()
    {
        this.fadeEffectManager.StartCoroutineFadeEffect(this.fadeEffectInfo);

        yield return StartCoroutine(CoroutineRecordTextEffect());
        yield return StartCoroutine(CoroutineChangeRankEffectObject());
    }

    private IEnumerator CoroutineRecordTextEffect()
    {
        const float kMaxRecordEffectTime = 10.0f;
        const float kNumberChangeWaitTerm = 0.01f;
        WaitForSeconds WFS = new WaitForSeconds(kNumberChangeWaitTerm);

        int audioIndex = SoundManager.instance.PlaySoundLoop(ESoundType.UI, (int)ESoundUI.FinishLoopTick);
        while (this.recordEffectTime <= kMaxRecordEffectTime)
        {
            this.recordEffectTime += kNumberChangeWaitTerm;
            yield return WFS;

            if (CheckWaitTimeOverAndSetRecordText() == true)
            {
                break;
            }
        }

        SoundManager.instance.StopSound(audioIndex);
        SetRecordText();
    }

    private bool CheckWaitTimeOverAndSetRecordText()
    {
        const float kStartWaitTime = 0.5f;
        const float kNumberTickWaitTime = 0.3f;

        for (int txtIndex = this.TXT_Records.Count - 1; txtIndex >= 0; txtIndex--)
        {
            float numberWaitTime = (this.TXT_Records.Count - txtIndex) * kNumberTickWaitTime;

            if (this.isRecordTextSet[txtIndex] == false)
            { 
                if (IsOverWaitTime(this.recordEffectTime, kStartWaitTime + numberWaitTime) == true)
                {
                    this.isRecordTextSet[txtIndex] = true;
                    this.TXT_Records[txtIndex].text = this.recordNumber[txtIndex].ToString();
                    SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.NumberTick);
                }
                else
                {
                    int number = Random.Range(0, 9);
                    this.TXT_Records[txtIndex].text = number.ToString();
                }
            }
        }

        float lastNumberWaitTime = this.TXT_Records.Count * kNumberTickWaitTime;
        if (IsOverWaitTime(this.recordEffectTime, kStartWaitTime + lastNumberWaitTime) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsOverWaitTime(float currentTime, float waitTime)
    {
        return (currentTime >= waitTime);
    }

    private IEnumerator CoroutineChangeRankEffectObject()
    {
        const float kLoopWaitTime = 1.5f;
        const int kRankEffectStartIndex = 1;
        WaitForSeconds WFS = new WaitForSeconds(kLoopWaitTime);

        this.rankEffectObjects[0].SetActive(false);

        for (int i = kRankEffectStartIndex; i < this.rankEffectObjects.Count; i++)
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
        for (int i = 0; i < TXT_Records.Count; i++)
        {
            this.TXT_Records[i].text = this.recordNumber[i].ToString();
        }
    }

    private void SetCoinText()
    {
        this.TXT_Coin.text = this.info.resultCoin.ToString();
    }
}
