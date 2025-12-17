using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardView : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtValue;

    public void UpdateUI(string value, Sprite icon = null)
    {
        if (icon != null)
        {
            imgIcon.sprite = icon;
        }

        txtValue.text = value;
    }

    public void UpdateUI(object value, Sprite icon = null)
    {
        if (icon != null)
        {
            imgIcon.sprite = icon;
        }

        txtValue.text = value.ToString();
    }
}