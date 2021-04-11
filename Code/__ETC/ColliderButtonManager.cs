using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ColliderButtonManager : MonoBehaviour
{
    public enum EButtonFunction
    {
        None = 0,

        SceneMoveNormal   = 1,
        SceneMoveFadeOut = 2,

        Max,
    }

    [Serializable]
    public class ColliderButtonInformation
    {
        public EButtonFunction eButtonFunction;

        public string sceneName;
        public bool isLoadingScene;

        public Vector2 beginPosition;
        public float dragTime;
    }

    public ColliderButtonInformation info;

    private FadeEffectManager fadeEffectManager;
    private FadeEffectManager.FadeEffectInformation fadeEffectInfo;


    public void Start()
    {
        InitColliderButtonManager();
    }

    private void InitColliderButtonManager()
    {
        if (this.info.eButtonFunction == EButtonFunction.SceneMoveFadeOut)
        {
            GameObject manager = GameObject.Find("Manager");

            if (this.fadeEffectManager == null)
            {
                this.fadeEffectManager = CMObjectManager.FindGameObjectInAllChild(manager, "FadeEffectManager", true).GetComponent<FadeEffectManager>();
            }

            this.fadeEffectInfo = new FadeEffectManager.FadeEffectInformation();
            this.fadeEffectManager.InitFadeEffectInformation(ref this.fadeEffectInfo, false);

            this.fadeEffectInfo.beginDelayTerm = 0.0f;
            this.fadeEffectInfo.fadeEffectTotalTime -= 0.5f;
        }
    }

    private void OnMouseUpAsButton()
    {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        Vector2 touchPosition = Input.GetTouch(0).position;
#else
        Vector2 touchPosition = Input.mousePosition;
#endif
        float distanceX = Mathf.Abs(this.info.beginPosition.x - touchPosition.x);
        float distanceY = Mathf.Abs(this.info.beginPosition.y - touchPosition.y);

        if (this.info.dragTime <= 0.05f || (distanceX <= 0.5f && distanceY <= 0.5f))
        {
            ExecuteFunction();
        }

        this.info.dragTime = 0.0f;
        this.info.beginPosition = Vector2.zero;
    }

    private void OnMouseDrag()
    {
        this.info.dragTime += Time.deltaTime;
    }

    private void OnMouseDown()
    {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        Vector2 touchPosition = Input.GetTouch(0).position;
#else
        Vector2 touchPosition = Input.mousePosition;
#endif
        this.info.beginPosition = touchPosition;
    }

    public virtual void ExecuteFunction()
    {
        switch(this.info.eButtonFunction)
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
    }

    
    /* [SCENE_MOVE_NORMAL] FUNCTION */

    public void LoadScene(string sceneName)
    {
        if (this.info.isLoadingScene == true)
        {
            LoadingSceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void LoadSceneWithFadeOut(string sceneName)
    {
        delegateLoadScene delegateLS;

        if (this.info.isLoadingScene == true)
        {
            delegateLS = new delegateLoadScene(LoadingSceneManager.LoadScene);
        }
        else
        {
            delegateLS = new delegateLoadScene(SceneManager.LoadScene);
        }

        this.fadeEffectManager.StartCoroutineFadeEffectWithLoadScene(this.fadeEffectInfo, delegateLS, sceneName);
        this.gameObject.SetActive(false);
    }
}
