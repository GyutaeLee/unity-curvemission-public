using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SPRLapManager : MonoBehaviour, ICMInterface
{
    public const float kNoneLapTime = -1;

    public class LapInformation
    {
        public int currentLapCount;
        public int finishLapCount;

        public float currentLapTime;
    }

    private LapInformation info;

    private Image IMG_LastLap;
    public Sprite[] SPT_LastLap;

    private void Awake()
    {
        this.info = new LapInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSPRLapManager();
    }

    public void PrepareBaseObjects()
    {
        if (this.IMG_LastLap == null)
        {
            this.IMG_LastLap = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("MainCanvas"), "IMG_LastLap", true).GetComponent<Image>();
        }
    }

    private void InitSPRLapManager()
    {
        this.info.currentLapCount = 1;
        this.info.finishLapCount = SPRCsvReader.instance.csvStageInfo.finishLapCount;
    }

    public void UpdateCurrentLapTime()
    {
        if (SPRGameManager.instance.IsGameStatePlaying() == false)
        {
            return;
        }

        this.info.currentLapTime += Time.deltaTime;
    }

    public void SetCurrentLapCount(int lapCount)
    {
        this.info.currentLapCount = lapCount;
    }

    public void AddCurrentLapCount(int lapCount)
    {
        this.info.currentLapCount += lapCount;
    }

    public bool IsLastLap()
    {
        if (this.info.currentLapCount == this.info.finishLapCount)
        {
            return true;
        }

        return false;
    }

    public void StartCoroutineBlinkLastLapUI(Image img)
    {
        if (img == null)
        {
            img = this.IMG_LastLap;
        }

        SoundManager.instance.PlaySoundSeveralTimes(ESoundType.ETC, (int)ESoundETC.LastLap, 2);

        StartCoroutine(CoroutineBlinkLastLapUI(img, 0.1f));
    }

    private IEnumerator CoroutineBlinkLastLapUI(Image img, float blinkTerm)
    {
        WaitForSeconds WFS = new WaitForSeconds(blinkTerm);
        int sptIndex = 0;

        img.gameObject.SetActive(true);
        img.enabled = true;

        while (true)
        { 
            img.sprite = this.SPT_LastLap[sptIndex];
            img.SetNativeSize();

            yield return WFS;

            sptIndex++;

            if (sptIndex >= this.SPT_LastLap.Length)
                break;
        }

        img.enabled = false;
        img.gameObject.SetActive(false);
    }

    /* etc */

    public float GetCurrentLapTime()
    {
        return this.info.currentLapTime;
    }

    public int GetCurrentLapCount()
    {
        return this.info.currentLapCount;
    }

    public int GetFinishLapCount()
    {
        return this.info.finishLapCount;
    }
}
