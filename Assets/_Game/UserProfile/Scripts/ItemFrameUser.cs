using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserProfile
{
    public class ItemFrameUser : ItemUserProfileBase
    {
        [SerializeField] private Color color;
        [SerializeField] private Image imgBack;
        public void Init(int id, Sprite spriteItem,Color colorBack, ItemState itemState)
        {
            base.Init(id, spriteItem, itemState);
            this.color = colorBack;
            imgBack.color = color;
        }

        public override void UpdateState(ItemState itemState)
        {
            this.itemState = itemState;
            gobjLock.SetActive(false);
            gobjCurrent.SetActive(false);

            switch (itemState)
            {
                case ItemState.Lock:
                    gobjLock.SetActive(true);
                    break;
                case ItemState.Unlock:
                    break;
                case ItemState.Select:
                    gobjCurrent.SetActive(true);
                    break;
            }
        }
    }
}
