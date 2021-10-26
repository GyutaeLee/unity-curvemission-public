using UnityEngine;

namespace Services.Gui
{
    public class TextSetter : MonoBehaviour
    {
        private UnityEngine.UI.Text text;

        public Enum.GameText.TextType textType;
        public int textID;

        private void Start()
        {
            SetText();
        }

        private void SetText()
        {
            this.text = this.GetComponent<UnityEngine.UI.Text>();
            if (this.text == null)
            {
                return;
            }

            this.text.text = GameText.Manager.Instance.GetText(this.textType, this.textID);
        }
    }
}