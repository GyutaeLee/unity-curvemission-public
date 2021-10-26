using UnityEngine;

namespace Services.Useful
{
    public class ObjectFinder
    {
        /* 'parent'의 자식 중 이름이 'name'과 동일한 첫번째 GameObject를 찾는다. */
        public static GameObject GetGameObjectInAllChild(GameObject parent, string name, bool includeInactive)
        {
            if (parent == null)
            {
                Debug.Log("ObjectFinder : parent 오브젝트가 null 입니다.");
                return null;
            }

            Transform[] allChildren = parent.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform child in allChildren)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
            }

            Debug.Log("ObjectFinder : " + parent + " 에서 " + name + " 오브젝트를 찾지 못했습니다.");
            return null;
        }

        public static bool FindGameObjectInAllChild(ref GameObject target, GameObject parent, string name, bool includeInactive)
        {
            if (target != null)
            {
                Debug.Log("ObjectFinder : target 오브젝트가 null이 아닙니다.");
                return false;
            }

            if (parent == null)
            {
                Debug.Log("ObjectFinder : parent 오브젝트가 null 입니다.");
                return false;
            }

            GameObject gameObject = ObjectFinder.GetGameObjectInAllChild(parent, name, includeInactive);
            if (gameObject == null)
            {
                return false;
            }

            target = ObjectFinder.GetGameObjectInAllChild(parent, name, includeInactive);

            return true;
        }

        public static bool FindComponentInAllChild<T>(ref T component, GameObject parent, string name, bool includeInactive)
        {
            if (component != null)
            {
                Debug.Log("ObjectFinder : component가 null이 아닙니다.");
                return false;
            }

            if (parent == null)
            {
                Debug.Log("ObjectFinder : parent 오브젝트가 null 입니다.");
                return false;
            }

            GameObject gameObject = ObjectFinder.GetGameObjectInAllChild(parent, name, includeInactive);

            if (gameObject == null)
            {
                return false;
            }

            component = gameObject.GetComponent<T>();

            return true;
        }
    }
}