using UnityEngine;
using UnityEngine.UI;

public class IntroNicknameManager : MonoBehaviour
{
    public class IntroNicknameInformation
    {
        public string nickname;
    }

    private IntroNicknameInformation info;

    private GameObject CANVAS_Nickname;
    private GameObject CANVAS_Start;
    private GameObject POPUP_WrongNickname;
    private Text TXT_Popup;
    private InputField INPUT_Nickname;

    private LoginManager loginManager;

    private void Awake()
    {
        this.info = new IntroNicknameInformation();
    }

    private void Start()
    {
        PrepareBaseObjects();
    }

    private void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Nickname, mainCanvas, "CANVAS_Nickname", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Start, mainCanvas, "CANVAS_Start", true);
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.POPUP_WrongNickname, mainCanvas, "POPUP_WrongNickname", true);
        CMObjectManager.CheckNullAndFindTextInAllChild(ref this.TXT_Popup, mainCanvas, "PopupText", true);
        CMObjectManager.CheckNullAndFindInputFieldInAllChild(ref this.INPUT_Nickname, mainCanvas, "InputField", true);

        if (this.loginManager == null)
        {
            this.loginManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "LoginManager", true).GetComponent<LoginManager>();
        }
    }

    public void ActiveNicknameCanvas(bool isEnabled)
    {
        if (this.CANVAS_Nickname != null)
        {
            this.CANVAS_Nickname.SetActive(isEnabled);
        }
        else
        {
            Debug.Log("ERROR : UserNickName Canavas is null");
        }
    }

    private void ActiveWrongNickNameUI(bool isEnabled)
    {
        if (this.POPUP_WrongNickname != null)
        {
            this.POPUP_WrongNickname.SetActive(isEnabled);
        }
        else
        {
            Debug.Log("ERROR : POPUPWrongUserNickName is null");
        }
    }

    private bool IsContainBanWord(string word)
    {
        // TO DO : 금칙어 검사

        return true;
    }
    
    private bool IsCorrectLength(string word)
    {
        // TO DO : 단어 길이 검사

        return true;
    }

    public void ClickNicknameButton()
    {
        this.info.nickname = this.INPUT_Nickname.text;

        if (IsValidNickname(this.info.nickname) == false)
        {
            ActiveWrongNickNameUI(true);
        }
        else
        {
            // TO DO : 구글/페이스북/이메일/익명 분기 처리 만들기
            this.loginManager.SignInWithAnonymously(this.info.nickname);

            //           + 콜백을 만들까...
            this.CANVAS_Start.SetActive(true);
        }
    }

    public bool IsValidNickname(string nickname)
    {
        bool isCorrectNickname = false;

        ActiveNicknameCanvas(false);

        isCorrectNickname = (IsContainBanWord(nickname) == true) && (IsCorrectLength(nickname) == true);

        return isCorrectNickname;
    }
}
