using UnityEngine;

[CreateAssetMenu(fileName = "BuildConfig", menuName = "Scriptable Objects/BuildConfig")]
public class BuildConfig : ScriptableObject
{
    [Header("Build Settings")]
    [Header("Android")]
    public string buildVersionAOS;
    public string packageNameAOS;
    public int bundleVersionCodeAOS;
    
    [Header("IOS")]
    public string buildVersionIOS;
    public string bundleIdentifierIOS;
    public int buildIOS;
    public string signingTeamIDIOS;

    [Header("Player Settings")]
    [Header("Android")]
    public string companyNameAOS;
    public string productNameAOS;
    
    [Header("IOS")]
    public string companyNameIOS;
    public string productNameIOS;
    
}
