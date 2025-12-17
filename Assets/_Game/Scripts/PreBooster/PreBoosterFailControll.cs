using UnityEngine;

public class PreBoosterFailControll : Singleton<PreBoosterFailControll>
{
    [SerializeField] private PreBooster preBoosterRocket, preBoosterGlass;

    public void ReInitUI()
    {
        preBoosterRocket.Init(true);
        preBoosterGlass.Init(true);
    }
}
