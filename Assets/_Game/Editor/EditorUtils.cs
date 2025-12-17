#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Place in an Editor folder
public static class EditorUtils
{
    /// <summary>
    /// Gets or creates a Unity Editor component for a given target object.
    /// </summary>
    public static T GetEditorComponent<T>(Object target) where T : Editor
    {
        if (target == null)
            return null;

        // This will create an Editor of type T for the target.
        // If T is not compatible with the target, Unity will log an error.
        return Editor.CreateEditor(target, typeof(T)) as T;
    }

    /// <summary>
    /// Gets or creates a default Unity Editor for a given target object.
    /// </summary>
    public static Editor GetEditorComponent(Object target)
    {
        if (target == null)
            return null;

        return Editor.CreateEditor(target);
    }

    /// <summary>
    /// Gets an EditorWindow of type T, opens it if not open.
    /// </summary>
    public static T GetEditorWindow<T>(string title = null) where T : EditorWindow
    {
        T window = EditorWindow.GetWindow<T>();
        if (!string.IsNullOrEmpty(title))
            window.titleContent = new GUIContent(title);
        window.Show();
        return window;
    }
}
#endif
