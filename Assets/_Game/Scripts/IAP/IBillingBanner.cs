using UnityEngine;

public abstract class IBillingBanner : MonoBehaviour
{
    [SerializeField] protected string productId;
    public virtual void Init(){}
}
