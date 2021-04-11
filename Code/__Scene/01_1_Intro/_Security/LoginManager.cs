using UnityEngine;

using Firebase.Auth;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    public IntroNicknameManager introNicknameManager;

    private void Start()
    {
        PrepareBaseObjects();
    }

    private void PrepareBaseObjects()
    {
        if (this.introNicknameManager == null)
        {
            this.introNicknameManager = CMObjectManager.FindGameObjectInAllChild(GameObject.Find("Manager"), "IntroNicknameManager", true).GetComponent<IntroNicknameManager>();
        }
    }

    public void SignInWithAnonymously(string nickname)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }

            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            DoSignInProcess(nickname);
        });
    }

    public void SignInWithGoogle(string nickname)
    {

    }

    public void SignInWithFacebook(string nickname)
    {

    }

    private void SignInWithEmail(string nickname)
    {

    }

    public void SignOutFirebase()
    {
        ServerManager.instance.SignOutFirebase();
    }

    public void SignOutFirebaseAndResetGame()
    {
        ServerManager.instance.SignOutFirebaseAndResetGame();
    }

    private void DoSignInProcess(string nickname)
    {
        ServerManager.instance.RefreshFirebase();
        ServerManager.instance.PostUserBaseInfoToFirebaseDB(nickname);
    }
}
