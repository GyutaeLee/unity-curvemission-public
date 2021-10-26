using UnityEngine;
using UnityEngine.UI;

namespace Services.Scene.Village
{
    public class Gui : MonoBehaviour
    {
        [SerializeField]
        private UnityEngine.UI.Text userNicknameText;
        [SerializeField]
        private UnityEngine.UI.Text userCoinText;

        private string userNickname;
        private int userCoin;

        private void Start()
        {
            SetUserDataUI();
        }

        private void SetUserDataUI()
        {
            this.userNickname = User.User.Instance.GetUserNickname();
            this.userCoin = User.User.Instance.GetUserCoin_1();

            // [TEST]
            this.userNicknameText.text = "NICKNAME : " + this.userNickname;
            this.userCoinText.text = "COIN : " + this.userCoin;
        }

#if (UNITY_INCLUDE_TESTS)
        public void Test_SetUserDataUI()
        {
            this.userNickname = User.User.Instance.GetUserNickname();
            this.userCoin = User.User.Instance.GetUserCoin_1();

            // [TEST]
            this.userNicknameText.text = "NICKNAME : " + this.userNickname;
            this.userCoinText.text = "COIN : " + this.userCoin;
        }
#endif

#if (CHEAT_MODE)
        public void Cheat_AddCoin(int addCoinCount)
        {
            Server.Poster.PostUserAddCoinToFirebaseDB(addCoinCount);
        }

        public void Cheat_RfreshCoin()
        {
            this.userCoin = User.User.Instance.GetUserCoin_1();
            this.userCoinText.text = "COIN : " + this.userCoin;
        }
#endif
    }
}