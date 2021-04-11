public enum ESPRCollision
{
    None = 0,

    Direction = 1,
    Wall = 2,
    Item = 3,
    Booster = 4,
    Obstacle = 5,
    Lap = 6,

    Max,
}

public enum ESPRCollisionDirection
{
    None = 0,

    Forward = 1,
    Left = 2,
    Back = 3,
    Right = 4,

    Max,
}

public enum ESPRCollisionWall
{
    None = 0,

    Normal = 1,

    Max,
}

public enum ESPRCollisionItem
{
    None = 0,

    Coin = 1,

    Max,
}

// EBoosterLevel과 맞추어서 가야함
public enum ESPRCollisionBooster
{
    Booster_1 = 0,    // 부스터 레벨 1
    Booster_2 = 1,    // 부스터 레벨 2

    Max,
}

public enum ESPRCollisionObstacle
{
    Obstacle_1 = 1,   // 바위
    Obstacle_2 = 2,

    Max,
}

public enum ESPRCollisionLap
{
    None = 0,

    Normal = 1,
    Half = 2,
    Finish = 3,

    Max,
}