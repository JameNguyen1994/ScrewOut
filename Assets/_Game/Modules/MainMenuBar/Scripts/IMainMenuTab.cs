using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMainMenuTab
{
    public abstract void Init(int index);
    public abstract void GoToThisTab();
    public abstract void ExitThisTab();
}
