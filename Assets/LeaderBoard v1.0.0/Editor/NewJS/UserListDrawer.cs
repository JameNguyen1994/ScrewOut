#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class UserListDrawer
{
    public static void DrawUserList(
        List<U> users,
        ref Vector2 scroll,
        UserListSettings settings,
        Action<int> onRemove = null
    )
    {
        if (users == null)
        {
            EditorGUILayout.HelpBox("User list is NULL", MessageType.Warning);
            return;
        }

        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(320));

        if (users.Count == 0)
        {
            GUILayout.Label("No users.");
            EditorGUILayout.EndScrollView();
            return;
        }

        int total = users.Count;
        int top = settings.topCount;
        int bottom = settings.bottomCount;

        // Nếu số lượng nhỏ -> hiển thị tất cả
        if (total <= top + bottom)
        {
            DrawRange(users, 0, total, settings, onRemove);
        }
        else
        {
            // TOP
            DrawRange(users, 0, top, settings, onRemove);

            // Hidden indicator
            int hiddenCount = total - top - bottom;
            if (hiddenCount > 0)
            {
                GUILayout.Space(6);
                GUILayout.Label(
                    $"... {hiddenCount} users hidden ...",
                    EditorStyles.centeredGreyMiniLabel
                );
                GUILayout.Space(6);
            }

            // BOTTOM
            DrawRange(users, total - bottom, bottom, settings, onRemove);
        }

        EditorGUILayout.EndScrollView();
    }


    private static void DrawRange(
        List<U> users,
        int start,
        int count,
        UserListSettings settings,
        Action<int> onRemove
    )
    {
        int end = start + count;
        int col = 0;

        EditorGUILayout.BeginHorizontal();

        for (int i = start; i < end; i++)
        {
            U u = users[i];

            EditorGUILayout.BeginVertical("box", GUILayout.Width(210));

            GUILayout.Label($"#{i}", EditorStyles.boldLabel);

            if (settings.showName)
                u.n = EditorGUILayout.TextField("Name", u.n);

            if (settings.showPoint)
                u.p = EditorGUILayout.IntField("Points", u.p);

            if (settings.showAvatar)
                u.a = EditorGUILayout.IntField("AvatarIndex", u.a);

            if (settings.showBorder)
                u.b = EditorGUILayout.IntField("BorderIndex", u.b);

            GUILayout.Space(3);

            if (onRemove != null && GUILayout.Button("X", GUILayout.Width(25)))
            {
                onRemove(i);
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                return;
            }

            EditorGUILayout.EndVertical();

            col++;

            if (col % settings.maxPerRow == 0)
            {
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif
