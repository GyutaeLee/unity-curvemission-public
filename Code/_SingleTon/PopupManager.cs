using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour, ICMInterface
{
    public static PopupManager instance = null;

    private List<GameObject> popupObjects;

    private GameObject popupCanvasObject;
        
    private GameObject checkPopupPrefab;
    private GameObject checkPopupObject;

    private void Awake()
    {
        InitInstance();
    }

    private void Start()
    {
        PrepareBaseObjects();
        InitPopupManager();
    }

    private void InitInstance()
    {
        if (PopupManager.instance == null)
        {
            PopupManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void PrepareBaseObjects()
    {
        this.popupCanvasObject = CMObjectManager.FindGameObjectInAllChild(this.gameObject, "popupCanvas", true);

        this.popupObjects = new List<GameObject>();

        this.checkPopupPrefab = Resources.Load("security-related") as GameObject;
    }

    private void InitPopupManager()
    {

    }

    private void ActivePopupCanvas(bool isEnabled)
    {
        if (isEnabled == true)
        {
            if (this.popupCanvasObject.activeSelf == true)
            {
                return;
            }
        }
        else
        {
            if (IsPopupObjectInUse() == true)
            {
                return;
            }
        }

        this.popupCanvasObject.SetActive(isEnabled);
    }

    private bool IsPopupObjectInUse()
    {
        for (int i = 0; i < this.popupObjects.Count; i++)
        {
            if (this.popupObjects[i].activeSelf == true)
            {
                return true;
            }
        }

        return false;
    }

    public void OpenCheckPopup(string text)
    {
        ActivePopupCanvas(true);

        this.checkPopupObject = Instantiate(this.checkPopupPrefab, this.popupCanvasObject.transform);
        this.checkPopupObject.SetActive(true);
        this.popupObjects.Add(this.checkPopupObject);

        this.checkPopupObject.transform.Find("BTN_CheckPopup").GetComponent<Button>().onClick.AddListener(() => CloseCheckPopup());
        this.checkPopupObject.transform.Find("TXT_CheckPopup").GetComponent<Text>().text = text;
    }

    public void CloseCheckPopup()
    {
        this.popupObjects.Remove(this.checkPopupObject);

        Destroy(this.checkPopupObject);
        ActivePopupCanvas(false);
    }
}
