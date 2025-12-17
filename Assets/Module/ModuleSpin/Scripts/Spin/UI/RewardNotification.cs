using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RewardNotification : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TMPro.TextMeshProUGUI txtValue;

    public void Show(string value, Sprite sprIcon)
    {
        imgIcon.sprite = sprIcon;
        txtValue.text = string.IsNullOrEmpty(value) ? string.Empty : $"<size=60>x</size>{value}";
        gameObject.SetActive(true);
        StartCoroutine(WaiteForEnd());
    }

    public void ShowMyText(string value, Sprite sprIcon, float delayTime)
    {
        imgIcon.sprite = sprIcon;
        txtValue.text = string.IsNullOrEmpty(value) ? string.Empty : $"{value}";
        gameObject.SetActive(true);
        StartCoroutine(WaiteForEnd(delayTime));
    }

    private IEnumerator WaiteForEnd(float delayTime = 1)
    {
        yield return new WaitForSeconds(delayTime);
        gameObject.SetActive(false);
    }
}