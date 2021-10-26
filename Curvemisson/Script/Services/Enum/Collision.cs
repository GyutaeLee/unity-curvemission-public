namespace Services.Enum.Collision
{
    public enum Type
    {
        None = 0,

        Direction = 1,
        Wall = 2,
        FieldItem = 3,
        Booster = 4,
        Obstacle = 5,
        Lap = 6,

        Max,
    }

    public enum Direction
    {
        None = 0,

        Forward = 1,
        Left = 2,
        Back = 3,
        Right = 4,

        Max,
    }

    public enum Wall
    {
        None = 0,

        Normal = 1,

        Max,
    }

    public enum FieldItem
    {
        None = 0,

        Coin = 1,

        Max,
    }

    public enum Booster
    {
        Booster_1 = 0,    // 부스터 레벨 1
        Booster_2 = 1,    // 부스터 레벨 2

        Max,
    }

    public enum Obstacle
    {
        None = 0,

        Obstacle_1 = 1,   // 바위
        Obstacle_2 = 2,

        Max,
    }

    public enum Lap
    {
        None = 0,

        Normal = 1,
        HalfOfFinish = 2,
        Finish = 3,

        Max,
    }
}