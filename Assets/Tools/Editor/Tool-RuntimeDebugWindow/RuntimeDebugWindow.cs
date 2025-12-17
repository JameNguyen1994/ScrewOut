using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class Example
{
    public static int fieldValue = 1;

    public static int intValue { get; set; }
    public static float floatValue;
    public static string stringValue;
    public static bool boolValue;
    public static readonly bool readonlyValue;
    public static TestEnum enumValue { get; set; }

    public enum TestEnum
    {
        a,
        b
    }

    static Example()
    {
        intValue = 0;
        floatValue = 0;
        stringValue = "";
        boolValue = false;
        enumValue = TestEnum.b;
    }
}

public class RuntimeDebugWindow : EditorWindow
{
    private bool editMode = false;
    private bool showFields = false;
    private bool showProperties = false;
    private bool saveReadOnly = false;
    private string className = "Example";
    private Vector2 scrollPosition = Vector2.zero;
    private Type staticClassType;

    [MenuItem("Tam's Window/Runtime Debug Window")]
    public static void ShowWindow()
    {
        GetWindow<RuntimeDebugWindow>("Runtime Debug Window");
    }

    void OnGUI()
    {
        GUILayout.Label("====================================================MENU======================================================");
        if (GUILayout.Button("SAVE"))
        {
            Save();
        }
        DrawHeader();
        DrawModeToggle();

        staticClassType = GetType(className);
        if (staticClassType == null)
        {
            EditorGUILayout.LabelField($"Class name invalid: {className}");
            return;
        }
        GUILayout.Label("==========================================================================================================");
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawFields();
        DrawProperties();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Common class: ");
        if (GUILayout.Button("Load Example", GUILayout.Width(100)))
        {
            className = "Example";
            Repaint();
        }
        if (GUILayout.Button("GameConfig", GUILayout.Width(100)))
        {
            className = "GameConfig";
            Repaint();
        }
        if (GUILayout.Button("InGameData", GUILayout.Width(100)))
        {
            className = "InGameData";
            Repaint();
        }
        GUILayout.EndHorizontal();
        className = EditorGUILayout.TextField(className);

    }

    private void DrawModeToggle()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Settings: ");

