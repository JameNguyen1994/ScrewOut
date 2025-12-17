using EasyButtons;
using UnityEngine;

public class ShakeCamera : Singleton<ShakeCamera>
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float magnitude = 0.1f;
    [SerializeField] private Transform tfmShake;

    [Button("Start Shake")]
    public void StartShake()
    {
        StopAllCoroutines();
        StartCoroutine(Shake());
    }
    private System.Collections.IEnumerator Shake()
    {
        Vector3 originalPos = tfmShake.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            tfmShake.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        tfmShake.localPosition = originalPos;
    }
}
