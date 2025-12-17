using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUBoosterHandler
{

    public abstract void ActiveBooster(UnityAction actionCompleteBooster);
    public abstract void SetDoneBooster();
    public abstract UniTask Action();

}
public class BoosterHandlerBase : MonoBehaviour, IUBoosterHandler
{
    protected UnityAction actionCompleteBooster;
    public virtual void ActiveBooster(UnityAction actionCompleteBooster)
    {
        this.actionCompleteBooster = actionCompleteBooster;

    }
    public virtual void SetDoneBooster()
    {
        actionCompleteBooster?.Invoke();
    }
    public virtual async UniTask Action()
    {

    }
}
