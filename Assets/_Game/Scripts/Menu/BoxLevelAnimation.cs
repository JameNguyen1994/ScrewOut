using Cysharp.Threading.Tasks;
using Storage;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxLevelAnimation : MonoBehaviour
{
    private const string ANIM_ILDE_1 = "Idle1";
    private const string ANIM_ILDE_2 = "Idle2";
    private const string ANIM_OPEN = "Appear";
    private const string ANIM_DONE = "Drop";

    [SerializeField] private Animator animBox;
    [SerializeField] private TextMeshPro txtLevel;
    [SerializeField] private bool isNextLevel = false;


    public async UniTask Init()
    {

        if (IngameData.IS_WIN_LEVEL)
        {
            Debug.Log("InitUI BoxLevelAnimation: IS_WIN_LEVEL = true");
            IngameData.IS_WIN_LEVEL = false;
            //  simpleAnimationBox.gameObject.SetActive(true);
            AudioController.Instance.PlaySound(SoundName.Anim_Box_Open);
            Invoke(nameof(PlaySoundOpenBox), 0.6f);
            Invoke(nameof(PlaySoundMove), 1.8f);
            await InitLevelDone();
            isNextLevel = true;
        }
        else
        {
            Debug.Log("InitUI BoxLevelAnimation: IS_WIN_LEVEL = false");
            isNextLevel = false;
            await InitLevelIdle();
        }


        StartCoroutine(StartIdle2());

    }
    private void PlaySoundOpenBox()
    {
        AudioController.Instance.PlaySound(SoundName.Anim_Screw_Down);

    }
    private void PlaySoundMove()
    {
        AudioController.Instance.PlaySound(SoundName.Anim_Box_Move);

    }
    private async UniTask InitLevelIdle()
    {
        animBox.gameObject.SetActive(true);

        animBox.Play(ANIM_ILDE_1);
        int level = Db.storage.USER_INFO.level;
        txtLevel.text = $"{level}";

    }
    private async UniTask InitLevelDone()
    {
        int level = Db.storage.USER_INFO.level;
        txtLevel.text = $"{level - 1}";
        await StartAnimationDone();
        txtLevel.text = $"{level}";
        // await StartAnimationNewBox();
        // animBox.Play(ANIM_ILDE_1);
    }
    public async UniTask InitLevelNextLevel()
    {
        if (!isNextLevel)
        {
           // Debug.LogError("BoxLevelAnimation: InitLevelNextLevel called but isNextLevel is false.");
            return;
        }
        await StartAnimationNewBox();
    }
    private async UniTask StartAnimationDone()
    {
        animBox.gameObject.SetActive(true);
        animBox.Play(ANIM_DONE);

        // Lấy thời gian của animation clip "Run"
        AnimatorClipInfo[] clipInfo = animBox.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            float clipLength = clipInfo[0].clip.length;
            await UniTask.Delay(TimeSpan.FromSeconds(clipLength));
        }
        //animBox.gameObject.SetActive(false);

        // Hoặc nếu muốn đảm bảo chờ đúng thời điểm animation kết thúc
        await UniTask.WaitUntil(() => animBox.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        animBox.gameObject.SetActive(false);

    }
    private async UniTask StartAnimationNewBox()
    {
        animBox.gameObject.SetActive(true);
        animBox.Play(ANIM_OPEN);

        // Lấy thời gian của animation clip "Run"
        AnimatorClipInfo[] clipInfo = animBox.GetCurrentAnimatorClipInfo(0);
        if (clipInfo.Length > 0)
        {
            float clipLength = clipInfo[0].clip.length;
            await UniTask.Delay(TimeSpan.FromSeconds(clipLength));
        }

        // Hoặc nếu muốn đảm bảo chờ đúng thời điểm animation kết thúc
        await UniTask.WaitUntil(() => animBox.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

    }


    private IEnumerator StartIdle2()
    {
        int min = 15;
        int max = 30;
        var time = Random.Range(min, max);
        while (true)
        {
            yield return new WaitForSeconds(time);
            time = Random.Range(min, max);
            animBox.Play(ANIM_ILDE_2);
        }
    }
}
