using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txtLevel;
    [SerializeField] private TextMeshProUGUI txtProgress;
    [SerializeField] private Image imgFillAmount;
    [SerializeField] private ParticleSystem parPoint;
    [SerializeField] private Transform tfmProcess;
    private int amountCount;
    private int currentCount = 0;
    private int totalScrews
    {
        get
        {
            if (LevelController.Instance.Level != null)
            {
                return LevelController.Instance.Level.TotalScrew;
            }

            return 1;
        }
    }

    public void InitLevel(int level)
    {
        print($"[LevelProgressBar] InitLevel {level}");
        txtLevel.text = $"LV.{level}";
    }
    public void HideUI()
    {
        tfmProcess.DOMoveY(tfmProcess.position.y + 400, 0.5f).SetEase(Ease.InBack);
    }
    public void SetProgress(int plus)
    {
        amountCount += plus;
        
        if (amountCount > totalScrews)
        {
            amountCount = totalScrews;
        }

        DoFillAmount(totalScrews);
    }

    void DoFillAmount(int total)
    {
        parPoint.Play();
        Debug.Log($"[LevelProgressBar] DoFillAmount {amountCount}/{total}");
        DOVirtual.Int(currentCount, amountCount, 0.3f, value =>
        {
           // currentCount = value;
            //txtProgress.text = $"{currentCount}/{total}";
            var percent = (value * 1.0f / total) * 100;
            percent = Mathf.RoundToInt(percent);
            txtProgress.text = $"{percent}%";
        });

        float amount = amountCount * 1.0f / total;
        imgFillAmount.DOFillAmount(amount, 0.4f);
    }
    public float GetProgress()
    {
        return imgFillAmount.fillAmount;
    }
}
