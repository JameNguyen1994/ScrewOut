using UnityEngine;
using UnityEngine.UI;

public class TabBaseLB : MonoBehaviour
{
    [SerializeField] private int index;

    [SerializeField] protected Image tabActive;
    [SerializeField] protected Sprite sprOn;
    [SerializeField] protected Sprite sprOff;

    public virtual void Init()
    {
        tabActive.sprite = sprOff;

    }
    public void GoToTab()
    {
        tabActive.sprite = sprOn;
        Show();
    }
    public void OnExitTab()
    {
        tabActive.sprite = sprOff;
        Hide();
    }

    protected virtual void Show()
    {
        Debug.Log($"Show tab {index}");
    }
    protected virtual void Hide()
    {
        Debug.Log($"Hide tab {index}");

    }
}
