using UnityEngine;

namespace Services.Vehicle
{
    public class Input : MonoBehaviour
    {
        private bool isScreenTouched;

        [SerializeField]
        private Controller controller;

        private void FixedUpdate()
        {
            UpdateScreenTouch();
        }

        private void UpdateScreenTouch()
        {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
        if (UnityEngine.Input.touchCount > 0)
        {
            this.isScreenTouched = true;
        }
        else
        {
            this.isScreenTouched = false;
        }
#else
            if (UnityEngine.Input.GetMouseButton(0) == true || UnityEngine.Input.GetKey(KeyCode.Space) == true)
            {
                this.isScreenTouched = true;
            }
#endif

            if (this.isScreenTouched == true)
            {
                this.controller.Acceleration();
            }
            else
            {
                this.controller.Deceleration();
                this.controller.Curve(true);
            }

            this.isScreenTouched = false;
        }

#if (UNITY_INCLUDE_TESTS)
        public void Test_TouchScreen()
        {
            this.isScreenTouched = true;
        }
#endif
    }
}