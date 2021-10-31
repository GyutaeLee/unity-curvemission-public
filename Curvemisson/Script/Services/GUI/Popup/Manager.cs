using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Services.Gui.Popup
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        [SerializeField]
        private GameObject checkPopupObject;

        [SerializeField]
        private Button checkPopupOkButton;
        private UnityAction checkPopupOkButtonUnityAction;

        [SerializeField]
        private Text checkPopupDescriptionText;

        private void Awake()
        {
            if (Manager.Instance == null)
            {
                Manager.Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        public void OpenCheckPopup(string descriptionText)
        {
            this.checkPopupObject.SetActive(true);
            this.checkPopupDescriptionText.text = descriptionText;
        }

        public void AddCheckPopupOkButtonListener(UnityAction unityAction)
        {
            this.checkPopupOkButtonUnityAction = unityAction;
            this.checkPopupOkButton.onClick.AddListener(this.checkPopupOkButtonUnityAction);
        }

        public void CloseCheckPopup()
        {
            this.checkPopupObject.SetActive(false);

            if (this.checkPopupOkButtonUnityAction != null)
            {
                this.checkPopupOkButton.onClick.RemoveListener(this.checkPopupOkButtonUnityAction);
                this.checkPopupOkButtonUnityAction = null;
            }

            Services.Sound.Effect.Manager.Instance.Play(Enum.Sound.Effect.Type.Gui, (int)Enum.Sound.Effect.Gui.ClickButton_1);
        }
    }
}