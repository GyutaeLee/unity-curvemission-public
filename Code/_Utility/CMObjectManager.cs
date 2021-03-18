using UnityEngine;

public class CMObjectManager : MonoBehaviour
{
    /* 'parent'의 자식 중 이름이 'name'과 동일한 첫번째 GameObject를 찾는다. */
    public static GameObject FindGameObjectInAllChild(GameObject parent, string name, bool bIncludeInactive)
    {
        Transform[] allChildren = parent.GetComponentsInChildren<Transform>(bIncludeInactive);

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
}
