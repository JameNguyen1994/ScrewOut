using UnityEngine;

public class MaterialEffectController : Singleton<MaterialEffectController>
{
    [SerializeField] private Material matBlink;
    [SerializeField] private Material matWhite;

    public Material MatBlink => matBlink;
    public Material MatWhite => matWhite;
}
