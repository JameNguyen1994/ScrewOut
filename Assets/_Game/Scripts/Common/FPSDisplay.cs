using System;
using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    private int frameCount = 0;
    private float elapsedTime = 0f;
    private float fps = 0f;
    public float updateInterval = 1.0f;
    
    public int fontSize = 30;
    public Color fontColor = Color.red;

    private GUIStyle guiStyle;

    //private void Awake()
    //{
    //    DontDestroyOnLoad(gameObject);
        
    //    guiStyle = new GUIStyle();
    //    guiStyle.fontSize = fontSize;
    //    guiStyle.normal.textColor = fontColor;
    //}

    //void Update()
    //{
    //    frameCount++;
    //    elapsedTime += Time.unscaledDeltaTime;

    //    if (elapsedTime >= updateInterval)
    //    {
    //        fps = frameCount / elapsedTime;
    //        frameCount = 0;
    //        elapsedTime = 0f;
    //    }
    //}

    //void OnGUI()
    //{
    //    GUI.Label(new Rect(10, 10, 300, 50), $"FPS: {fps:F2}", guiStyle);
    //}
}