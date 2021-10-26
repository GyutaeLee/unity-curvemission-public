namespace Services.Enum.Vehicle
{
    public enum BoosterLevel
    {
        None = 0,
        
        Level_1 = 1,
        Level_2 = 2,

        Max
    }

    public enum ObstacleLevel
    {
        None = 0,

        Level_1 = 1,
        Level_2 = 2,

        Max
    }

    public enum VehicleState
    {
        None = 0,

        Forward = 1,
        Left = 2,
        Back = 3,
        Right = 4,

        Death = 5,
        Finish = 6,

        Max,
    }
}