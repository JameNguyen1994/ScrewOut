using PS.Utils;
using UnityEngine;

namespace MainMenuBar
{
    public class ToastMessage : Singleton<ToastMessage>
    {
        public void ShowToast(string message)
        {
#if PLATFORM_ANDROID

            // Run on the Android UI thread
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Prepare toast parameters
            AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
            currentActivity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaObject context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
                AndroidJavaObject toastObject = toastClass.CallStatic<AndroidJavaObject>("makeText", context, message, toastClass.GetStatic<int>("LENGTH_SHORT"));
                toastObject.Call("show");
            }));
#endif

        }
    }


}
