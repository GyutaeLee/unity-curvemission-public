using UnityEngine;

public class CMObjectManager : MonoBehaviour
{
    /* 'parent'의 자식 중 이름이 'name'과 동일한 첫번째 GameObject를 찾는다. */
    public static GameObject FindGameObjectInAllChild(GameObject parent, string name, bool includeInactive)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(includeInactive);

        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            {
                return child.gameObject;
            }
        }

        Debug.Log("CMObjectManager : " + parent + " 에서 " + name + " 오브젝트를 찾지 못했습니다.");
        return null;
    }

    public static bool CheckNullAndFindTransformInAllChild(ref Transform target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).transform;

        return true;
    }
    public static bool CheckNullAndFindGameObjectInAllChild(ref GameObject target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive);

        return true;
    }

    public static bool CheckNullAndFindImageInAllChild(ref UnityEngine.UI.Image target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.UI.Image>();

        return true;
    }

    public static bool CheckNullAndFindSpriteRendererInAllChild(ref UnityEngine.SpriteRenderer target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.SpriteRenderer>();

        return true;
    }

    public static bool CheckNullAndFindAnimatorInAllChild(ref UnityEngine.Animator target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.Animator>();

        return true;
    }

    public static bool CheckNullAndFindTextInAllChild(ref UnityEngine.UI.Text target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.UI.Text>();

        return true;
    }

    public static bool CheckNullAndFindInputFieldInAllChild(ref UnityEngine.UI.InputField target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.UI.InputField>();

        return true;
    }

    public static bool CheckNullAndFindRectTransformInAllChild(ref UnityEngine.RectTransform target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.RectTransform>();

        return true;
    }

    public static bool CheckNullAndFindCameraInAllChild(ref UnityEngine.Camera target, GameObject parent, string name, bool includeInactive)
    {
        if (target != null)
        {
            return false;
        }

        target = CMObjectManager.FindGameObjectInAllChild(parent, name, includeInactive).GetComponent<UnityEngine.Camera>();

        return true;
    }
}
