using UnityEngine;

public class EditorSoundLoader : MonoBehaviour
{
    public void PlayGuiSound(int soundIndex)
    {
        Services.Sound.Effect.Manager.Instance.Play(Services.Enum.Sound.Effect.Type.Gui, soundIndex);
    }
}