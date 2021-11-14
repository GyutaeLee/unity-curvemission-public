using UnityEngine;

using Services.Delegate;

namespace Services.Scene.StageSelection
{
    public class Main : MonoBehaviour
    {
        private void Start()
        {
            Gui.FadeEffect.Instance.InActiveFadeEffectObject();
        }

        public void ClickMoveToStageButton(int stageID)
        {
            User.User.Instance.CurrentStageID = stageID;

            delegateLoadScene delegateLoadScene = new delegateLoadScene(Loading.Main.LoadScene);
            Gui.FadeEffect.Instance.StartCoroutineFadeEffectWithLoadScene(delegateLoadScene, Constants.SceneName.SingleRacingStage, false);
        }

#if DEV_MODE
        public void Test_ActiveUserReplayMode()
        {
            Static.Replay.ActiveUserReplayMode();
        }
#endif
    }
}