using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Storage;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyFly : FlyBase
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform render;
    
    public override async UniTask Execute(Vector3 startPos, Vector3 targetPos, int amount, float duration, UnityAction<int> onUpdateUI)
    {
        // var userInfo = Db.storage.USER_INFO;
        // userInfo.coin += amount;
        // Db.storage.USER_INFO = userInfo;
        
        await DoStep1(startPos);
        await transform.DOMove(targetPos, duration).SetSpeedBased(true);
        onUpdateUI?.Invoke(amount);
        Destroy(gameObject);
    }

    async UniTask DoStep1(Vector3 startPos)
    {
        render.position = startPos;
        Vector3 offsetPos = new Vector3(-Random.Range(40, 70), -Random.Range(50, 70), 0);
        Vector3 moveBottomPos = startPos + offsetPos;
        render.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        render.DOScale(Vector3.one, 0.2f);
        await UniTask.Delay(200);
        await transform.DOMove(moveBottomPos, 0.6f).SetEase(Ease.InSine);
    }

    public override async UniTask ExecuteLocal(Vector3 startPos, Vector3 targetPos, int amount, float duration, UnityAction<int> onUpdateUI)
    {
        render.SetWorldToLocalPosition(startPos);
        Vector3 offsetPos = new Vector3(-Random.Range(40, 70), -Random.Range(50, 70), 0);
        Vector3 moveBottomPos = startPos + offsetPos;
        render.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        await render.DOScale(Vector3.one, 0.1f);
        await transform.DOLocalMove(transform.WorldToLocalPosition(moveBottomPos), 0.2f).SetEase(Ease.InSine);
        await transform.DOLocalMove(transform.WorldToLocalPosition(targetPos), duration).SetSpeedBased(true);
        onUpdateUI?.Invoke(amount);
        Destroy(gameObject);
    }
}
