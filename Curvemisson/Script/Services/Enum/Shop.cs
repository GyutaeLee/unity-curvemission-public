namespace Services.Enum.Shop
{
    public enum PurchaseResultType
    {
        None = 0,

        Success = 1,
        Fail = 2,
        NotEnoughCoin = 2,
        AlreadyOwned = 3,
        FirebaseDBFail = 4,

        Max,
    }
}
