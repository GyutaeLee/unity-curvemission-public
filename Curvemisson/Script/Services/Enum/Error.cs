namespace Services.Enum.Error
{
    public enum Type
    {
        None = 0,

        Game = 1,
        Server = 2,

        Max
    }

    public enum GameError
    {
        None = 0,

        ThreadWaitTimeOver = 100,
        ShopPurchaseError = 101,

        Max
    }

    public enum ServerError
    {

    }
}