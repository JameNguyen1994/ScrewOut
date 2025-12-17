using UnityEngine;

[System.Serializable]
public class FileHotkeyData
{
    public string filePath;   // Asset path
    public KeyCode key;       // Hotkey
    public bool isVideo;      // True = video, False = image
}
