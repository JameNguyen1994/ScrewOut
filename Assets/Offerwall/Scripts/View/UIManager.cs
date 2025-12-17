using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private UIBase notify;
    [SerializeField] private UIBase popupOfferList;
    [SerializeField] private UIBase popupOfferCompleted;
    
    public UIBase Notify => notify;
    public UIBase PopupOfferList => popupOfferList;
    public UIBase PopupOfferCompleted => popupOfferCompleted;
}
