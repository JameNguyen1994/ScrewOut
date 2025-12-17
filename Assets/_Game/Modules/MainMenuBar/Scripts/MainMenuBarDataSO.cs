using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MainMenuBar
{
    [CreateAssetMenu(fileName = "MainMenuBarDataSO", menuName = "SO/MainMenuBar/MainMenuBarDataSO")]
    public class MainMenuBarDataSO : ScriptableObject
    {
        public float selectedScale = 1.3f;
        public float timeAnimation = 1.3f;
        public Vector2 normalIconSize;
        public Vector2 selectedIconSize;
        public int defaultIndexItembar = 1;
        public List<ItemBarData> data;
    }

    [System.Serializable]
    public class ItemBarData
    {
        public string name;
        public int levelUnlock;
        public Sprite spriteSelected;
        public Sprite spriteNormal;
        public Sprite spriteLock;
    }
}
