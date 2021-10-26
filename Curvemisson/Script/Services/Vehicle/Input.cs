using UnityEngine;

namespace Services.Vehicle
{
    public class Input : MonoBehaviour
    {
        [SerializeField]
        private Controller controller;

        private void FixedUpdate()
        {
            UpdateScreenTouch();
        }

        private void UpdateScreenTouch()
        {
            bool isScreenTouched = false;

#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        if (UnityEngine.Input.touchCount > 0)
        {
            isScreenTouched = true;
        }
        else
        {
            isScreenTouched = false;
        }
#else
            if (UnityEngine.Input.GetMouseButton(0) == true || UnityEngine.Input.GetKey(KeyCode.Space) == true)
            {
                isScreenTouched = true;
            }
            else
            {
                isScreenTouched = false;
            }
#endif

            if (isScreenTouched == true)
            {
                this.controller.Acceleration();
            }
            else
            {
                this.controller.Deceleration();
                this.controller.Curve(true);
            }
        }
    }
}