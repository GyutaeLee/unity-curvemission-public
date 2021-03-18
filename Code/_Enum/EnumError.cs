public enum EGameError
{
    ShopPurchaseError = 101,

    MAX
}

public enum EServerError
{

}

public class EnumError
{
    public static string GetEGameErrorCode(EGameError eGameError)
    {
        int errorCode = (int)eGameError;

        return errorCode.ToString();
    }
}