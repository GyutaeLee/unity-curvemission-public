using UnityEngine;

namespace Services.Sound.Bgm
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        private bool isBgmOn;
        private float bgmVolume;

        private AudioSource _bgmAudioSource;
        private AudioClip bgmAudioClip;

        private AudioSource bgmAudioSource
        {
            get
            {
                if (this._bgmAudioSource == null)
                {
                    this._bgmAudioSource = this.gameObject.AddComponent<AudioSource>();
                    this._bgmAudioSource.loop = true;
                    this._bgmAudioSource.volume = this.bgmVolume;
                }

                return this._bgmAudioSource;
            }
        }

        private void Awake()
        {
            if (Manager.Instance == null)
            {
                Manager.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void Start()
        {
            this.isBgmOn = Security.PlayerPrefs.GetBool(Constants.PlayerPrefs.IsBgmOn, true);
            this.bgmVolume = Security.PlayerPrefs.GetFloat(Constants.PlayerPrefs.BgmVolume, 1.0f);
        }

        public bool Play(Enum.Sound.Bgm.Type bgmType, int bgmIndex)
        {
            string bgmResourceName = GetBgmResourceDirectoryPath(bgmType) + bgmIndex;
            AudioClip bgmAudioClip = Resources.Load<AudioClip>(bgmResourceName);
            
            if (bgmAudioClip == null)
                return false;

            if (this.bgmAudioSource.clip == bgmAudioClip && this.bgmAudioSource.isPlaying == true)
                return false;

            this.bgmAudioClip = bgmAudioClip;
            this.bgmAudioSource.clip = this.bgmAudioClip;

            if (this.isBgmOn == false)
            {
                Stop();
            }
            else
            {
                this.bgmAudioSource.volume = this.bgmVolume;
                this.bgmAudioSource.Play();
            }

            return true;
        }

        public void Stop()
        {
            this.bgmAudioSource.Stop();
        }

        public void Pause()
        {
            this.bgmAudioSource.Pause();
        }

        public void Resume()
        {
            this.bgmAudioSource.Play();
        }

        public void SetIsBgmOn(bool isBgmOn)
        {
            if (this.isBgmOn == isBgmOn)
                return;

            if (isBgmOn == true)
            {
                this.bgmAudioSource.volume = this.bgmVolume;
                this.bgmAudioSource.Play();
            }
            else
            {
                this.bgmAudioSource.volume = 0;
                this.bgmAudioSource.Stop();
            }

            this.isBgmOn = isBgmOn;
        }

        public void SetVolume(float bgmVolume)
        {
            this.bgmAudioSource.volume = bgmVolume;
        }

        private string GetBgmResourceDirectoryPath(Enum.Sound.Bgm.Type bgmType)
        {
            string bgmResourceDirectoryPath = "Sound/Bgm/" + bgmType.ToString() + "/";
            return bgmResourceDirectoryPath;
        }
    }
}