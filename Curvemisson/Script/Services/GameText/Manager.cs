using Services.Enum.GameText;

namespace Services.GameText
{
    public class Manager : Util.Singleton<Manager>
    {
        private void Start()
        {
            InitializeGameText();
        }

        private void InitializeGameText()
        {
            int beginIndex = (int)TextType.None + 1;
            int endIndex = (int)TextType.Max;
            for (int i = beginIndex; i < endIndex; i++)
            {
                TextType textType = (TextType)i;
                Util.Csv.Text.Reader reader = new Util.Csv.Text.Reader(textType.ToString());
                reader.Read();
                reader.StoreDatasInStorage();
            }
        }

        public string GetText(TextType textType, int infoID)
        {
            int countryIndex = (int)LanguageType.Kor; // TODO : 추후에 랭귀지 설정 하면서 수정 필요
            string gameText = Util.Csv.Text.Storage.Instance.GetText(textType.ToString(), infoID, countryIndex);

            return gameText;
        }
    }
}