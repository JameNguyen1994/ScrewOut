using UnityEngine;
using UnityEditor;
using System.IO;

public class ScreenshotHotkey
{
    [MenuItem("Tools/Capture Screenshot %t")] // Ctrl + T
    private static void CaptureScreenshot()
    {
        // Gợi ý tên file mặc định
        string defaultName = "Screenshot_" + System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".png";

        // Hộp thoại cho người dùng chọn nơi lưu
        string filePath = EditorUtility.SaveFilePanel(
            "Save Screenshot",
            Application.dataPath,   // thư mục mặc định là Assets
            defaultName,
            "png"
        );

        // Nếu người dùng cancel thì thoát
        if (string.IsNullOrEmpty(filePath))
            return;

        // Chụp ảnh GameView
        ScreenCapture.CaptureScreenshot(filePath);

        Debug.Log($"📸 Screenshot saved to: {filePath}");
        EditorUtility.RevealInFinder(filePath); // mở thư mục chứa ảnh
    }
}
