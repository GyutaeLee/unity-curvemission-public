namespace Services.Static
{
    public static class Game
    {
        public static bool IsGameProceeding()
        {
            if (Scene.SingleRacing.GameLogic.Instance == null)
                return false;

            return Services.Scene.SingleRacing.GameLogic.Instance.IsGameProceeding();
        }
    }
}