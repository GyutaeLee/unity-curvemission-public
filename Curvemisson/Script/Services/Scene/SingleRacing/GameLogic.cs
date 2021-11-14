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

        private void Start()
        {
            this.currentGameState = GameState.Intro;
            this.currentStageID = User.User.Instance.CurrentStageID;

            IngameUI.Instance.StartIntroAction(StartGame);
        }

        private void StartGame()
        {
            this.CurrentGameState = GameState.Play;

            if (Static.Replay.IsReplayMode == true)
            {
                Replay.Player.Instance.StartReplay();
            }
            else
            {
                Replay.Recorder.Instance.StartRecording();
            }

            Sound.Bgm.Manager.Instance.Play(Enum.Sound.Bgm.Type.Racing, (int)Enum.Sound.Bgm.Racing.ValsanVilalge);
        }

        public void RetrytGame()
        {
            Time.timeScale = 1.0f;
            Scene.Loading.Main.LoadScene(Services.Constants.SceneName.SingleRacingStage);
        }

        public void PauseGame()
        {
            Time.timeScale = 0.0f;
            this.CurrentGameState = GameState.Pause;

            if (Static.Replay.IsReplayMode == true)
            {
                Replay.Player.Instance.PauseReplay();
            }
            else
            {
                Replay.Recorder.Instance.PauseRecording();
            }

            IngameUI.Instance.PauseGame();
            Sound.Bgm.Manager.Instance.Pause();
        }

        public void ResumeGame()
        {
            Time.timeScale = 1.0f;
            this.CurrentGameState = GameState.Play;

            if (Static.Replay.IsReplayMode == true)
            {
                Replay.Player.Instance.ResumeReplay();
            }
            else
            {
                Replay.Recorder.Instance.ResumeRecording();
            }

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
            Debug.Log("기록 : " + Lap.Instance.CurrentLapTime);
            this.CurrentGameState = GameState.End;
            IngameUI.Instance.FinishGame();

            if (Static.Replay.IsReplayMode == true)
            {
                FinishGameInReplayMode();
            }
            else
            {
                FinishGameInPlayMode();
            }

            Sound.Bgm.Manager.Instance.Stop();
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Etc, (int)Enum.Sound.Effect.Etc.FinishGame);
        }

        private void FinishGameInReplayMode()
        {
            Replay.Player.Instance.StopReplay();
            Sound.Bgm.Manager.Instance.Stop();
            Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Etc, (int)Enum.Sound.Effect.Etc.FinishGame);
            FadeOutAndLoadBeforeScene();
        }

        private void FinishGameInPlayMode()
        {
            Replay.Recorder.Instance.StopRecording();
            Server.Poster.PostUserAddCoinToFirebaseDB(this.AcquiredCoinQuantity);

            bool isUserBestRecord = IsUserBestRecord();
            if (isUserBestRecord == true)
            {
                Replay.Recorder.Instance.SaveRecording();
                Server.Poster.PostUserSingleRacingRecordingFile(this.currentStageID);

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

            StartCoroutine(CoroutineWaitingToGoResultScene(isUserBestRecord));
        }

        private bool IsUserBestRecord()
        {
            float singleRacingRecordTime = User.User.Instance.GetSingleRacingRecords(this.currentStageID, "time");

            if (Static.Record.IsValidRecordValue(singleRacingRecordTime) == false)
                return true;

            if (singleRacingRecordTime >= Lap.Instance.CurrentLapTime)
                return true;

            return false;
        }

        private IEnumerator CoroutineWaitingToGoResultScene(bool isBestRecord)
        {
            if (isBestRecord == true)
            {
                Delegate.delegateGetFlag delegateGetFlag = new Delegate.delegateGetFlag(Thread.Waiter.GetThreadWaitIsCompleted);
                yield return StartCoroutine(Thread.Waiter.CoroutineThreadWait(delegateGetFlag));

                if (Thread.Waiter.GetThreadWaitIsCompleted() == false)
                {
                    string errorText = string.Format(GameText.Manager.Instance.GetText(Enum.GameText.TextType.Game, (int)Enum.GameText.Game.Error), Enum.RequestResult.Game.ThreadWaitTimeOver);
                    Gui.Popup.Manager.Instance.OpenCheckPopup(errorText);
                    Gui.Popup.Manager.Instance.AddCheckPopupOkButtonListener(() => { Loading.Main.LoadScene(User.User.Instance.BeforeSceneName); });
                    yield break;
                }
            }

            InstantiateAndSetResult(isBestRecord);
            FadeOutAndLoadResultScene();
        }

        private void InstantiateAndSetResult(bool isBestRecord)
        {
            GameObject singleRacingResultObject = new GameObject("SingleRacingResult");
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
            Delegate.delegateLoadScene delegateLoadScene = new Delegate.delegateLoadScene(UnityEngine.SceneManagement.SceneManager.LoadScene);
            Gui.FadeEffect.Instance.StartCoroutineFadeEffectWithLoadScene(delegateLoadScene, Constants.SceneName.SingleRacingResult, false);
        }

        private void FadeOutAndLoadBeforeScene()
        {
            Delegate.delegateLoadScene delegateLoadScene = new Delegate.delegateLoadScene(UnityEngine.SceneManagement.SceneManager.LoadScene);
            Gui.FadeEffect.Instance.StartCoroutineFadeEffectWithLoadScene(delegateLoadScene, User.User.Instance.BeforeSceneName, false);
        }

        public void FailGame()
        {
            this.CurrentGameState = GameState.End;

            Server.Poster.PostUserAddCoinToFirebaseDB(this.AcquiredCoinQuantity);
            Sound.Bgm.Manager.Instance.Stop();
        }

        public bool IsGameProceeding()
        {
            if (this.CurrentGameState != GameState.Play)
                return false;

            return true;
        }
    }
}