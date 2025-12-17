using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TotalQuestProcess : MonoBehaviour
{
    [SerializeField] private Image imgTotalProcess;
    [SerializeField] private ParticleSystem parProcess;
    [SerializeField] private TextMeshProUGUI txtProcess;
    [Header("Time per point")]
    [SerializeField] private float countWait = 0.1f;
    [SerializeField] private float countWaitMin = 0.1f;
    [SerializeField] private float countWaitMax = 0.1f;
    [Header("Current and Max Points")]
    [SerializeField] private int currentPoint;
    [SerializeField] private int maxPoint;
    [SerializeField] private int textPoint;
    [SerializeField] private RectTransform rtfmFill;
    [SerializeField] private Vector2Int size;
    UnityAction<int> actionOnChangeProcess;
    Coroutine coroutineAddCount;

    public void SetAction(UnityAction<int> actionOnChangeProcess)
    {
        this.actionOnChangeProcess = actionOnChangeProcess;
    }
    public void Addcount(int point)
    {
        parProcess.Play();
        textPoint += point;
        AudioController.Instance.PlaySound(SoundName.GiftProcess);
        if (coroutineAddCount == null)
        {
            coroutineAddCount = StartCoroutine(CountPoint());
        }
    }
    public void InitBar(int currentPoint, int maxPoint)
    {
        this.maxPoint = maxPoint;
        this.currentPoint = currentPoint;
       // imgTotalProcess.fillAmount = (float)currentPoint / maxPoint;
        rtfmFill.sizeDelta = new Vector2(((float)currentPoint / maxPoint) * size.x, size.y);
        txtProcess.text = $"{currentPoint}/{maxPoint}";
    }

    IEnumerator CountPoint()
    {
        while (textPoint > 0)
        {
            textPoint--;
            currentPoint++;
            float process = (float)currentPoint / maxPoint;
            txtProcess.text = $"{currentPoint}/{maxPoint}";
            //imgTotalProcess.fillAmount = process;
            rtfmFill.sizeDelta = new Vector2(process * size.x, size.y);
            countWait = GetTime(textPoint);
            actionOnChangeProcess?.Invoke(currentPoint);
            yield return new WaitForSeconds(countWait);
        }
        coroutineAddCount = null;
    }
    private float GetTime(int count)
    {
        float time = Mathf.Clamp(countWaitMax / count, countWaitMin, countWaitMax);
        return time;
    }
}
