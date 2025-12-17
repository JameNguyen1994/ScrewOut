using UnityEngine;
using UnityEngine.UI;

public class LifeState : MonoBehaviour
{
    [SerializeField] private Image imgLife;
    [SerializeField] private Sprite sprOn;
    [SerializeField] private Sprite sprOff;

    public void InitState(bool isOn)
    {
        if (imgLife == null)
        {
            Debug.LogError("Image component is not assigned.");
            return;
        }
        imgLife.sprite = isOn ? sprOn : sprOff;
    }
}
