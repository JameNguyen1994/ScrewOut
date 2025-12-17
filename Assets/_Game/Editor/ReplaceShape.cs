using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReplaceShape : EditorWindow
{
    private LevelMap levelMap;
    private GameObject cude;
    private GameObject screw;
    private GameObject parent;
    private List<GameObject> cudes;
    private string replaceName;
    private string replaceNameScrew;
    private bool checkScrewHaveMesh = true;

    [MenuItem("Tools/Replace Shape")]
    public static void ShowWindow()
    {
        ReplaceShape replaceShape = GetWindow<ReplaceShape>("Replace Shape");
    }

    private void OnGUI()
    {
        // Background
        var rect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(rect, new Color(0.13f, 0.13f, 0.13f));

        // Styles
        GUIStyle headerStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 20,
            normal = { textColor = Color.cyan }
        };

        GUIStyle sectionStyle = new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(15, 15, 10, 10),
            margin = new RectOffset(10, 10, 10, 10)
        };

        GUIStyle bigButtonStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold,
            fixedHeight = 25
        };

        GUILayout.Space(10);

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Prefab Settings", EditorStyles.boldLabel);

        levelMap = (LevelMap)EditorGUILayout.ObjectField("Level Map", levelMap, typeof(LevelMap), true);
        cude = (GameObject)EditorGUILayout.ObjectField("Cude", cude, typeof(GameObject), true);
        replaceName = EditorGUILayout.TextField("Replace Name", replaceName);

        if (cudes != null && cudes.Count > 0)
        {
            for (int i = 0; i < cudes.Count; i++)
            {
                cudes[i] = (GameObject)EditorGUILayout.ObjectField("Cude ID" + i, cudes[i], typeof(GameObject), true);
            }
        }

        if (GUILayout.Button("⚡Add Cude", bigButtonStyle))
        {
            if (cudes == null)
            {
                cudes = new List<GameObject>();
            }

            cudes.Add(null);
        }

        if (GUILayout.Button("⚡Clear Cude", bigButtonStyle))
        {
            cudes = new List<GameObject>();
        }

        if (GUILayout.Button(" 🏗️ UpdateShape", bigButtonStyle))
        {
            UpdateShape();
        }

        GUILayout.EndVertical();

        GUILayout.BeginVertical(sectionStyle);
        GUILayout.Label("⚙️ Screw Settings", EditorStyles.boldLabel);

        parent = (GameObject)EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true);
        screw = (GameObject)EditorGUILayout.ObjectField("Screw", screw, typeof(GameObject), true);
        replaceNameScrew = EditorGUILayout.TextField("Replace Name", replaceNameScrew);
        checkScrewHaveMesh = EditorGUILayout.Toggle("Check Screw Have Mesh", checkScrewHaveMesh);

        if (GUILayout.Button("⚡Load screw", bigButtonStyle))
        {
            screw = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Game/Prefabs/level_new_control/screw.prefab");
        }

        if (GUILayout.Button("🏗️ Replace Screw", bigButtonStyle))
        {
            Transform[] transforms = parent.GetComponentsInChildren<Transform>();

            foreach (Transform t in transforms)
            {
                if (!checkScrewHaveMesh || (checkScrewHaveMesh && t.GetComponent<MeshRenderer>() != null))
                {
                    if (t.name.ToLower().Contains(replaceNameScrew.ToLower()))
                    {
                        GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(screw, t.parent);
                        childInstance.transform.position = t.transform.position;
                        DestroyImmediate(t.gameObject);
                    }
                }
            }
        }

        GUILayout.EndVertical();
    }

    public void UpdateShape()
    {
        bool isRandom = false;

        for (int i = 0; i < levelMap.LstShape.Count; i++)
        {
            if (levelMap.LstShape[i].GetComponent<MeshRenderer>() != null)
            {
                if (levelMap.LstShape[i].name.ToLower().Contains(replaceName.ToLower()))
                {
                    //UpdateModelShape(levelMap.LstShape[i], !isRandom ? Vector3.zero : new Vector3(90, 0, 0));
                    //isRandom = !isRandom;
                    UpdateModelShape(levelMap.LstShape[i]);
                }
            }
        }
    }

    private void UpdateModelShape(Shape shape)
    {
        MeshFilter meshFilter = shape.GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = shape.GetComponent<MeshRenderer>();

        //Remove mesh
        DestroyImmediate(meshFilter);
        DestroyImmediate(meshRenderer);

        Transform hole = shape.transform.GetChild(0);

        if (cudes != null && cudes.Count > 0)
        {
            GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(cudes[Random.Range(0, cudes.Count)], shape.transform);

            childInstance.transform.localPosition = Vector3.zero;
            childInstance.transform.SetSiblingIndex(0);

            shape.SetMeshRenderer(childInstance.GetComponent<MeshRenderer>());
        }
        else
        {
            GameObject childInstance = (GameObject)PrefabUtility.InstantiatePrefab(cude, shape.transform);

            childInstance.transform.localPosition = Vector3.zero;
            childInstance.transform.SetSiblingIndex(0);

            shape.SetMeshRenderer(childInstance.GetComponent<MeshRenderer>());
        }
    }
}