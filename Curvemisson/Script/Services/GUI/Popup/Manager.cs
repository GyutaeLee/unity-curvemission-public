using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace Services.Gui.Popup
{
    public class Manager : MonoBehaviour
    {
        public static Manager Instance { get; private set; }

        [SerializeField]
        private GameObject checkPopupObject;

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

        public void CloseCheckPopup()
        {
            this.checkPopupObject.SetActive(false);
        }
    }
}