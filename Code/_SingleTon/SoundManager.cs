using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    None = 0,

    ETC         = 1,
    UI          = 2,
    Collision   = 3,
    Car         = 4,

    Max,
}

public enum ESoundETC
{
    None = 0,

    CountDown   = 1,
    Finish      = 2,
    LastLap     = 3,

    Max,
}

public enum ESoundUI
{
    None = 0,

    ClickButton_1 = 1,

    Max,
}

public enum ESoundCollision
{
    None = 0,

    Rock_1        = 1,
    Coin_1        = 2,
    Booster_1     = 3,
    Booster_2     = 4,

    Max,
}

public enum ESoundCar
{
    None = 0,

    Engine_1      = 1,
    Drift_1       = 2,

    Max,
}

public class SoundManager : MonoBehaviour, ICMInterface
{
    public static SoundManager instance = null;
    const int kDefualtAudioSourceCount = 4;
    const int kInvalidIndex = -1;

    public class SoundInformation
    {
        public bool isSoundOff;
        public float soundVolume;
    }

    private SoundInformation info;

    private List<AudioSource> AS_Sound;
    private List<List<AudioClip>> AC_Sound;

    private void Awake()
    {
        InitInstance();
        this.info = new SoundInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitSoundManager();
    }

    private void InitInstance()
    {
        if (SoundManager.instance == null)
        {
            SoundManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareBaseObjects()
    {
        this.info.isSoundOff = SecurityPlayerPrefs.GetInt("security-related", 0) == 1 ? true : false;
        this.info.soundVolume = 1.0f - SecurityPlayerPrefs.GetFloat("security-related", 1.0f);
    }

    private void InitSoundManager()
    {
        IncreaseAudioPool();
        LoadSoundResources();
    }

    private void LoadSoundResources()
    {
        this.AC_Sound = new List<List<AudioClip>>();

        for (int i = 1; i < (int)ESoundType.Max; i++)
        {
            List<AudioClip> l = new List<AudioClip>();
            string soundResourceName;
            int count = 0;

            switch ((ESoundType)i)
            {
                case ESoundType.ETC:
                    count = (int)ESoundETC.Max;
                    soundResourceName = "security-related";
                    break;
                case ESoundType.UI:
                    count = (int)ESoundUI.Max;
                    soundResourceName = "security-related";
                    break;
                case ESoundType.Collision:
                    count = (int)ESoundCollision.Max;
                    soundResourceName = "security-related";
                    break;
                case ESoundType.Car:
                    count = (int)ESoundCar.Max;
                    soundResourceName = "security-related";
                    break;
                default:
                    count = 0;
                    soundResourceName = "";
                    break;
            }

            // TO DO : 필요한것만 들고 있도록 변경
            for (int j = 1; j < count; j++)
            {
                string s = string.Format("security-related{0}{1:d}", soundResourceName, j);
                l.Add(Resources.Load(s) as AudioClip);
            }

            this.AC_Sound.Add(l);
        }
    }

    public void PlaySound(ESoundType eSoundType, int soundIndex)
    {
        if (this.info.isSoundOff == true)
        {
            return;
        }

        if (eSoundType <= ESoundType.None || eSoundType >= ESoundType.Max)
        {
            return;
        }

        if (soundIndex <= 0 || soundIndex > this.AC_Sound[(int)eSoundType - 1].Count)
        {
            return;
        }

        int audioIndex = GetAvailableAudioIndex();

        if (audioIndex == kInvalidIndex)
        {
            IncreaseAudioPool();
            audioIndex = GetAvailableAudioIndex();
        }

        this.AS_Sound[audioIndex].clip = this.AC_Sound[(int)eSoundType - 1][soundIndex - 1];
        this.AS_Sound[audioIndex].Play();
    }

    public void PlaySoundSeveralTimes(ESoundType eSoundType, int soundIndex, int times)
    {
        StartCoroutine(CoroutinePlaySoundSeveralTimes(eSoundType, soundIndex, times));
    }

    private IEnumerator CoroutinePlaySoundSeveralTimes(ESoundType eSoundType, int soundIndex, int times)
    {
        WaitForSeconds WFS = new WaitForSeconds(this.AC_Sound[(int)eSoundType - 1][soundIndex - 1].length);

        for (int i = 0; i < times; i++)
        {
            PlaySound(eSoundType, soundIndex);

            yield return WFS;
        }
    }

    private int GetAvailableAudioIndex()
    {
        for (int i = 0; i < this.AS_Sound.Count; i++)
        {
            if (this.AS_Sound[i].isPlaying == false)
            {
                return i;
            }
        }

        return kInvalidIndex;
    }

    private void IncreaseAudioPool()
    {
        if (this.AS_Sound == null)
        {
            this.AS_Sound = new List<AudioSource>();
        }

        for (int i = 0; i < kDefualtAudioSourceCount; i++)
        {
            AudioSource a = this.gameObject.AddComponent<AudioSource>();

            this.AS_Sound.Add(a);
        }
    }

    private void DecreaseAudioPool()
    {
        if (this.AS_Sound.Count <= kDefualtAudioSourceCount)
        {
            return;
        }

        int count = this.AS_Sound.Count;

        for (int i = 0; i < count; i++)
        {
            if (this.AS_Sound[i].isPlaying == false)
            {
                this.AS_Sound.RemoveAt(i);
                i--;
                count--;

                if (count <= kDefualtAudioSourceCount)
                {
                    break;
                }
            }
        }
    }
}
