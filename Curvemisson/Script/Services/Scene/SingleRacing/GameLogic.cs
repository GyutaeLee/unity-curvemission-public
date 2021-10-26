using System.Collections;
using UnityEngine;

using Services.Enum.SingleRacing;

namespace Services.Scene.SingleRacing
{
    public class GameLogic : Util.Singleton<GameLogic>
    {
        private GameState currentGameState;
        public GameState CurrentGameState
        {
            get
            {
                return this.currentGameState;
            }
            private set
            {
                if (value <= GameState.None || value >= GameState.Max)
                {
                    this.currentGameState = GameState.None;
                }
                else
                {
                    this.currentGameState = value;
                }
            }
        }

        private int acquiredCoinQuantity;
        public int AcquiredCoinQuantity
        {
            get
            {
                return this.acquiredCoinQuantity;
            }
            set
            {
                if (value < 0)
                {
                    this.acquiredCoinQuantity = 0;
                }
                else
                {
                    this.acquiredCoinQuantity = value;
                }
            }
        }

        private int currentStageID;

        private void Awake()
        {
            if (GameLogic.Instance == null)
            {
                GameLogic.Instance = this;
            }
        }

        private void Start()
        {
            this.currentGameState = GameState.Intro;
            this.currentStageID = User.User.Instance.CurrentStageID;

            IngameUI.Instance.StartIntroAction(StartGame);
        }

        private void StartGame()
        {
            this.CurrentGameState = GameState.Playing;
            Sound.Bgm.Manager.Instance.Play(Enum.Sound.Bgm.Type.Racing, (int)Enum.Sound.Bgm.Racing.ValsanVilalge);
        }

        public void RetrytGame()
        {
            Time.timeScale = 1.0f;
            Scene.Loading.Main.LoadScene(Services.Constants.SceneName.SingleRacing);
        }

        public void PauseGame()
        {
            Time.timeScale = 0.0f;
            this.CurrentGameState = GameState.Pause;

            IngameUI.Instance.PauseGame();

            Sound.Bgm.Manager.Instance.Pause();
        }

        public void ResumeGame()
        {
            Time.timeScale = 1.0f;
            this.CurrentGameState = GameState.Playing;

            IngameUI.Instance.ResumeGame();

            Sound.Bgm.Manager.Instance.Resume();
        }

        public void GoToVillage()
        {
            Time.timeScale = 1.0f;
            Scene.Loading.Main.LoadScene(Constants.SceneName.Village);
        }

        public void FinishGame()
        {
            this.CurrentGameState = GameState.End;

            IngameUI.Instance.FinishGame();

            Server.Poster.PostUserAddCoinToFirebaseDB(this.AcquiredCoinQuantity);

            bool isUserBestRecord = IsUserBestRecord();
            if (isUserBestRecord == true)
            {
                Server.Poster.PostUserSingleRacingRecordToFirebaseDB(this.currentStageID,
                                                                     Lap.Instance.CurrentLapTime,
                                                                     User.User.Instance.CurrentCar);

                User.User.Instance.SetSingleRacingRecord(this.currentStageID,
                                                        Lap.Instance.CurrentLapTime,
                                                        User.User.Instance.CurrentCar);

                Thread.Waiter.InActiveThreadWait();
                Delegate.delegateActiveFlag delegateActiveFlag = new Delegate.delegateActiveFlag(Thread.Waiter.ActiveThreadWait);
                Server.Poster.CheckAndPostUserSingleRacingRankingToFirebaseDB(this.currentStageID, delegateActiveFlag);
            }

            Sound.Bgm.Manager.Instance.Stop();
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Etc, (int)Enum.Sound.Effect.Etc.FinishGame);
            StartCoroutine(CoroutineWaitingToGoResultScene(isUserBestRecord));
        }

        private bool IsUserBestRecord()
        {
            float singleRacingRecordTime = User.User.Instance.GetSingleRacingRecords(this.currentStageID, "security-related");

            if (Static.Record.IsValidRecordValue(singleRacingRecordTime) == false)
                return true;

            if (singleRacingRecordTime >= Lap.Instance.CurrentLapTime)
                return true;

            return false;
        }

        private IEnumerator CoroutineWaitingToGoResultScene(bool isBestRecord)
        {
            Delegate.delegateGetFlag delegateGetFlag = new Delegate.delegateGetFlag(Thread.Waiter.GetThreadWaitIsCompleted);
            yield return StartCoroutine(Thread.Waiter.CoroutineThreadWait(delegateGetFlag));

            if (Thread.Waiter.GetThreadWaitIsCompleted() == false)
            {
                string errorText = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Game, (int)Enum.GameText.Game.Error), Enum.Error.GameError.ThreadWaitTimeOver);
                Gui.Popup.Manager.Instance.OpenCheckPopup(errorText);
                Loading.Main.LoadScene(User.User.Instance.BeforeSceneName);
                yield break;
            }

            InstantiateAndSetResult(isBestRecord);
            FadeOutAndLoadResultScene();
        }

        private void InstantiateAndSetResult(bool isBestRecord)
        {
            GameObject singleRacingResultPrefab = Resources.Load<GameObject>("Prefab/Etc/SingleRacingResult");
            GameObject singleRacingResultObject = Instantiate(singleRacingResultPrefab);
            DontDestroyOnLoad(singleRacingResultObject);

            Result result = singleRacingResultObject.AddComponent<Result>();
            result.Set(isBestRecord,
                       this.currentStageID,
                       999, // TODO : user Ranking 찾아오기
                       this.AcquiredCoinQuantity,
                       Lap.Instance.CurrentLapTime);
        }

        private void FadeOutAndLoadResultScene()
        {
            Delegate.delegateLoadScene delegateLoadScene = new Delegate.delegateLoadScene(Loading.Main.LoadScene);
            Gui.FadeEffect.Instance.StartCoroutineFadeEffectWithLoadScene(delegateLoadScene, Constants.SceneName.SingleRacingResult, false);
        }

        public void FailGame()
        {
            this.CurrentGameState = GameState.End;

            Server.Poster.PostUserAddCoinToFirebaseDB(this.AcquiredCoinQuantity);
            Sound.Bgm.Manager.Instance.Stop();
        }

        public bool IsGameStatePlaying()
        {
            if (this.CurrentGameState != GameState.Playing)
                return false;

            return true;
        }
    }
}