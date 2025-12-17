using Cysharp.Threading.Tasks;
using DG.Tweening;
using EasyButtons;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CoinCollector : Singleton<CoinCollector>
{
    [Header("References")]
    [SerializeField] private Canvas canvas;
    [SerializeField] private Transform target;        // RectTransform UI
    [SerializeField] private Transform iconScale;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private TMP_Text txtCoin;
    [SerializeField] private int coinAmount;
    [SerializeField] private int coin;

    [Header("Path Settings")]
    public PathType pathType = PathType.CatmullRom;

    private Camera cam;
    public bool Completed { get => coinAmount == 0; }
    public int Coin { get => coin; set => coin = value; }

    protected override void CustomAwake()
    {
        cam = Camera.main;
        coin = 0;
    }

    // ======================================================
    // Convert WORLD ---> UI (local position trong Canvas)
    // ======================================================
    private Vector2 WorldToCanvasLocal(Vector3 worldPos)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            screenPos,
            null,
            out Vector2 localPos
        );

        return localPos;
    }
    [SerializeField] private GameObject testObject;
    [SerializeField] private int delayMili = 130;
    [SerializeField] private int waitMili = 200;
    [SerializeField] private int spawnCountMin = 2;
    [SerializeField] private int spawnCountMax = 5;
    [SerializeField] private int offsetPos = 30;
    [SerializeField] private float scaleTime = 0.2f;
    [SerializeField] private float moveTime = 0.8f;
    [SerializeField] private float rotate1Loop = 0.2f;
    [Button]
    public void Test()
    {
        Collect(testObject.transform, 10).Forget();
    }
    // ======================================================
    // Spawn nhiều coin
    // ======================================================
    public async UniTask Collect(Transform startWorldObj, int count)
    {
        LevelBonusController.Instance.StartCountDownTime();
        var spawnCount = Random.Range(spawnCountMin, spawnCountMax + 1);
        var lstTasks = new List<UniTask>();
        var pos3D = startWorldObj.position;
        AudioController.Instance.PlaySound(SoundName.COIN_LEVEL_BONUS);
        coinAmount += spawnCount;
        for (int i = 0; i < spawnCount; i++)
        {
            lstTasks.Add(SpawnOne(pos3D, waitMili, UpdateCoin));
            await UniTask.Delay(delayMili);
        }
        await UniTask.WhenAll(lstTasks);
        // Effect icon khi hoàn thành
        //await iconScale.DOScale(1.5f, 0.1f).SetEase(Ease.OutBack);
        coinAmount -= spawnCount;

    }
    private void UpdateCoin()
    {
        coin++;
        txtCoin.text = $"x{coin}";
        particleSystem.Play();

        //iconScale.DOKill();
        //txtCoin.transform.DOKill();
        iconScale.DOScale(1f, 0.1f).SetEase(Ease.OutBack).From(0.9f);
        txtCoin.transform.DOScale(1f, 0.1f).SetEase(Ease.OutBack).From(0.9f);
    }

    // ======================================================
    // Spawn một coin + Tween path
    // ======================================================
    private async UniTask SpawnOne(Vector3 pos3D, int wait, UnityAction action)
    {
        // Lấy coin từ pool
        CoinLevelBonus coin = CoinBonusPool.Instance.Get();

        // Convert start từ 3D sang UI
        Vector3 startUIRoot = WorldToCanvasLocal(pos3D);
        var offset = new Vector2(offsetPos, offsetPos);
        var startUI = startUIRoot + new Vector3(Random.Range(-offset.x, offset.x), Random.Range(-offset.y, offset.y), 0);
        // Convert target UI
        Vector3 endUI = target.position;

        // Setup coin trong canvas
        Transform coinT = coin.transform;
        coinT.SetParent(canvas.transform);
        coinT.SetAsLastSibling();
        coinT.localPosition = startUIRoot;
        //coinT.localRotation = new Quaternion(0,0,0,0);

        // Bật animation
        coin.Animator.enabled = true;

        // Nảy nhẹ lúc spawn
        coinT.DOLocalMove(startUI, scaleTime).SetEase(Ease.OutBack);
        //    coinT.DOLocalRotate(new Vector3(0,360,0), rotate1Loop, RotateMode.LocalAxisAdd).SetLoops(-1).SetEase(Ease.Linear);
        await coinT.DOScale(1f, scaleTime).From(Vector3.zero).SetEase(Ease.OutBack);
        //await coinT.DOScale(1f, 0.05f);

        //  AudioController.Instance.PlaySound(SoundName.ScrewDown);
        //   AudioController.Instance.PlaySound(SoundName.Fly);
        coin.UpdateParentRootTween();
        // coin.Animator.enabled = false;
        await UniTask.Delay(wait);
        await coinT.DOMove(endUI, moveTime)
                .SetEase(Ease.OutCubic)
                .OnComplete(() => coin.UpdateParentRoot())
                .AsyncWaitForCompletion();
        //AudioController.Instance.PlaySound(SoundName.CollectExp);
        action?.Invoke();
        // Return to pool
        CoinBonusPool.Instance.Release(coin);
    }
}
