namespace Services.Delegate
{
    public delegate void delegateProcessAfterPurchase();

    public delegate bool delegateGetFlag();

    public delegate void delegateLoadScene(string sceneName);
    public delegate void delegateActiveFlag();

    public delegate void delegatePurchaseResult(Services.Enum.Shop.PurchaseResultType purchaseResultType);
}