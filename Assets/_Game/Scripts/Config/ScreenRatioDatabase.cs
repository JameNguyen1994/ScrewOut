using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScreenRatioDatabase", menuName = "Configs/Screen Ratio Database")]
public class ScreenRatioDatabase : ScriptableObject
{
    [System.Serializable]
    public class RatioConfig
    {
        public float Width;
        public float Height;

        public float PosY;
        public float Scale;
        public float PosX = 11.45f;

        public float Ratio => RatioService.CalculateAspectRatio(Width, Height);
    }

#if UNITY_EDITOR

    [EasyButtons.Button]
    private void TestApplyAspectRatioSettings()
    {
        BaseBox baseBox = FindFirstObjectByType<BaseBox>();

        if (baseBox != null)
        {
            baseBox.ApplyAspectRatioSettings();
            baseBox.CaculaterLstBoxPos();
        }
    }

#endif

    public List<RatioConfig> configs = new List<RatioConfig>();
}