using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace UserProfile
{
    public class NotiUnlock : Singleton<NotiUnlock>
    {
        [SerializeField] private Image imgFrame;
        [SerializeField] private Text txtFrame;

        private void Start()
        {
            imgFrame.DOFade(0, 0);
            txtFrame.DOFade(0, 0);
        }
        public void ShowNoti(string noti)
        {
            txtFrame.text = noti;
            txtFrame.DOKill();
            imgFrame.DOKill();
            imgFrame.DOFade(1, 1).OnComplete(() =>
             {
                 imgFrame.DOFade(0, 1).SetDelay(2);
             });
            txtFrame.DOFade(1, 1).OnComplete(() =>
            {
                txtFrame.DOFade(0, 1).SetDelay(2);
            });
        }
    }
}