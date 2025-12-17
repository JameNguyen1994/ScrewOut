using DG.Tweening;
using UnityEngine;

public class Wrench : MonoBehaviour
{
    public Screw Screw;
    public Animator Animator;

    public Transform ParentRoot;

    public void OnSelect()
    {
        if (Screw != null)
        {
            Screw.RemoveWrench(this);
        }
    }

    public void UpdateParentRoot()
    {
        ParentRoot.rotation = Quaternion.Euler(new Vector3(-45, 0, 0));
    }

    public void UpdateParentRootTween()
    {
        ParentRoot.DOLocalRotate(new Vector3(-45, 0, 0), 0.25f);
    }
}