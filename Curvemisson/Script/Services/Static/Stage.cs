using UnityEngine;

namespace Services.Static
{
    public static class Stage
    {
        public static int GetDefualtStageID()
        {
            return (int)Enum.Stage.InfoID.BalsanVillage;
        }

        public static Sprite GeStageIconSprite(int stageID)
        {
            int mapIconIndex = stageID - GetDefualtStageID();

            const string mapIconSpriteSheetName = "Texture/UI/MapIcon/MapIcon";
            Sprite mapIcon = Resources.LoadAll<Sprite>(mapIconSpriteSheetName)[mapIconIndex];

            return mapIcon;
        }

        public static int GetStageTextID(int stageID)
        {
            int textID = (int)Enum.GameText.Stage.None;

            switch (stageID)
            {
                case (int)Enum.Stage.InfoID.BalsanVillage:
                    textID = (int)Enum.GameText.Stage.Stage01;
                    break;
                default:
                    textID = (int)Enum.GameText.Stage.None;
                    break;
            }

            return textID;
        }
    }
}