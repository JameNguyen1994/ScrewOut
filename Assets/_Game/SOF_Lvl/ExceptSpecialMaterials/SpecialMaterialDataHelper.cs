using UnityEngine;

public class SpecialMaterialDataHelper : Singleton<SpecialMaterialDataHelper>
{
    [SerializeField] private SpecialDatasSO specialDatasSO;

    public bool IsInListExcept(Material currentMaterial)
    {
        bool isInstance = specialDatasSO.lstSpecialMaterialDataStr.Exists(m => IsMaterialInstanceOf(currentMaterial, m));
        var count = specialDatasSO.lstSpecialMaterialDataStr.Count;
        Debug.Log(isInstance
            ? $"Material {currentMaterial.name} là instance của một material trong danh sách {count}"
            : $"Material {currentMaterial.name} KHÔNG phải instance của bất kỳ material nào trong danh sách {count}");

        return isInstance;
    }
    private bool IsMaterialInstanceOf(Material instance, string original)
    {
       // if (instance == null || original == null) return false;

        // Check cùng shader
       // if (instance.shader != original.shader) return false;

        // Check các property quan trọng (có thể mở rộng)
        // if (instance.color != original.color) return false;

        // So sánh theo tên gốc (thường Unity clone sẽ thêm " (Instance)")
        if (instance.name.StartsWith(original))
            return true;

        return false;
    }
}
