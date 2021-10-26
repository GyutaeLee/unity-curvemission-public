using UnityEngine;
using UnityEngine.UI;

namespace Services.Scene.Intro
{
    public class UserNickname : MonoBehaviour
    {
        private string nickname;

        [SerializeField]
        private GameObject nicknameInptCanvas;
        [SerializeField]
        private GameObject wrongNicknameCanvas;

        [SerializeField]
        private UnityEngine.UI.Text description;

        [SerializeField]
        private InputField nicknameInput;

        public void ActiveNicknameInputCanvas(bool isEnabled)
        {
            if (this.nicknameInptCanvas != null)
            {
                this.nicknameInptCanvas.SetActive(isEnabled);
            }
            else
            {
                Debug.Log("ERROR : UserNickName Canavas is null");
            }
        }

        public void ActiveWrongNicknameCanvas(bool isEnabled)
        {
            if (this.wrongNicknameCanvas != null)
            {
                this.wrongNicknameCanvas.SetActive(isEnabled);
            }
            else
            {
                Debug.Log("ERROR : POPUPWrongUserNickName is null");
            }
        }

        private bool IsContainBanWord(string word)
        {
            // TODO : 금칙어 검사

            return true;
        }

        private bool IsCorrectLength(string word)
        {
            // TODO : 단어 길이 검사

            return true;
        }

        public string GetEnteredNickname()
        {
            this.nickname = this.nicknameInput.text;
            
            if (IsValidNickname(this.nickname) == false)
            {
                return default(string);
            }
            else
            {
                return this.nickname;
            }
        }

        public bool IsValidNickname(string nickname)
        {
            return (IsContainBanWord(nickname) == true) && (IsCorrectLength(nickname) == true);
        }
    }
}