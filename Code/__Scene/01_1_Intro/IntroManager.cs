using UnityEngine;

public class IntroManager : MonoBehaviour
{
    private IntroNicknameManager introNicknameManager;

    private GameObject CANVAS_Start;

    private void Start()
    {
        PrepareBaseObjects();
        InitIntroManager();
    }

    private void PrepareBaseObjects()
    {
        if (this.introNicknameManager == null)
        {
            this.introNicknameManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "IntroNicknameManager", true).GetComponent<IntroNicknameManager>();
        }

        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.CANVAS_Start, GameObject.Find("MainCanvas"), "CANVAS_Start", true);
    }

    private void InitIntroManager()
    {
        // 프레임 60 고정
        Application.targetFrameRate = 60;
        
        // TO DO : 임시 스크린 고정 코드
        //int narrower = (Screen.width < Screen.height) ? Screen.width : Screen.height;

        //Screen.SetResolution(narrower, (narrower / 9) * 16, true);
        //Screen.SetResolution(1125, 2436, false);

        // 서버 연결 되었는지 확인하기 
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
    public void LoadNextScene(string sceneName)
    {
        LoadingSceneManager.LoadScene(sceneName);
        SoundManager.instance.PlaySound(ESoundType.UI, (int)ESoundUI.ClickButton_1);
    }
}
