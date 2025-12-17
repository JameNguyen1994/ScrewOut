using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ps.modules.leaderboard
{
    public class UserItemUIBase : MonoBehaviour
    {
        [SerializeField] protected Image avatar;
        [SerializeField] protected Image border;
        [SerializeField] protected Image iconStar;
        [SerializeField] protected TMP_Text txtIndex;
        [SerializeField] protected TMP_Text txtName;
        [SerializeField] protected TMP_Text txtPoint;
        [SerializeField] protected int currentPoint;
        [SerializeField] protected int index;

        public virtual void SetData(int index, string userName, int points, Sprite avatarSprite, Sprite borderSprite, bool isPlayerData = false)
        {
            this.currentPoint = points;
            
            this.index = index;
        }
        public RectTransform GetRectTransformStar()
        {
            return iconStar.rectTransform;
        }
        public async UniTask DOPlayPoint(int newIndex, int points, float time = 0.5f)
        {
            DOVirtual.Float(currentPoint, points, time, (value) =>
             {
                 txtPoint.text = ((int)value).ToString();
                 Debug.Log($"Value: {txtPoint.text}");
             });
            await DOVirtual.Float(int.Parse(txtIndex.text), newIndex, time, (value) =>
            {
                txtIndex.text = ((int)value + 1).ToString();
            }).ToUniTask();
            currentPoint = points;
        }

        public async UniTask DOPlayNewIndex(int newIndex, float time = 0.5f)
        {
            await DOVirtual.Float(int.Parse(txtIndex.text), newIndex, time, (value) =>
            {
                txtIndex.text = ((int)value + 1).ToString();
            }).ToUniTask();
        }
    }
}