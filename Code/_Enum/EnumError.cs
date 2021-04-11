public enum EGameError
{
    None = 0,

    ThreadWaitTimeOver = 100,
    ShopPurchaseError = 101,

    MAX
}

public enum EServerError
{

}

public class EnumError
{
    public static string GetEGameErrorCodeString(EGameError eGameError)
    {
        int errorCode = (int)eGameError;

        return errorCode.ToString();
    }
}