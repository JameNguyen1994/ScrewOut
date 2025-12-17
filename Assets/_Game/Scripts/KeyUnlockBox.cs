using Cysharp.Threading.Tasks;
using UnityEngine;

public class KeyUnlockBox : MonoBehaviour
{
    const string ANIM_OPEN = "Open";
    [SerializeField] private Transform tfmKey;
    [SerializeField] private Animator animKey;

    public async UniTask PlayAnimationKey()
    {
        tfmKey.gameObject.SetActive(true);
        animKey.Play(ANIM_OPEN);
        await UniTask.WaitUntil(() => animKey.GetCurrentAnimatorStateInfo(0).IsName(ANIM_OPEN) && animKey.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        tfmKey.gameObject.SetActive(false);
    }
}
