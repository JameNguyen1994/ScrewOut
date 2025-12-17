using UnityEngine;

[CreateAssetMenu(fileName = "SettingAdapterSO", menuName = "Scriptable Objects/SettingAdapterSO")]
public class SettingAdapterSO : ScriptableObject
{
    public MonoBehaviour defaultTimeAdapterBehaviour;
    public MonoBehaviour defaultPlayerDataAdapterBehaviour;
    public MonoBehaviour alternativeTimeAdapterBehaviour;
    public MonoBehaviour alternativePlayerDataAdapterBehaviour;
}
