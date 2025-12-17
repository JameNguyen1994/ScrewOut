using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIBase : MonoBehaviour
{
    public abstract void Show(Dictionary<string, object> data, UnityAction<Dictionary<string, object>> callback);
    public abstract void Hide();
}
