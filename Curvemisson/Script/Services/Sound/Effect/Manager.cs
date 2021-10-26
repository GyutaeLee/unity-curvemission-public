using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Services.Sound.Effect
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        private const int InvalidIndex = -1;
        private const int DefualtAudioSourceCount = 4;

        private bool isEffectSoundOn;
        private float effectSoundVolume;

        private List<AudioSource> _effectSoundAudioSource;
        private Dictionary<string, AudioClip> _effectSoundAudioClipDictionary;

        private List<AudioSource> effectSoundAudioSource
        {
            get
            {
                if (this._effectSoundAudioSource == null)
                {
                    this._effectSoundAudioSource = new List<AudioSource>();
                }
                return this._effectSoundAudioSource;
            }
        }

        private Dictionary<string, AudioClip> effectSoundAudioClipDictionary
        {
            get
            {
                if (this._effectSoundAudioClipDictionary == null)
                {
                    this._effectSoundAudioClipDictionary = new Dictionary<string, AudioClip>();
                }
                return this._effectSoundAudioClipDictionary;
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
            this.isEffectSoundOn = Security.PlayerPrefs.GetBool(Constants.PlayerPrefs.IsEffectSoundOn, true);
            this.effectSoundVolume = Security.PlayerPrefs.GetFloat(Constants.PlayerPrefs.EffectSoundVolume, 1.0f);
        }

        private void IncreaseAudioPool()
        {
            for (int i = 0; i < DefualtAudioSourceCount; i++)
            {
                AudioSource audioSource = this.gameObject.AddComponent<AudioSource>();
                this.effectSoundAudioSource.Add(audioSource);
            }
        }

        private void DecreaseAudioPool()
        {
            if (this.effectSoundAudioSource.Count <= DefualtAudioSourceCount)
                return;

            int count = this.effectSoundAudioSource.Count;

            for (int i = 0; i < count; i++)
            {
                if (this.effectSoundAudioSource[i].isPlaying == false)
                {
                    this.effectSoundAudioSource.RemoveAt(i);
                    i--;
                    count--;

                    if (count <= DefualtAudioSourceCount)
                        break;
                }
            }
        }

        private int GetAvailableAudioSourceIndex()
        {
            for (int i = 0; i < this.effectSoundAudioSource.Count; i++)
            {
                if (this.effectSoundAudioSource[i].isPlaying == false)
                    return i;
            }

            return InvalidIndex;
        }


        public int Play(Enum.Sound.Effect.Type effectSoundType, int effectSoundIndex)
        {
            if (this.isEffectSoundOn == false)
                return InvalidIndex;

            if (effectSoundType <= Enum.Sound.Effect.Type.None || effectSoundType >= Enum.Sound.Effect.Type.Max)
                return InvalidIndex;

            string effectSoundResourcePath = GetEffectSoundResourceDirectoryPath(effectSoundType) + effectSoundIndex;
            if (this.effectSoundAudioClipDictionary.ContainsKey(effectSoundResourcePath) == false)
            {
                AudioClip audioClip = Resources.Load<AudioClip>(effectSoundResourcePath);
                if (audioClip == null)
                    return InvalidIndex;

                this.effectSoundAudioClipDictionary[effectSoundResourcePath] = Resources.Load<AudioClip>(effectSoundResourcePath);
            }

            int audioSourceIndex = GetAvailableAudioSourceIndex();
            if (audioSourceIndex == InvalidIndex)
            {
                IncreaseAudioPool();
                audioSourceIndex = GetAvailableAudioSourceIndex();
            }

            this.effectSoundAudioSource[audioSourceIndex].clip = this.effectSoundAudioClipDictionary[effectSoundResourcePath];
            this.effectSoundAudioSource[audioSourceIndex].Play();

            return audioSourceIndex;
        }

        public int PlayLoop(Enum.Sound.Effect.Type effectSoundType, int effectSoundIndex)
        {
            int audioSourceIndex = Play(effectSoundType, effectSoundIndex);
            if (audioSourceIndex == InvalidIndex)
                return InvalidIndex;

            this.effectSoundAudioSource[audioSourceIndex].loop = true;

            return audioSourceIndex;
        }

        public void Stop(int audioSourceIndex)
        {
            if (audioSourceIndex == InvalidIndex)
                return;

            if (audioSourceIndex < 0 || audioSourceIndex >= this.effectSoundAudioSource.Count)
                return;

            this.effectSoundAudioSource[audioSourceIndex].loop = false;
            this.effectSoundAudioSource[audioSourceIndex].Stop();
        }

        public void PlaySeveralTimes(Enum.Sound.Effect.Type effectSoundType, int effectSoundIndex, int times)
        {
            StartCoroutine(CoroutinePlaySoundSeveralTimes(effectSoundType, effectSoundIndex, times));
        }

        private IEnumerator CoroutinePlaySoundSeveralTimes(Enum.Sound.Effect.Type effectSoundType, int effectSoundIndex, int times)
        {
            string effectSoundResourcePath = GetEffectSoundResourceDirectoryPath(effectSoundType) + effectSoundIndex;
            if (this.effectSoundAudioClipDictionary.ContainsKey(effectSoundResourcePath) == false)
            {
                this.effectSoundAudioClipDictionary[effectSoundResourcePath] = Resources.Load<AudioClip>(effectSoundResourcePath);
            }

            WaitForSeconds WFS = new WaitForSeconds(this.effectSoundAudioClipDictionary[effectSoundResourcePath].length);

            for (int i = 0; i < times; i++)
            {
                Play(effectSoundType, effectSoundIndex);
                yield return WFS;
            }
        }

        private string GetEffectSoundResourceDirectoryPath(Enum.Sound.Effect.Type effectSoundType)
        {
            string soundResourceDirectoryPath = "Sound/Effect/" + effectSoundType.ToString() + "/";
            return soundResourceDirectoryPath;
        }
    }
}