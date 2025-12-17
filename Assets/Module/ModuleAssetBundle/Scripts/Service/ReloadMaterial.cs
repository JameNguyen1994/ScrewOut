using System.Collections.Generic;
using UnityEngine;

public class ReloadMaterial : MonoBehaviour
{
    public string materialName;
    public MeshRenderer meshRenderer;
    public string baseMap;
    public string normalMap;
    public string detailNormal;
    public ReloadTextureContain reloadTexture;

    public void Awake()
    {
        List<Material> materials = AssetBundleService.Config.Materials;

        for (int i = 0; i < materials.Count; i++)
        {
            if (materialName == materials[i].name)
            {
                Material newMat = new Material(materials[i]);
                newMat.name = materialName;

                for (int j = 0; j < reloadTexture.textures.Count; j++)
                {
                    if (baseMap == reloadTexture.textures[j].name)
                    {
                        newMat.SetTexture("_BaseMap", reloadTexture.textures[j]);
                        Debug.Log("[SET] _BaseMap " + baseMap);
                    }

                    if (normalMap == reloadTexture.textures[j].name)
                    {
                        newMat.SetTexture("_BumpMap", reloadTexture.textures[j]);
                        newMat.EnableKeyword("_NORMALMAP");
                        Debug.Log("[SET] _BumpMap " + normalMap);
                    }

                    if (detailNormal == reloadTexture.textures[j].name)
                    {
                        newMat.SetTexture("_DetailNormalMap", reloadTexture.textures[j]);
                        newMat.EnableKeyword("_DETAIL_NORMALMAP");
                        Debug.Log("[SET] _DetailNormalMap " + detailNormal);
                    }
                }

                meshRenderer.material = newMat;

                break;
            }
        }
    }
}