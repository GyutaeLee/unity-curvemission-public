using UnityEngine;

using Services.Delegate;

namespace Services.Scene.StageSelection
{
    public class Main : MonoBehaviour
    {
        public void ClickMoveToStageButton(int stageID)
        {
            User.User.Instance.CurrentStageID = stageID;

            delegateLoadScene delegateLoadScene = new delegateLoadScene(Loading.Main.LoadScene);
            Gui.FadeEffect.Instance.StartCoroutineFadeEffectWithLoadScene(delegateLoadScene, Constants.SceneName.SingleRacing, false);
        }
    }
}