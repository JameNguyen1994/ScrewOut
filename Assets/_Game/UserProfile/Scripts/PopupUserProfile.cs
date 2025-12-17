using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using Storage.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;
using UserProfile;

public class PopupUserProfile : MonoBehaviour
{
    [SerializeField] private GameObject gobjContent;
    [SerializeField] private Image imgFade;
    [SerializeField] private Text txtLevel;
    [SerializeField] private Text txtExp;
    [SerializeField] private Image imgFillBar;
    [SerializeField] private UserExp levelTarget = new UserExp();
    [SerializeField] private Button btnClose;
    [SerializeField] private List<Transform> lstItem;
    [SerializeField] private List<Transform> lstItemFrame;
    [SerializeField] private Button btnSave;

    [SerializeField] private TabAvatar tabAvatar;
    [SerializeField] private TabFrame tabFrame;

    private int userFrame;
    private int userAvatar;
    private string userName;

    public async UniTask Show()
    {
        TrackingController.Instance.TrackingProfile();
        imgFade.gameObject.SetActive(true);
        gobjContent.SetActive(true);
        HideItems();
        txtLevel.text = "";
        txtExp.text = "";
        imgFillBar.fillAmount = 0f;
        btnClose.transform.localScale = Vector3.zero;
        imgFade.DOFade(0.99f, 0.3f).SetEase(Ease.OutBack);

        gobjContent.gameObject.SetActive(true);
        gobjContent.transform.localScale = Vector3.zero;
        ShowListItemAsync();

        await gobjContent.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack).ToUniTask();

        btnClose.transform.localScale = Vector3.zero;
        await btnClose.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack);//.SetDelay(time);
        btnClose.interactable = true;


        var userExp = Db.storage.USER_EXP;
        levelTarget.level = userExp.level + 1;
        levelTarget.exp = userExp.level * 1000;
        ActiveSaveButton(false);
        UpdateUIExp(userExp);

        userFrame = DBUserProfileController.Instance.USER_INFO_DATA.frameID;
        userAvatar = DBUserProfileController.Instance.USER_INFO_DATA.avatarID;
        userName = DBUserProfileController.Instance.USER_INFO_DATA.userName;
    }

    public void ActiveSaveButton(bool interactable)
    {
        btnSave.interactable = interactable;
    }

    public void OnClickSave()
    {
        Hide();
    }

    public void OnClickExit()
    {
        Hide();

        DBUserProfileController.Instance.USER_INFO_DATA.SetUserName(userName);

        tabAvatar.UpdateAvatar(userAvatar);
        tabFrame.UpdateFrameData(userFrame);

        UserProfileManager.Instance.UpdateUserUI();
    }

    private void HideItems()
    {
        foreach (var item in lstItem)
        {
            item.transform.localScale = Vector3.zero;
        }
        foreach (var item in lstItemFrame)
        {
            item.transform.localScale = Vector3.zero;
        }
    }
    public async UniTask ShowListItemAsync()
    {
        await UniTask.Delay(150);
        for (int i = 0; i < lstItem.Count; i++)
        {
            lstItem[i].DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            if (i < lstItemFrame.Count)
            {
                lstItemFrame[i].DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            }
            if ((i + 1) % 3 == 0)
            {
                await UniTask.Delay(150);
            }
        }
    }

    public void Hide()
    {
        imgFade.gameObject.SetActive(false);
        gobjContent.SetActive(false);
    }

    private void UpdateUIExp(UserExp userExpUI)
    {
        txtExp.text = $"{userExpUI.exp}/{levelTarget.exp}";
        txtLevel.text = $"{userExpUI.level}";
        DOVirtual.Int(0, userExpUI.level, 0.5f, (value) =>
        {
            txtLevel.text = $"{value}";
        });
        DOVirtual.Float(0, userExpUI.exp, 0.5f, (value) =>
        {
            txtExp.text = $"{(int)value}/{levelTarget.exp}";
        });
        var fill = (float)userExpUI.exp / (float)levelTarget.exp;
        imgFillBar.DOFillAmount(fill, 0.5f).From(0);

    }
}
