using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingManager : MonoBehaviour
{
    private static LoadingManager _instance = null;
    public static LoadingManager instance
    {
        get
        {
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

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

    private void PrepareBaseObjects()
    {
        CMObjectManager.CheckNullAndFindGameObjectInAllChild(ref this.loadingCanvasObject, this.gameObject, "loadingCanvas", true);

        this.currentLoadingFlags = new Dictionary<string, bool>();
        this.currentLoadingKeys = new Dictionary<string, int>();

        this.progressCirclePrefab = Resources.Load("security-related") as GameObject;
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

    public void OpenProgressCircle(string loadingKey)
    {
        IncreaseCurrentLoadingKey(loadingKey);

        if (IsProgressCircleEnable() == true)
        {
            return;
        }

        ActiveLoadingCanvas(true);

        this.progressCircleObject = Instantiate(this.progressCirclePrefab, this.loadingCanvasObject.transform);
        this.progressCircleObject.SetActive(true);
    }

    public void CloseProgressCircle(string loadingKey)
    {
        if (this.currentLoadingKeys.ContainsKey(loadingKey) == false)
        {
            return;
        }

        DecreaseCurrentLoadingKey(loadingKey);

        if (this.currentLoadingKeys.Count == 0)
        {
            Destroy(this.progressCircleObject);
            ActiveLoadingCanvas(false);
        }
    }

    private object lock_object_increaseCurrentLoadingKey = new object();
    private void IncreaseCurrentLoadingKey(string loadingKey)
    {
        lock (lock_object_increaseCurrentLoadingKey)
        {
            if (this.currentLoadingKeys.ContainsKey(loadingKey) == false)
            {
                this.currentLoadingKeys.Add(loadingKey, 0);
            }
            this.currentLoadingKeys[loadingKey] = this.currentLoadingKeys[loadingKey] + 1;
        }
    }

    private object lock_object_decreaseCurrentLoadingKey = new object();
    private void DecreaseCurrentLoadingKey(string loadingKey)
    {
        lock (lock_object_decreaseCurrentLoadingKey)
        {
            if (this.currentLoadingKeys[loadingKey] <= 1)
            {
                this.currentLoadingKeys.Remove(loadingKey);
            }
            else
            {
                this.currentLoadingKeys[loadingKey] = this.currentLoadingKeys[loadingKey] - 1;
            }
        }
    }

    private bool IsProgressCircleEnable()
    {
        if (this.progressCircleObject == null)
        {
            return false;
        }

        if (this.progressCircleObject.activeSelf == true)
        {
            return true;
        }

        return false;
    }

    /* In Multi Thread */

    public string GetLoadingFlagKey(string loadingKey)
    {
        string flagKey = loadingKey + "_" + this.currentLoadingFlags.Count;

        return flagKey;
    }

    public void CloseScheduledProgressCircle(string flagKey)
    {
        SetLoadingFlag(flagKey, false);
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

    public void ScheduleCloseProgressCircle(string loadingKey, string flagKey)
    {
        SetLoadingFlag(flagKey, true);
        StartCoroutine(CoroutineCloseProgressCircle(loadingKey, flagKey));
    }

    private IEnumerator CoroutineCloseProgressCircle(string loadingKey, string flagKey)
    {
        while (true)
        {
            if (this.currentLoadingFlags.ContainsKey(flagKey) == false)
            {
                yield break;
            }

            if (this.currentLoadingFlags[flagKey] == false)
            {
                break;
            }

            yield return null;
        }

        this.currentLoadingFlags.Remove(flagKey);
        CloseProgressCircle(loadingKey);
    }
}
