using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace WeeklyQuest
{
    [CreateAssetMenu(fileName = "GiftSpriteSO", menuName = "Scriptable Objects/GiftSpriteSO")]
    public class GiftSpriteSO : ScriptableObject
    {
        public List<ImageGiftData> giftSprites;

        public ImageGiftData GetSprite(int index)
        {
            if (index < 0 || index >= giftSprites.Count)
            {
                Debug.LogError($"ID {index} is out of bounds for giftSprites list.");
                if (giftSprites.Count > 0)
                {
                    Debug.LogWarning("Returning the first sprite as a fallback.");
                    return giftSprites[giftSprites.Count - 1];
                }
                return null;
            }
            return giftSprites[index];
        }
    }
    [System.Serializable]
    public class ImageGiftData
    {
        public Sprite sprLid;
        public Sprite sprBox;
        public float scale;
    }
}