public class ColliderStageSelection : ColliderButtonManager
{
    public int stageID;

    public override void ExecuteFunction()
    {
        switch (this.info.eButtonFunction)
        {
            case EButtonFunction.SCENE_MOVE_NORMAL:
                LoadScene(this.info.sceneName);
                SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
                break;
            case EButtonFunction.SCENE_MOVE_FADE_OUT:
                LoadSceneWithFadeOut(this.info.sceneName);
                SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
                break;
            default:
                break;
        }

        SecurityPlayerPrefs.SetInt("security-related", this.stageID);
        this.gameObject.SetActive(false);
    }
}
