using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Services.Scene.SingleRacing
{
    public class IngameUI : Util.Singleton<IngameUI>
    {
        [SerializeField]
        private GameObject pauseCanvas;

        [SerializeField]
        private GameObject playingCanvas;

        [SerializeField]
        private Text currentLapCountText;
        [SerializeField]
        private Text currentLapTimeText;
        [SerializeField]
        private Text currentAcquiredCoinText;

        [SerializeField]
        private GameObject deathCanavs;
        [SerializeField]
        private Text deathLapTimeText;
        [SerializeField]
        private Text deathAcquiredCoinText;

        [SerializeField]
        private GameObject countDownCanvas;
        [SerializeField]
        private Image countDownImage;

        [SerializeField]
        private List<Sprite> countDownSprites;

        [SerializeField]
        private Image lastLapImage;

        [SerializeField]
        public List<Sprite> lastLapSprites;

#if (DEBUG_MODE)
        private float deltaTime;
        private float worstFPS = 1000;
        private float currentFPS;

        [SerializeField]
        public GameObject debugCanvas;

        [SerializeField]
        private Text currentFrameText;
        [SerializeField]
        private Text worstFrameText;
#endif

        private void Awake()
        {
            if (IngameUI.Instance == null)
            {
                IngameUI.Instance = this;
            }
        }

        private void Update()
        {
#if (DEBUG_MODE)
            UpdateDebugUI();
#endif
            if (GameLogic.Instance.IsGameStatePlaying() == false)
                return;

            this.currentLapCountText.text = string.Format("{0:d} / {1:d}", Lap.Instance.CurrentLapCount, Lap.Instance.FinishLapCount);
            this.currentLapTimeText.text = string.Format("{0:N3}", Lap.Instance.CurrentLapTime);
            this.currentAcquiredCoinText.text = GameLogic.Instance.AcquiredCoinQuantity.ToString();
        }

        public void StartIntroAction(System.Action StartGame)
        {
#if (UNITY_EDITOR)
            Gui.FadeEffect.Instance.InActiveFadeEffectObject();
            StartGame();
            this.playingCanvas.SetActive(true);
#else
            const int CountDownSpriteCount = 27;
            const int GoSpriteCount = 12;

            Gui.FadeEffect.Instance.StartCoroutineFadeEffect(true);
            StartCoroutine(CoroutineGameIntroCountDown(StartGame, 2.0f, 0.1f, CountDownSpriteCount, GoSpriteCount));
#endif
        }

        private IEnumerator CoroutineGameIntroCountDown(System.Action StartGame, float beginDelayTerm, float countDelayTerm, int countDownSptCount, int goSptCount)
        {
            // TODO : 차량에 맞게 사운드 변경
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Car, (int)Enum.Sound.Effect.Car.Engine_1);
            yield return new WaitForSeconds(beginDelayTerm);

            yield return StartCoroutine(CoroutineCountDownAnimation(countDelayTerm, countDownSptCount));

            StartGame();
            this.playingCanvas.SetActive(true);

            yield return StartCoroutine(CoroutineGoAnimation(countDelayTerm, countDownSptCount, goSptCount));

            this.countDownCanvas.SetActive(false);
        }

        private IEnumerator CoroutineCountDownAnimation(float countDelayTerm, int countDownSptCount)
        {
            this.countDownCanvas.SetActive(true);
            this.countDownImage.gameObject.SetActive(true);
            this.countDownImage.enabled = true;
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Etc, (int)Enum.Sound.Effect.Etc.CountDown);

            WaitForSeconds WFS = new WaitForSeconds(countDelayTerm);
            for (int spriteIndex = 0; spriteIndex < countDownSptCount; spriteIndex++)
            {
                if (spriteIndex < this.countDownSprites.Count)
                {
                    this.countDownImage.sprite = this.countDownSprites[spriteIndex];
                    this.countDownImage.SetNativeSize();
                }

                yield return WFS;
            }
        }

        private IEnumerator CoroutineGoAnimation(float countDelayTerm, int countDownSptCount, int goSptCount)
        {
            WaitForSeconds WFS = new WaitForSeconds(countDelayTerm);

            int totalSptCount = countDownSptCount + goSptCount;
            for (int spriteIndex = countDownSptCount; spriteIndex < totalSptCount; spriteIndex++)
            {
                if (spriteIndex < this.countDownSprites.Count)
                {
                    this.countDownImage.sprite = this.countDownSprites[spriteIndex];
                    this.countDownImage.SetNativeSize();
                }

                yield return WFS;
            }
        }

        public void StartCoroutineBlinkLastLapUI()
        {
            StartCoroutine(CoroutineBlinkLastLapUI(0.1f));
            Sound.Effect.Manager.Instance.PlaySeveralTimes(Enum.Sound.Effect.Type.Etc, (int)Enum.Sound.Effect.Etc.LastLap, 2);
        }

        private IEnumerator CoroutineBlinkLastLapUI(float blinkTerm)
        {
            WaitForSeconds WFS = new WaitForSeconds(blinkTerm);
            int spriteIndex = 0;

            this.lastLapImage.gameObject.SetActive(true);
            this.lastLapImage.enabled = true;

            while (true)
            {
                this.lastLapImage.sprite = this.lastLapSprites[spriteIndex];
                this.lastLapImage.SetNativeSize();

                yield return WFS;

                spriteIndex++;

                if (spriteIndex >= this.lastLapSprites.Count)
                    break;
            }

            this.lastLapImage.enabled = false;
            this.lastLapImage.gameObject.SetActive(false);
        }

        public void PauseGame()
        {
            this.pauseCanvas.SetActive(true);
            this.playingCanvas.SetActive(false);
        }

        public void ResumeGame()
        {
            this.pauseCanvas.SetActive(false);
            this.playingCanvas.SetActive(true);
        }

        public void FinishGame()
        {
            this.playingCanvas.SetActive(false);
        }

        public void FailGame()
        {
            this.playingCanvas.SetActive(false);
            this.deathCanavs.SetActive(true);

            this.deathLapTimeText.text = string.Format("{0:N3}", Lap.Instance.CurrentLapTime);
        }

#if (DEBUG_MODE)
        public void UpdateDebugUI()
        {
            this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
            this.currentFPS = 1.0f / this.deltaTime;
            this.worstFPS = (this.worstFPS < this.currentFPS) ? this.worstFPS : this.currentFPS;

            this.currentFrameText.text = string.Format("NOW   : {0:N2}", this.currentFPS);
            this.worstFrameText.text = string.Format("WORST : {0:N2}", this.worstFPS);
        }
#endif
    }
}