        /*        if (GUILayout.Button(editMode ? "Edit Mode" : "Only View Mode"))
                {
                    editMode = !editMode;
                }*/
        editMode = GUILayout.Toggle(editMode, "Editable");
        saveReadOnly = GUILayout.Toggle(saveReadOnly, "Can edit value readonly");
        /*        if (GUILayout.Button(saveReadOnly ? "Force save readonly" : "not save readonly"))
                {
                    saveReadOnly = !saveReadOnly;
                }*/
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();
    }

    private void DrawFields()
    {
        FieldInfo[] fields = staticClassType.GetFields();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"----------------------------------Fields: {fields.Length}-------------------------------");
        showFields = EditorGUILayout.Toggle(showFields);
        GUILayout.EndHorizontal();

        if (showFields)
        {
            foreach (var field in fields)
            {
                DrawField(field);
            }
        }

        EditorGUILayout.Space(20);
    }

    private void DrawField(FieldInfo field)
    {
        object value = field.GetValue(null);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(field.Name + $"{(field.IsInitOnly ? " (Read only)" : "")}");

        if (editMode && (!field.IsInitOnly || saveReadOnly))
        {
            DrawEditableField(field, value);
        }
        else
        {
            EditorGUILayout.LabelField(value.ToString());
        }
        GUILayout.EndHorizontal();
    }

    private void DrawEditableField(FieldInfo field, object value)
    {
        if (field.FieldType == typeof(int))
        {
            int intValue = EditorGUILayout.IntField((int)value);
            field.SetValue(null, intValue);
        }
        else if (field.FieldType == typeof(float))
        {
            float floatValue = EditorGUILayout.FloatField((float)value);
            field.SetValue(null, floatValue);
        }
        else if (field.FieldType == typeof(string))
        {
            string stringValue = EditorGUILayout.TextField((string)value);
            field.SetValue(null, stringValue);
        }
        else if (field.FieldType == typeof(bool))
        {
            bool boolValue = EditorGUILayout.Toggle((bool)value);
            field.SetValue(null, boolValue);
        }
        else if (field.FieldType.IsEnum)
        {
            Enum enumValue = EditorGUILayout.EnumPopup((Enum)value);
            field.SetValue(null, enumValue);
        }
    }

    private void DrawProperties()
    {
        PropertyInfo[] properties = staticClassType.GetProperties();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"---------------------------------Properties: {properties.Length}---------------------------");
        showProperties = EditorGUILayout.Toggle(showProperties);
        GUILayout.EndHorizontal();

        if (showProperties)
        {
            foreach (var property in properties)
            {
                DrawProperty(property);
            }
        }
    }

    private void DrawProperty(PropertyInfo property)
    {
        object value = property.GetValue(null);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(property.Name);

        if (editMode)
        {
            DrawEditableProperty(property, value);
        }
        else
        {
            EditorGUILayout.LabelField(value.ToString());
        }
        GUILayout.EndHorizontal();
    }

    private void DrawEditableProperty(PropertyInfo property, object value)
    {
        if (property.PropertyType == typeof(int))
        {
            int intValue = EditorGUILayout.IntField((int)value);
            property.SetValue(null, intValue);
        }
        else if (property.PropertyType == typeof(float))
        {
            float floatValue = EditorGUILayout.FloatField((float)value);
            property.SetValue(null, floatValue);
        }
        else if (property.PropertyType == typeof(string))
        {
            string stringValue = EditorGUILayout.TextField((string)value);
            property.SetValue(null, stringValue);
        }
        else if (property.PropertyType == typeof(bool))
        {
            bool boolValue = EditorGUILayout.Toggle((bool)value);
            property.SetValue(null, boolValue);
        }
        else if (property.PropertyType.IsEnum)
        {
            Enum enumValue = EditorGUILayout.EnumPopup((Enum)value);
            property.SetValue(null, enumValue);
        }

    }

    private Type GetType(string typeName)
    {
        var type = Type.GetType(typeName);
        if (type != null) return type;

        if (typeName.Contains("."))
        {
            var assemblyName = typeName.Substring(0, typeName.IndexOf('.'));
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
        }

        var currentAssembly = Assembly.GetExecutingAssembly();
        var referencedAssemblies = currentAssembly.GetReferencedAssemblies();
        foreach (var assemblyName in referencedAssemblies)
        {
            var assembly = Assembly.Load(assemblyName);
            if (assembly != null)
            {
                type = assembly.GetType(typeName);
                if (type != null) return type;
            }
        }

        return null;
    }
    private string FindClass(string className)
    {
        string[] scriptFiles = Directory.GetFiles(Application.dataPath, "*.cs", SearchOption.AllDirectories);

        foreach (string file in scriptFiles)
        {
            string[] lines = File.ReadAllLines(file);
            foreach (string line in lines)
            {
                if (line.Contains($"class {className}") || line.Contains($"class {className}"))
                {
                    Debug.Log($"Class '{className}' found in file: {file}");
                    return file;

                }
            }
        }
        Debug.Log("Not found class");
        return "";
    }
    private void Save()
    {
        // Path to the .cs file containing the class
        string filePath = FindClass(className);
        Debug.Log($"File path: {filePath}");
        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        string fileContent = File.ReadAllText(filePath);

        // Update field values in the file
        FieldInfo[] fields = staticClassType.GetFields();
        foreach (var field in fields)
        {
            if (!IsNoneValueCanEdit(field))
                continue;
            object value = field.GetValue(null);
            string pattern = $@"public static {GetShortTypeName(field.FieldType)} {field.Name} = .*?;";
            string replacement = $"public static {GetShortTypeName(field.FieldType)} {field.Name} = {FormatValue(value)}{(field.FieldType == typeof(float) ? "f" : "")};";
            //Debug.Log(fileContent);
            try
            {
                fileContent = Regex.Replace(fileContent, pattern, replacement);
            }
            catch
            {

            }
            //Debug.Log(pattern);
        }

        if (saveReadOnly)
        {
            foreach (var field in fields)
            {
                if (!IsNoneValueCanEdit(field))
                    continue;
                object value = field.GetValue(null);
                string isClass = field.FieldType.IsClass ? $"new {field.FieldType}" : "";
                string isStruct = field.FieldType.IsConstructedGenericType ? $"new {field.FieldType}" : "";
                string pattern = $@"public static readonly {GetShortTypeName(field.FieldType)} {field.Name} = .*?;";
                string replacement = $"public static readonly {GetShortTypeName(field.FieldType)} {field.Name} = {isClass}{isStruct} {FormatValue(value)}{(field.FieldType == typeof(float) ? "f" : "")};";
                //Debug.Log(fileContent);
                try
                {
                    fileContent = Regex.Replace(fileContent, pattern, replacement);

                }
                catch
                {

                }
                //Debug.Log(pattern);
            }
        }
        if (saveReadOnly)
        {
            foreach (var field in fields)
            {
                if (!IsNoneValueCanEdit(field))
                    continue;
                object value = field.GetValue(null);
                string isClass = field.FieldType.IsClass ? $"new {field.FieldType}" : "";
                string isStruct = field.FieldType.IsConstructedGenericType ? $"new {field.FieldType}" : "";
                string pattern = $@"public readonly static {GetShortTypeName(field.FieldType)} {field.Name} = .*?;";
                string replacement = $"public readonly static {GetShortTypeName(field.FieldType)} {field.Name} =  {isClass}{isStruct} {FormatValue(value)}{(field.FieldType == typeof(float) ? "f" : "")};";
                //Debug.Log(fileContent);
                try
                {
                    fileContent = Regex.Replace(fileContent, pattern, replacement);
                }
                catch
                {

                }
                //Debug.Log(pattern);
            }
        }

        // Update property values in the file
        PropertyInfo[] properties = staticClassType.GetProperties();
        foreach (var property in properties)
        {
            if (!IsNoneValueCanEdit(property))
                continue;
            object value = property.GetValue(null);
            string pattern = $@"public static {property.PropertyType.Name} {property.Name} {{ get; set; }}";
            string replacement = $@"public static {property.PropertyType.Name} {property.Name} {{ get; set; }} = {FormatValue(value)};";
            try
            {
                fileContent = Regex.Replace(fileContent, pattern, replacement);
            }
            catch
            {

            }
        }

        File.WriteAllText(filePath, fileContent);

        AssetDatabase.Refresh();
    }

    private string FormatValue(object value)
    {
        if (value is string)
        {
            return $"\"{value}\"";
        }
        else if (value is bool)
        {
            return value.ToString().ToLower();
        }
        else if (value.GetType().IsEnum)
        {
            return $"{value.GetType().Name}.{value}";
        }
        else
        {
            return value.ToString();
        }
    }
    public static string GetShortTypeName(Type type)
    {
        if (type == typeof(int))
            return "int";
        else if (type == typeof(float))
            return "float";
        else if (type == typeof(bool))
            return "bool";
        // Thêm các kiểu dữ liệu khác tương tự

        return type.Name; // Trong trường hợp không khớp, trả về tên đầy đủ
    }
    public bool IsNoneValueCanEdit(PropertyInfo property)
    {
        if (property.PropertyType == typeof(int))
        {
            return true;
        }
        else if (property.PropertyType == typeof(float))
        {
            return true;

        }
        else if (property.PropertyType == typeof(string))
        {
            return true;

        }
        else if (property.PropertyType == typeof(bool))
        {
            return true;

        }
        else if (property.PropertyType.IsEnum)
        {
            return true;

        }
        return false;
    }
    public bool IsNoneValueCanEdit(FieldInfo property)
    {
        if (property.FieldType == typeof(int))
        {
            return true;
        }
        else if (property.FieldType == typeof(float))
        {
            return true;

        }
        else if (property.FieldType == typeof(string))
        {
            return true;

        }
        else if (property.FieldType == typeof(bool))
        {
            return true;

        }
        else if (property.FieldType.IsEnum)
        {
            return true;

        }
        return false;
    }
}
#endif