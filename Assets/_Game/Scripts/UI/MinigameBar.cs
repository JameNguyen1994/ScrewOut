using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class MinigameBar : MonoBehaviour
{
    [SerializeField] private List<MiniEventRange> lstRange;

    [SerializeField] private Image imgPercent;
    [SerializeField] private Image imgCursor;
    [SerializeField] private float cursorSpeed = 0.2f; // 0.3f
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private List<RectTransform> lstRectransform;
    [SerializeField] private float minScale = 1f; // 0.3f
    [SerializeField] private float maxScale = 1.8f; // 0.3f



    private Vector2 _cursorAnchor;
    private float _totalSize;
    [SerializeField] private float _startPos;
    [SerializeField] private float _endPos;
    [SerializeField]
    private float _cursorValue;
    private float _direction;
    private bool _canTouch;
    private bool _isStop;

    public bool CanTouch => _canTouch;
    public float CurrentValue => Mathf.Clamp01(_cursorValue);
    public bool IsStop => _isStop;

    private float prevValue = -1;
    public UnityAction<float> OnValueChanged;

    [EasyButtons.Button]
    public void Setup()
    {
        Debug.Log("MinigameBar Setup");
        _cursorAnchor = imgCursor.rectTransform.anchoredPosition;
        _totalSize = imgPercent.rectTransform.sizeDelta.x * 0.95f;

        _startPos = 16.35f;
        _endPos = -16.35f;
        _cursorValue = 0;
        _direction = 1;
        _isStop = false;

        for (int i = 0; i < lstRange.Count; i++)
        {
            lstRange[i].txtValue.text = $"x{lstRange[i].value}";
        }
    }

    [EasyButtons.Button]
    public void StartEvent()
    {
        StartCoroutine(DelayTouch(2f));
    }


    private void Update()
    {
        if (_isStop) return;

        CalculateCursorValue();
        var startAnchoredPos = lstRectransform[0].anchoredPosition;
        var endAnchoredPos = lstRectransform[lstRectransform.Count - 1].anchoredPosition;
        var newAnchoredPos = Vector2.Lerp(startAnchoredPos, endAnchoredPos, _cursorValue);
        imgCursor.rectTransform.anchoredPosition = newAnchoredPos;
        // lấy vị trí X của cursor
        float cursorX = imgCursor.rectTransform.anchoredPosition.x;
        var distanceTotal = Mathf.Abs(endAnchoredPos.x - startAnchoredPos.x);
        for (int i = 0; i < lstRectransform.Count; i++)
        {
            float cardX = lstRectransform[i].anchoredPosition.x;
            float distance = Mathf.Abs(cardX - cursorX);

            // tính t: gần → 1, xa → 0
            float t = Mathf.InverseLerp(distanceTotal, 0, distance);

            float scale = Mathf.Lerp(minScale, maxScale, t);
            lstRectransform[i].localScale = Vector3.one * scale;
        }
    }

    private void CalculateCursorValue()
    {
        _cursorValue += (Time.deltaTime + curve.Evaluate(_cursorValue)) * cursorSpeed * _direction;

        if (_cursorValue >= 1)
        {
            _cursorValue = 1;
            _direction = -1;
        }
        else if (_cursorValue <= 0)
        {
            _cursorValue = 0;
            _direction = 1;
        }

        var value = GetValue();

        if (!Mathf.Approximately(prevValue, value))
        {
          //  print($"x{value}");
            prevValue = value;
            OnValueChanged?.Invoke(prevValue);
        }
    }

    IEnumerator DelayTouch(float dur)
    {
        _canTouch = false;
        yield return new WaitForSeconds(dur);
        _canTouch = true;
    }

    public void OnStopEvent(out float value)
    {
        _isStop = true;

        value = GetValue();
    }

    public float GetValue()
    {
        var startAnchoredPos = lstRectransform[0].anchoredPosition;
        var endAnchoredPos = lstRectransform[lstRectransform.Count - 1].anchoredPosition;
        var totalDistance = Mathf.Abs(endAnchoredPos.x - startAnchoredPos.x);
        var cursorX = imgCursor.rectTransform.anchoredPosition.x;

        var valuePercent = Mathf.InverseLerp(startAnchoredPos.x, endAnchoredPos.x, cursorX);
        var nearestRange = lstRange[0];
        var nearestIndex = 0;
        for (var i = 1; i < lstRange.Count; i++)
        {
            var range = lstRange[i];
            if (cursorX >= range.range.x && cursorX <= range.range.y)
            {
                nearestRange = range;
                nearestIndex = i;
                break;
            }
        }
      //  Debug.Log($"GetValue: {valuePercent} - nearestRange: {nearestRange.range.x} - {nearestRange.range.y} - value: {nearestRange.value}");
        return lstRange[nearestIndex].value;
    }

}

[System.Serializable]
public class MiniEventRange
{
    public Vector2 range;
    public float value;
    public Text txtValue;
}
