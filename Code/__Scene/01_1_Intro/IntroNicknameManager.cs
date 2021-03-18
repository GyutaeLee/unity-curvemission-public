using UnityEngine;
using UnityEngine.UI;

public class IntroNicknameManager : MonoBehaviour, ICMInterface
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
        InitIntroNicknameManager();
    }

    public void PrepareBaseObjects()
    {
        GameObject mainCanvas = GameObject.Find("MainCanvas");

        if (this.CANVAS_Nickname == null)
        {
            this.CANVAS_Nickname = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Nickname", true);
        }

        if (this.CANVAS_Start == null)
        {
            this.CANVAS_Start = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "CANVAS_Start", true);
        }

        if (this.POPUP_WrongNickname == null)
        {
            this.POPUP_WrongNickname = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "POPUP_WrongNickname", true);
        }

        if (this.TXT_Popup == null)
        {
            this.TXT_Popup = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "PopupText", true).GetComponent<Text>();
        }

        if (this.INPUT_Nickname == null)
        {
            this.INPUT_Nickname = CMObjectManager.FindGameObjectInAllChild(mainCanvas, "InputField", true).GetComponent<InputField>();
        }

        if (this.loginManager == null)
        {
            this.loginManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "LoginManager", true).GetComponent<LoginManager>();
        }
    }

    private void InitIntroNicknameManager()
    {

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
            // 검사에 걸리면 규칙에 맞지 않다는 UI를 띄운다.
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

        // 닉네임 입력하는 UI off
        ActiveNicknameCanvas(false);

        // 금칙어 & 닉네임 길이 검사
        isCorrectNickname = (IsContainBanWord(nickname) == true) && (IsCorrectLength(nickname) == true);

        return isCorrectNickname;
    }
}
