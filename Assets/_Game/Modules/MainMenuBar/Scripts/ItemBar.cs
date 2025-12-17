using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MainMenuBar
{

    public class ItemBar : MonoBehaviour
    {
        [SerializeField] private int index;

        [Header("Lock")]
        [SerializeField] private bool isLock;
        [SerializeField] private bool isNew;
        [SerializeField] private int levelUnlock;
        [SerializeField] private GameObject gobjLock;
        [SerializeField] private Text txtLevelToUnLock;


        [Header("Unlock")]
        [SerializeField] private GameObject gobjUnLock;
        [SerializeField] private GameObject gobjRedDot;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Image imgItem;
        [SerializeField] private Image imgIconLock;
        [SerializeField] private RectTransform rtfmIconHolder;
        [SerializeField] private Sprite iconSelected, iconNoneSelect, sprLock;
        [SerializeField] private Text txtName;
        UnityAction<ItemBar> actionOnClick;
        UnityAction<ItemBar> actionFinishAnimation;

        [Header("Animation")]
        [SerializeField] private float timeAnimation;
        [SerializeField] private Vector2 normalSize;
        [SerializeField] private Vector2 selectedSize;
        [SerializeField] private Vector2 normalIconPos;
        [SerializeField] private Vector2 selectedIconPos;
        [SerializeField] private Vector2 normalIconSize;
        [SerializeField] private Vector2 selectedIconSize;
        [SerializeField] private string nameItem;
        [SerializeField] private RectTransform rtfmHolder;

        [SerializeField] private Image frameLeft;
        [SerializeField] private Image frameRight;
        [SerializeField] private Button btnTab;

        public Button GetButton() { return btnTab; }

        public bool IsLock { get => isLock; }
        public int LevelUnlock { get => levelUnlock; }
        public bool IsNew { get => isNew; }
        public int Index { get => index; }

        public static System.Action<int> ChangeTab;

        private void Awake()
        {
            ChangeTab += OnChangeTabHandler;

            frameLeft.gameObject.SetActive(false);
            frameRight.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            ChangeTab -= OnChangeTabHandler;
        }

        private void OnChangeTabHandler(int tab)
        {
            if (tab - index >= 2)
            {
                frameLeft.gameObject.SetActive(false);
                frameRight.gameObject.SetActive(true);
            }
            else if (tab - index <= -2)
            {
                frameLeft.gameObject.SetActive(true);
                frameRight.gameObject.SetActive(false);
            }
            else
            {
                frameLeft.gameObject.SetActive(false);
                frameRight.gameObject.SetActive(false);
            }
        }

        public void SetDefaultSize(float timeAnimation, Vector2 normalSize, Vector2 selectedSize, Vector2 normalIconPos, Vector2 selectedIconPos, Vector2 normalIconSize, Vector2 selectedIconSize)
        {
            this.timeAnimation = timeAnimation;
            this.normalSize = normalSize;
            this.selectedSize = selectedSize;
            this.normalIconPos = normalIconPos;
            this.selectedIconPos = selectedIconPos;
            this.normalIconSize = normalIconSize;
            this.selectedIconSize = selectedIconSize;
        }
        public void Init(int index, bool isNew, bool isLock, string name, int levelUnlock, Sprite spriteSelect, Sprite spriteUnSelect, Sprite sprLock, UnityAction<ItemBar> actionOnClick, UnityAction<ItemBar> actionFinishAnimation)
        {
            this.index = index;
            this.isLock = isLock;
            this.sprLock = sprLock;
            this.levelUnlock = levelUnlock;
            this.iconSelected = spriteSelect;
            this.iconNoneSelect = spriteUnSelect;
            this.actionOnClick = actionOnClick;
            this.actionFinishAnimation = actionFinishAnimation;
            this.nameItem = name;
            this.isNew = isNew;
            UnSelect(0);
            //Select(0);
            UpdateUI();
        }
        public void UpdateUI()
        {
            txtLevelToUnLock.text = $"UNLOCK\nLEVEL {levelUnlock}";
            //xtLevelToUnLock.text = $"LOCK";
            txtName.text = this.nameItem;
            // gobjRedDot.SetActive(isNew);
            SetLockState(isLock);
        }
        public void SetNoti(bool active)
        {
            if (active && IsLock)
                return;
            gobjRedDot.SetActive(active);
        }
        public void SetLockState(bool isLock)
        {
            if (isLock)
            {
                gobjLock.SetActive(true);
                gobjUnLock.SetActive(true);
                txtName.gameObject.SetActive(false);
                txtLevelToUnLock.gameObject.SetActive(true);
                imgIcon.gameObject.SetActive(false);
                imgIconLock.sprite = sprLock;
                imgIconLock.gameObject.SetActive(true);

            }
            else
            {
                gobjLock.SetActive(false);
                imgIconLock.gameObject.SetActive(false);

                gobjUnLock.SetActive(true);
            }
        }
        private void OnReset()
        {
            txtName.DOKill();
            rtfmHolder.DOKill();
            rtfmIconHolder.DOKill();
            imgItem.DOKill();

        }
        public void Select(float timeAnimation = 0)
        {
            OnReset();
            imgIcon.sprite = iconSelected;

            // Animation
            rtfmIconHolder.DOAnchorPos(selectedIconPos, timeAnimation);
            rtfmIconHolder.DOSizeDelta(selectedIconSize, timeAnimation);

            txtName.DOFade(1f, timeAnimation);
            rtfmHolder.DOSizeDelta(selectedSize, timeAnimation);

            imgItem.DOFade(1f, timeAnimation);
            gobjRedDot.SetActive(false);

            frameLeft.DOFade(0f, timeAnimation);
            frameRight.DOFade(0f, timeAnimation);
        }

        public void UnSelect(float timeAnimation = 0)
        {
            OnReset();
            imgIcon.sprite = iconNoneSelect;

            //Animation
            rtfmIconHolder.DOAnchorPos(normalIconPos, timeAnimation);
            rtfmIconHolder.DOSizeDelta(normalIconSize, timeAnimation);
            txtName.DOFade(0f, timeAnimation);
            rtfmHolder.DOSizeDelta(normalSize, timeAnimation);
            imgItem.DOFade(0f, timeAnimation);

            frameLeft.DOFade(1f, timeAnimation);
            frameRight.DOFade(1f, timeAnimation);
        }
        public void OnClickItem()
        {
            if (MainMenuRecieveRewardsHelper.Instance != null && MainMenuRecieveRewardsHelper.Instance.IsShowReward)
            {
                return;
            }

            Debug.Log($"Click item bar {index}");
            AudioController.Instance.PlaySound(SoundName.Click);
            actionOnClick?.Invoke(this);
        }
    }
}
