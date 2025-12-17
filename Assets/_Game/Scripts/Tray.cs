using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Tray : MonoBehaviour
{
    [SerializeField] private bool isFill = false;
    [SerializeField] private bool isComletedAnim = false;
    [SerializeField] private Screw screw ;

    public Box Box;

    public bool IsFill { get { return isFill; } }

    public void InitNewTray()
    {
        if (screw != null)
        {
           
            screw.gameObject.SetActive(false);
            screw = null;

        }
        isFill = false;
        isComletedAnim = false;
    }
    public void Fill(Screw screw)
    {
        this.screw = screw;
        isFill = screw != null;
    }
    public bool IsComletedAnim()
    {
        return isComletedAnim;
    }
    public void SetComletedAnim(bool isComletedAnim)
    {
        this.isComletedAnim = isComletedAnim;
    }

    public string GetScrewId()
    {
        if (IsFill)
        {
            return screw != null ? screw.UniqueId : string.Empty;
        }

        return string.Empty;
    }

    public bool TryFill(Screw screw)
    {
        if (Box != null && Box.Color != screw.ScrewColor)
        {
            return false;
        }

        if (!IsFill)
        {
            Fill(screw);
            return true;
        }

        return false;
    }
    public async UniTask HideTray()
    {
        transform.DOScale(Vector3.zero, 0.3f);
    }
}
