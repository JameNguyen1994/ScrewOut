using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class CustomBuildScript : MonoBehaviour
{
    [InitializeOnLoadMethod]
    static void RegisterBuildPlayerHandler()
    {
        BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildPlayer);
    }

    static void OnBuildPlayer(BuildPlayerOptions buildPlayerOptions)
    {
        if (PlayerSettings.insecureHttpOption != InsecureHttpOption.AlwaysAllowed)
        {
            PlayerSettings.insecureHttpOption = InsecureHttpOption.AlwaysAllowed;
        }
        
#if STAGING_SERVER
        if (EditorUserBuildSettings.buildAppBundle)
        {
            if (EditorUtility.DisplayDialog(
                    "[ACT] Build Reminder",
                    "The user is building the staging server configuration for the release version.",
                    "Cancel"))
            {
                
            }
        }
        else
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
#else
        if (!EditorUserBuildSettings.buildAppBundle)
        {
            if (EditorUtility.DisplayDialog(
                    "[ACT] Build Reminder",
                    "The user is building the release server configuration for the test version.",
                    "Continue",
                    "Cancel"))
            {
                BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
            }
            else
            {
                
            }
        }
        else
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(buildPlayerOptions);
        }
#endif
    }
}