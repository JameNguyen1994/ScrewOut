using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinFlyAnim : MonoBehaviour
{
    [SerializeField] private Transform tfmFrom;
    [SerializeField] private Transform tfmDestination;
    [SerializeField] private float radius;
    [SerializeField] private Sprite sprCoin;

    [SerializeField] private List<Image> lstImgCoins;
    private List<Vector3> lstInitialPos = new List<Vector3>();

    private AudioController audioController;

    private void Awake()
    {
        ToggleCoin(false);

        for (int i = 0; i < lstImgCoins.Count; i++)
        {
            lstInitialPos.Add(lstImgCoins[i].transform.localPosition);
        }
    }
    private void Start()
    {
        audioController = AudioController.Instance;
    }
    public async UniTaskVoid PlayCoinFlyWithPos(float dur,Vector3 startPos,SoundName soundName = SoundName.Coin, bool isHaveSound = false, int amount = 20)
    {
        this.transform.position = startPos;
        
        for (int i = 0; i < lstImgCoins.Count; i++)
        {
            lstImgCoins[i].transform.localScale = Vector3.zero;
            lstImgCoins[i].transform.localPosition = Vector3.zero;
            // lstImgCoins[i].transform.localPosition = lstInitialPos[i];
        }
        
        // foreach (var imgCoin in lstImgCoins)
        // {
        //     imgCoin.transform.localScale = Vector3.zero;
        //     // await UniTask.Delay(100);
        // }
        this.gameObject.SetActive(true);
        await PlayFlyCoinAnim(dur, soundName, isHaveSound, amount);
        this.gameObject.SetActive(false);
    }

    public async UniTask PlayFlyCoinAnim(float dur, SoundName soundName = SoundName.Coin, bool isHaveSound = false, int amount = 20)
    {
        for (var i=0; i<lstImgCoins.Count; i++)
        {
            var imgCoin = lstImgCoins[i];
            imgCoin.gameObject.SetActive(true);
            imgCoin.transform.DOScale(Vector3.one, 0.05f);
            imgCoin.transform.DOLocalMove(lstInitialPos[i], 0.2f).SetEase(Ease.OutBack);

            //await UniTask.Delay(100);
        }
        
        await UniTask.Delay(300);
        
        List<UniTask> tasks = new List<UniTask>();

        int count = 0;
        foreach (var coin in lstImgCoins)
        {
            if (count > amount - 1) break;

            count++;
            Vector3 _rnd = Random.insideUnitCircle * radius;
            MakeCoinFly(coin.transform, dur, _rnd, soundName, isHaveSound).Forget();
            await UniTask.Delay(Random.Range(80, 150));
            //tasks.Add(MakeCoinFly(coin.transform, dur, _rnd, soundName, isHaveSound));
        }

        await UniTask.WaitUntil(() => count >= amount);
    }
    public async UniTask PlayFlyCoinAnimThinly(float dur, SoundName soundName = SoundName.Coin, bool isHaveSound = false, int amount = 20)
    {
        List<UniTask> tasks = new List<UniTask>();

        int count = 0;
        foreach (var coin in lstImgCoins)
        {
            if (count > amount - 1) break;

            count++;
            Vector3 _rnd = Random.insideUnitCircle * radius;
            tasks.Add(MakeCoinFlyThinly(coin.transform, dur, _rnd, soundName, isHaveSound));
        }

        await UniTask.WhenAll(tasks);
    }

    public async UniTask MakeCoinFly(Transform rtfmCoin, float dur, Vector3 startPos, SoundName soundName = SoundName.Coin, bool isHaveSound = false)
    {
        //await UniTask.WaitForSeconds(Random.Range(0f, 0.5f));

        //rtfmCoin.transform.localScale = Vector3.zero;
        //rtfmCoin.position = tfmFrom.position + startPos;
        //rtfmCoin.gameObject.SetActive(true);

        Vector3[] path = new Vector3[3];
        path[0] = rtfmCoin.position;
        path[2] = tfmDestination.position;
        var randVect = Vector3.Lerp(path[0], path[2], 0.5f);
        path[1] = new Vector3(randVect.x * Random.Range(0.7f, 1.4f), randVect.y, randVect.z);

        rtfmCoin.transform.DOScale(Vector3.one, dur * 0.2f).ToUniTask().Forget();
        await rtfmCoin.DOPath(path, dur, PathType.CatmullRom).SetEase(Ease.InQuart);

        if (isHaveSound) audioController.PlaySound(soundName);

        rtfmCoin.gameObject.SetActive(false);
    }
    public async UniTask MakeCoinFlyThinly(Transform rtfmCoin, float dur, Vector3 startPos, SoundName soundName = SoundName.Coin, bool isHaveSound = false)
    {
        await UniTask.WaitForSeconds(Random.Range(0f, 0.5f));

        rtfmCoin.transform.localScale = Vector3.zero;
        rtfmCoin.position = tfmFrom.position + startPos;
        rtfmCoin.gameObject.SetActive(true);

        Vector3[] path = new Vector3[3];
        path[0] = rtfmCoin.position;
        path[2] = tfmDestination.position;
        var randVect = Vector3.Lerp(path[0], path[2], 0.5f);
        path[1] = new Vector3(randVect.x * Random.Range(0.9f, 1.1f), randVect.y, randVect.z);

        rtfmCoin.transform.DOScale(Vector3.one, dur * 0.2f).ToUniTask().Forget();
        await rtfmCoin.DOPath(path, dur, PathType.CatmullRom).SetEase(Ease.InQuart);

        if (isHaveSound) audioController.PlaySound(soundName);

        rtfmCoin.gameObject.SetActive(false);
    }
    private void ToggleCoin(bool toggle)
    {
        foreach (var coin in lstImgCoins)
        {
            coin.gameObject.SetActive(toggle);
        }
    }
}
