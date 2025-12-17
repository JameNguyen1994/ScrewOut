using System.Collections;
using UnityEngine;

/// <summary>
/// Global coroutine runner to handle async operations
/// for static loader classes (e.g. SpriteLoader, AudioLoader, etc.)
/// </summary>
public class AsyncRunner : MonoBehaviour
{
    private static AsyncRunner _instance;

    /// <summary>
    /// Returns the global runner instance.
    /// Automatically creates a hidden GameObject if not present.
    /// </summary>
    public static AsyncRunner Instance
    {
        get
        {
            if (_instance == null)
            {
                // Create a hidden GameObject to run coroutines
                var obj = new GameObject("[AsyncRunner]");
                _instance = obj.AddComponent<AsyncRunner>();
                DontDestroyOnLoad(obj);
                obj.hideFlags = HideFlags.HideInHierarchy;
            }

            return _instance;
        }
    }

    /// <summary>
    /// Allows other classes to start coroutines without needing a MonoBehaviour reference.
    /// </summary>
    public static Coroutine RunCoroutine(IEnumerator routine)
    {
        return Instance.StartCoroutine(routine);
    }

    /// <summary>
    /// Stop coroutine safely
    /// </summary>
    public static void StopRunning(Coroutine coroutine)
    {
        if (_instance != null && coroutine != null)
            _instance.StopCoroutine(coroutine);
    }
}