using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsControlSelectScrew
{
    public virtual void OnClickUp(RaycastHit hit) { }
    public virtual void OnClickDown(RaycastHit hit) { }
}
