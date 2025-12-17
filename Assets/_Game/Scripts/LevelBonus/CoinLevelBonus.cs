using DG.Tweening;
using UnityEngine;

public class CoinLevelBonus : MonoBehaviour
{
    public Animator Animator;

    public Transform ParentRoot;


    public void UpdateParentRoot()
    {
        transform.DOScale(Vector3.zero, 0.25f);
    }

    public void UpdateParentRootTween()
    {
       // ParentRoot.DOLocalRotate(new Vector3(-45, 0, 0), 0.25f);
    }
}