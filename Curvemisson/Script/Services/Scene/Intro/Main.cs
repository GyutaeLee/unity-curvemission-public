using UnityEngine;

namespace Services.Scene.Intro
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        private UserNickname userNickname;

        [SerializeField]
        private GameObject gameStartCanvas;

        private void Start()
        {
            const int FixedFrameRate = 60;
            Application.targetFrameRate = FixedFrameRate;

            // TODO : 서버 연결 안 되어 있으면 팝업 띄우고 기록이 저장되지 않는다는 메시지를 띄움
            //         클릭이 다시 들어오면 다시 시도해보고, 가만히 있으면 TOUCH TO START 나오게 하기
            Server.Manager.Instance.CheckNetworkConnection("", ServerConnectionSuccessAction, ServerConnectionFailAction);

            CheckSignIn();
        }

        private void CheckSignIn()
        {
            if (Server.Manager.Instance.IsFirebaseSignedIn() == false)
            {
                this.userNickname.ActiveNicknameInputCanvas(true);
            }
            else
            {
                this.gameStartCanvas.SetActive(true);
            }
        }

        public void ClickNicknameButton()
        {
            this.userNickname.ActiveNicknameInputCanvas(false);

            string nickname = this.userNickname.GetEnteredNickname();

            if (nickname == default(string))
            {
                this.userNickname.ActiveWrongNicknameCanvas(true);
            }
            else
            {
                Server.Sign.SignUpWithAnonymously(nickname, () => this.gameStartCanvas.SetActive(true));
            }
        }

        private void ServerConnectionSuccessAction()
        {
            Debug.Log("ConnectionSuccessAction");
        }

        private void ServerConnectionFailAction()
        {
            Debug.Log("ConnectionFailAction");
        }

        public void SignOutFirebase()
        {
            Server.Sign.SignOutFirebase();
        }
    }
}