using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ps.modules.leaderboard
{
    public class FPSCounter : MonoBehaviour
    {
        private float deltaTime = 0.0f;

        private void Start()
        {
            Application.targetFrameRate = 60; // Đặt mục tiêu FPS
        }
        //private void Update()
        //{
        //    // Tính toán thời gian giữa các khung hình
        //    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        //}

        //private void OnGUI()
        //{
        //    // Hiển thị FPS trên màn hình
        //    int width = Screen.width, height = Screen.height;

        //    // Tạo một style cho text
        //    GUIStyle style = new GUIStyle();

        //    // Kích thước font
        //    int fontSize = height * 2 / 100;
        //    style.fontSize = fontSize;
        //    style.normal.textColor = Color.white;

        //    // Tính toán FPS
        //    float fps = 1.0f / deltaTime;
        //    string text = $"FPS: {Mathf.Ceil(fps)}";

        //    // Vẽ text trên màn hình
        //    Rect rect = new Rect(10, 10, width, height * 2 / 100);
        //    GUI.Label(rect, text, style);
        //}
    }
}