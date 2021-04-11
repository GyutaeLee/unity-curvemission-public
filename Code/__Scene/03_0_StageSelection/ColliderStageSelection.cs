public class ColliderStageSelection : ColliderButtonManager
{
    public int stageID;

    public override void ExecuteFunction()
    {
        switch (this.info.eButtonFunction)
        {
            case EButtonFunction.SceneMoveNormal:
                LoadScene(this.info.sceneName);
                SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
                break;
            case EButtonFunction.SceneMoveFadeOut:
                LoadSceneWithFadeOut(this.info.sceneName);
                SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
                break;
            default:
                break;
        }

        if (this.stageID != 0)
        {
            SecurityPlayerPrefs.SetInt("security-related", this.stageID);
        }
        this.gameObject.SetActive(false);
    }
}
