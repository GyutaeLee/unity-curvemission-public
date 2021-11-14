namespace Services.Enum.RequestResult
{
    public enum Type
    {
        None = 0,

        Game = 1,
        Server = 2,

        Max
    }

    public enum Game
    {
        None = 0,
        Success = 1,

        ThreadWaitTimeOver = 100,
        ShopPurchaseError = 101,

        Max
    }

    public enum Server
    {
        None = 0,
        Success = 1,

        DownloadRecordingFileFailaure = 100,
    }
}