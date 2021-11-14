using UnityEngine;

namespace Services.Vehicle
{
    public class Input : MonoBehaviour
    {
        public bool IsScreenTouched { get; private set; }

        [SerializeField]
        private Controller controller;

        private void Start()
        {
            if (Static.Replay.IsReplayMode == true)
            {
                this.enabled = false;
            }
        }

        private void FixedUpdate()
        {
            UpdateScreenTouch();
        }

        private void UpdateScreenTouch()
        {
#if (UNITY_ANDROID || UNITY_IOS) && (!UNITY_EDITOR)
            if (UnityEngine.Input.touchCount > 0)
            {
                this.IsScreenTouched = true;
            }
            else
            {
                this.IsScreenTouched = false;
            }
#else
            if (UnityEngine.Input.GetMouseButton(0) == true || UnityEngine.Input.GetKey(KeyCode.Space) == true)
            {
                this.IsScreenTouched = true;
            }
#endif

            if (this.IsScreenTouched == true)
            {
                this.controller.Acceleration();
            }
            else
            {
                this.controller.Deceleration();
                this.controller.Curve(true);
            }

            this.IsScreenTouched = false;
        }

#if (UNITY_INCLUDE_TESTS)
        public void Test_TouchScreen()
        {
            this.IsScreenTouched = true;
        }
#endif
    }
}