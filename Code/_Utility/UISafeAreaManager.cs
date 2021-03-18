using UnityEngine;

/*
 * "TOP" UI 오브젝트에 붙이면 자동으로 Safe Area 대응하는 코드
 */
public class UISafeAreaManager : MonoBehaviour, ICMInterface
{
    private RectTransform rectTransform;

    private void Start()
    {
        PrepareBaseObjects();
        InitUISafeAreaManager();
        ApplySafeAreaPosition();
    }

    public void PrepareBaseObjects()
    {
        if (this.rectTransform == null)
        {
            this.rectTransform = this.gameObject.GetComponent<RectTransform>();
        }
    }

    private void InitUISafeAreaManager()
    {

    }

    public void ApplySafeAreaPosition()
    {
        Rect safeArea = Screen.safeArea;

        // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        // use origin anchor.x
        anchorMin.x = this.rectTransform.anchorMin.x;
        anchorMax.x = this.rectTransform.anchorMax.x;

        anchorMin.y /= Screen.height;
        anchorMax.y /= Screen.height;

        this.rectTransform.anchorMin = anchorMin;
        this.rectTransform.anchorMax = anchorMax;
    }
}