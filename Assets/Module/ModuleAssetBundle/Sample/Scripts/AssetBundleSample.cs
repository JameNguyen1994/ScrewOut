using UnityEngine;
using Cysharp.Threading.Tasks;

public class AssetBundleSample : MonoBehaviour
{
    public Transform root;
    public TMPro.TMP_InputField inputField;

    private int indexModel;
    private GameObject levelMap;

    public void OnClickLoad()
    {
        if (levelMap != null)
        {
            DestroyImmediate(levelMap);
        }

        LoadLevel().Forget();
    }

    private async UniTask LoadLevel()
    {
        int level = int.Parse(inputField.text);
        Debug.Log(">>>> Load Level " + level);

        GameObject prefabLevel = await AssetBundleService.LoadPrefabAsync(level);

        if (prefabLevel != null)
        {
            Debug.Log("[AssetBundle] LoadAsync Success!!!");

            levelMap = Instantiate(prefabLevel);
            levelMap.name = $"Level_{level}_Load_{indexModel++}";
            levelMap.transform.SetParent(root, false);
            levelMap.transform.position = Vector3.zero;
            levelMap.transform.rotation = Quaternion.identity;
        }
        else
        {
            Debug.LogError("[AssetBundle] LoadAsync Failed!!!");
        }
    }
}