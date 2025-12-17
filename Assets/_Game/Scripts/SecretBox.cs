using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using NUnit.Framework;
using ScrewCraze3D;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SecretBox : ObjectIdentifier
{
    [SerializeField] private List<Screw> lstScrew;
    [SerializeField] private List<Screw> lstScrewSave;
    [SerializeField] private Transform tfmSecretBox;
    [SerializeField] private Tray tray;
    [SerializeField] private Transform tfmSecretBoxShow;
    [SerializeField] private Transform tfmLeftScreenPos;
    [SerializeField] private Transform tfmShowPos;
    private Vector3 offset = new Vector3(7, 0, 0);
    [SerializeField] private Camera mainCam;   // Camera đang render (gán trong Inspector)
    [SerializeField] private int screwAnimAdd;
    [SerializeField] private int screwAnimSub;
    [SerializeField] private bool isShowing;
    [SerializeField] private Animator animBox;


    private const string ANIM_OPEN = "Open";
    private const string ANIM_CLOSE = "Close";

    public List<Screw> LstScrew { get => lstScrew; }

    public override string UniqueId => Define.SECRET_BOX_ID;

    private void Start()
    {
        SetUpLeftScreenPos();
    }
    private void SetUpLeftScreenPos()
    {
        if (mainCam == null)
            mainCam = Camera.main;
        isShowing = false;
        // Lấy toạ độ trên màn hình: mép trái, cùng y hiện tại
        Vector3 screenPos = mainCam.WorldToScreenPoint(tfmLeftScreenPos.position);

        // Ép x về mép trái màn hình
        screenPos.x = 0;

        // Chuyển lại sang world
        Vector3 worldPos = mainCam.ScreenToWorldPoint(screenPos);

        // Giữ nguyên y và z gốc
        worldPos.y = tfmLeftScreenPos.position.y;
        worldPos.z = tfmLeftScreenPos.position.z;
        var result = worldPos;
        // Cộng offset nếu cần
        tfmLeftScreenPos.position = result - offset;
        tfmShowPos.position = result + offset;
        tfmSecretBox.position = tfmShowPos.position;
        tfmSecretBoxShow.localPosition = Vector3.zero;
        tfmSecretBoxShow.position = tfmLeftScreenPos.position;
    }
    [Button("Show")]
    public async UniTask TestShow()
    {
        await ShowAsync();
        animBox.Play(ANIM_OPEN);
    }
    public async UniTask ShowAsync()
    {
        var timeOpen = 0.3f;
        tfmSecretBoxShow.DOKill();
        isShowing = true;

        var timeRotateMove = timeOpen / 5 * 4;
        var timeRotateStop = timeOpen / 5 * 1;
        var angleMove = new Vector3(tfmSecretBoxShow.eulerAngles.x, tfmSecretBoxShow.eulerAngles.y, 15);
        var angleStop = new Vector3(tfmSecretBoxShow.eulerAngles.x, tfmSecretBoxShow.eulerAngles.y, 0);
        tfmSecretBoxShow.DORotate(angleMove, timeRotateMove).OnComplete(
            () =>
            {
                tfmSecretBoxShow.DORotate(angleStop, timeRotateStop).SetEase(Ease.OutBack);
            });
        await tfmSecretBoxShow.DOMove(tfmShowPos.position, timeOpen).SetEase(Ease.OutBack);


    }
    [Button("Hide")]
    public async UniTask TestHide()
    {
        animBox.Play(ANIM_CLOSE);
        await HideAsync();
    }
    public async UniTask HideAsync()
    {
        isShowing = false;
        Debug.Log("CheckNewBox Hide");
        await WaitForAnimation(animBox, ANIM_CLOSE);

        await tfmSecretBoxShow.DOMove(tfmLeftScreenPos.position, 0.5f).SetEase(Ease.InQuart);
    }
    private async UniTask WaitForAnimation(Animator animator, string stateName)
    {
        // Lấy layer mặc định (0)
        int layer = 0;
        // Đợi animator thực sự vào state cần
        if (!animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName))
        {
            return;
        }
        //await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layer).IsName(stateName));
        // Đợi tới khi anim chạy xong
        await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(layer).normalizedTime >= 1f);
    }
    public async UniTask CheckToShowSecretBox()
    {
        if (isShowing)
            return;
        await ShowAsync();

        animBox.Play(ANIM_OPEN);
        await WaitForAnimation(animBox, ANIM_OPEN);
        await UniTask.Delay(100);
    }
    public async UniTask AddScrewToSecretBox(Screw screw)
    {
        screwAnimAdd++;
        lstScrewSave.Add(screw);
        await CheckToShowSecretBox();
        lstScrew.Add(screw);
        await screw.MoveToSecretBox(tray);
        tfmSecretBoxShow.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack);
        RemoveScrewAnimationCount();
    }

    public async UniTask RemoveScrew(Screw screw)
    {
        screwAnimSub++;
        lstScrewSave.Remove(screw);

        await CheckToShowSecretBox();
        lstScrew.Remove(screw);
        tfmSecretBoxShow.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutBack);

    }
    public void OnDoneAnimationMove()
    {
        screwAnimSub--;
        if (screwAnimSub < 0) screwAnimSub = 0;
        CheckHide();
    }
    private void RemoveScrewAnimationCount()
    {
        screwAnimAdd--;
        if (screwAnimAdd < 0) screwAnimAdd = 0;
        CheckHide();
    }
    private void CheckHide()
    {
        if (screwAnimAdd == 0 && screwAnimSub == 0)
        {
            animBox.Play(ANIM_CLOSE);
            HideAsync().Forget();
        }
    }

    public override void Serialize()
    {
        base.Serialize();

        BoxData boxData = new BoxData(UniqueId);

        for (int i = 0; i < lstScrewSave.Count; i++)
        {
            boxData.Screws.Add(lstScrewSave[i] != null ? lstScrewSave[i].UniqueId : string.Empty);
        }

        Serialize(boxData);
    }

    public override void InitializeFromSave()
    {
        base.InitializeFromSave();

        BoxData boxData = SerializationService.DeserializeObject<BoxData>(UniqueId);

        for (int i = 0; i < boxData.Screws.Count; i++)
        {
            Screw screw = ScrewManager.GetScrewById(boxData.Screws[i]);

            if (screw != null)
            {
                lstScrew.Add(screw);
                lstScrewSave.Add(screw);

                screw.SetState(ScrewState.OnReviveBox);
                screw.SetTray(tray);
                screw.transform.parent = tray.transform;
                screw.transform.localPosition = Vector3.zero;
                screw.transform.localScale = Vector3.zero;
            }
        }
    }
}
