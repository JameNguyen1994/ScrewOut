using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserProfile
{
    public class ItemAvatarUser : ItemUserProfileBase
    {
        public void Init(int id, Sprite spriteItem, ItemState itemState)
        {
            base.Init(id, spriteItem, itemState);
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
