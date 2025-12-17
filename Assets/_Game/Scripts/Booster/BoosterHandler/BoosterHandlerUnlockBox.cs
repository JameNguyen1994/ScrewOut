using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoosterHandlerUnlockBox : BoosterHandlerBase
{
    [SerializeField] private Box box;
    public override void ActiveBooster(UnityAction actionCompleteBooster)
    {
        base.ActiveBooster(actionCompleteBooster);
        box.ChangeState(BoxState.Unlock);
        AudioController.Instance.PlaySound(SoundName.Effectt_Appear); 
        Debug.Log($"Unlock Box {box.name}");
        SetDoneBooster();
    }
    public override void SetDoneBooster()
    {
        base.SetDoneBooster();
    }
}
