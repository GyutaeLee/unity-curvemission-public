using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour, ICMInterface
{
    public static LoadingManager instance = null;

    private Dictionary<string, bool> currentLoadingFlags;
    private Dictionary<string, int> currentLoadingKeys;

    private GameObject loadingCanvasObject;

    private GameObject progressCirclePrefab;
    private GameObject progressCircleObject;

    private void Awake()
    {
        InitInstance();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitLoadingManager();
    }

    private void InitInstance()
    {
        if (LoadingManager.instance == null)
        {
            LoadingManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareBaseObjects()
    {
        this.loadingCanvasObject = CMObjectManager.FindGameObjectInAllChild(this.gameObject, "loadingCanvas", true);

        // loadingFlag
        this.currentLoadingFlags = new Dictionary<string, bool>();

        // loadingKey
        this.currentLoadingKeys = new Dictionary<string, int>();

        // prefab
        this.progressCirclePrefab = Resources.Load("security-related") as GameObject;
    }

    private void InitLoadingManager()
    {

    }

    private void ActiveLoadingCanvas(bool isActive)
    {
        if (isActive == true)
        {
            if (this.loadingCanvasObject.activeSelf == true)
            {
                return;
            }
        }
        else
        {            
            if (this.currentLoadingKeys.Count > 0)
            {
                return;
            }
        }

        this.loadingCanvasObject.SetActive(isActive);
    }

    // TO DO : 로딩 시간이 1초 이상 걸리지 않을 때에는 그냥 깜빡이기만해서,, 차라리 안 켜는건 어떨까?
    public void OpenProgressCircle(string loadingKey)
    {
        // 로딩 키를 가지고 있지 않으면 하나 추가해준다.
        if (this.currentLoadingKeys.ContainsKey(loadingKey) == false)
        {
            this.currentLoadingKeys.Add(loadingKey, 0);
        }

        // 로딩 키 값 +1
        this.currentLoadingKeys[loadingKey] += this.currentLoadingKeys[loadingKey] + 1;

        // 이미 프로그레스 서클이 켜져있으면 key에만 추가한다.
        if (this.progressCircleObject != null && this.progressCircleObject.activeSelf == true)
        {
            return;
        }

        // 1. 로딩 캔버스를 켜준다.
        ActiveLoadingCanvas(true);

        // 2. 프로그레서 서클을 인스턴싱 후 켜준다.
        this.progressCircleObject = Instantiate(this.progressCirclePrefab, this.loadingCanvasObject.transform);
        this.progressCircleObject.SetActive(true);
    }

    public void CloseProgressCircle(string loadingKey)
    {
        if (this.currentLoadingKeys.ContainsKey(loadingKey) == false)
        {
            return;
        }

        if (this.currentLoadingKeys[loadingKey] <= 1)
        {
            this.currentLoadingKeys.Remove(loadingKey);
        }
        else
        {
            this.currentLoadingKeys[loadingKey] = this.currentLoadingKeys[loadingKey] - 1;
        }

        if (this.currentLoadingKeys.Count == 0)
        {
            Destroy(this.progressCircleObject);
            ActiveLoadingCanvas(false);
        }
    }

    /* In Multi Thread */

    public string GetLoadingFlagKey(string loadingKey)
    {
        string flagKey = loadingKey + "_" + this.currentLoadingFlags.Count;

        return flagKey;
    }

    public void SetLoadingFlag(string flagKey, bool isEnabled)
    {
        if (this.currentLoadingFlags.ContainsKey(flagKey) == false)
        {
            this.currentLoadingFlags.Add(flagKey, isEnabled);
        }
        else
        {
            this.currentLoadingFlags[flagKey] = isEnabled;
        }
    }

    public void ScheduleCloseProgressCircleInOtheThread(string loadingKey, string flagKey)
    {
        SetLoadingFlag(flagKey, false);
        StartCoroutine(CoroutineCloseProgressCircleInOtherThread(loadingKey, flagKey));
    }

    private IEnumerator CoroutineCloseProgressCircleInOtherThread(string loadingKey, string flagKey)
    {
        while (true)
        {
            if (this.currentLoadingFlags.ContainsKey(flagKey) == false)
            {
                yield break;
            }

            if (this.currentLoadingFlags[flagKey] == true)
            {
                break;
            }

            yield return null;
        }

        this.currentLoadingFlags.Remove(flagKey);
        CloseProgressCircle(loadingKey);
    }
}
