namespace Services.Enum.Sound.Effect
{
    public enum Type
    {
        None = 0,

        Etc = 1,
        Gui = 2,
        Collision = 3,
        Car = 4,

        Max,
    }

    public enum Etc
    {
        None = 0,

        CountDown = 1,
        FinishGame = 2,
        LastLap = 3,

        Max,
    }

    public enum Gui
    {
        None = 0,

        ClickButton_1 = 1,
        NumberTick = 2,
        FinishLoopTick = 3,

        Max,
    }

    public enum Collision
    {
        None = 0,

        Rock_1 = 1,
        Coin_1 = 2,
        Booster_1 = 3,
        Booster_2 = 4,

        Max,
    }

    public enum Car
    {
        None = 0,

        Engine_1 = 1,
        Curve_1 = 2,

        Max,
    }
}