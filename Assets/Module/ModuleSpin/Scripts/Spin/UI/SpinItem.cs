using UnityEngine;
using UnityEngine.UI;

public class SpinItem : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private Text txtValue;

    public void Init(int value, Sprite sprIcon)
    {
        imgIcon.sprite = sprIcon;
        txtValue.text = $"+{value}";
    }
}