public interface IPurchaseHandler
{
    void OnPurchaseSuccess(string productID, object data);
    void OnPurchaseCancel(string productID, object data);
    void OnPurchaseError(string productID, string error);
}
