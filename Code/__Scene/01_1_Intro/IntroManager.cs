using UnityEngine;

public class IntroManager : MonoBehaviour, ICMInterface
{
    private IntroNicknameManager introNicknameManager;

    private GameObject CANVAS_Start;

    private void Start()
    {
        PrepareBaseObjects();
        InitIntroManager();
    }

    public void PrepareBaseObjects()
    {
        if (this.introNicknameManager == null)
        {
            this.introNicknameManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "IntroNicknameManager", true).GetComponent<IntroNicknameManager>();
        }

        if (this.CANVAS_Start == null)
        {
            this.CANVAS_Start = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("MainCanvas"), "CANVAS_Start", true);
        }
    }

    private void InitIntroManager()
    {
        Application.targetFrameRate = 60;

        // TO DO : 서버 연결 안 되어 있으면 팝업 띄우고 기록이 저장되지 않는다는 메시지를 띄움
        //         클릭이 다시 들어오면 다시 시도해보고, 가만히 있으면 TOUCH TO START 나오게 하기
        //ServerManager.instance.CheckServerConnection("", ServerConnectionSuccessAction, ServerConnectionFailAction);

        if (ServerManager.instance.IsFirebaseLoggedIn() == false)
        {
            // TO DO : 어떤 경로로 가입할건지에 대해 물어보는 팝업을 띄운다.
            this.introNicknameManager.ActiveNicknameCanvas(true);
        }
        else
        {
            this.CANVAS_Start.SetActive(true);
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

    // TO DO : 임시 테스트 함수
    public void testLoadNextScene(string sceneName)
    {
        SecurityPlayerPrefs.SetInt("security-related", 1);
        LoadingSceneManager.LoadScene(sceneName);
        SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
    }
}
