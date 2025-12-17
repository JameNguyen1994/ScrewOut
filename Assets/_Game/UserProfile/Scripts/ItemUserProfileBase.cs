using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UserProfile
{
    public abstract class ItemUserProfileBase : MonoBehaviour
    {
        [SerializeField] private int id;
        [SerializeField] protected Image imgItem;
        [SerializeField] protected GameObject gobjLock;
        [SerializeField] protected GameObject gobjCurrent;
        [SerializeField] protected ItemState itemState;

        protected UnityAction<ItemUserProfileBase> actionOnClick;

        public ItemState ItemState { get => itemState; }
        public int Id { get => id; }

        public void Init(int id, Sprite spriteItem, ItemState itemState)
        {
            this.id = id;
            this.imgItem.sprite = spriteItem;
            this.itemState = itemState;
            UpdateState(itemState);
        }
        public abstract void UpdateState(ItemState itemState);
        public void OnClickItem()
        {
            this.actionOnClick?.Invoke(this);
        }
        public void SetActionClick(UnityAction<ItemUserProfileBase> actionOnClick)
        {
            this.actionOnClick = actionOnClick;
        }


    }
}
