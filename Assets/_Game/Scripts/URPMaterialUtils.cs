using UnityEngine;
using UnityEngine.Rendering;

public static class URPMaterialUtils
{
    public enum SurfaceType
    {
        Opaque = 0,
        Transparent = 1
    }

    /// <summary>
    /// Clone material gốc và đổi surface type (không ảnh hưởng asset gốc).
    /// </summary>
    public static Material GetClonedWithSurface(Material original, SurfaceType type)
    {
        if (original == null) return null;

        Material mat = new Material(original);
        ApplySurfaceType(mat, type);
        return mat;
    }

    /// <summary>
    /// Đổi surface type trực tiếp trên material gốc (ảnh hưởng toàn bộ object share material này).
    /// </summary>
    public static void SetSurfaceDirect(Material mat, SurfaceType type)
    {
        if (mat == null) return;

        ApplySurfaceType(mat, type);
    }

    /// <summary>
    /// Logic đổi surface type (dùng chung cho clone & direct).
    /// </summary>
    private static void ApplySurfaceType(Material mat, SurfaceType type)
    {
        switch (type)
        {
            case SurfaceType.Opaque:
                mat.SetFloat("_Surface", 0); // Opaque
                mat.renderQueue = (int)RenderQueue.Geometry;
                mat.SetShaderPassEnabled("DepthOnly", true);
                mat.SetShaderPassEnabled("ShadowCaster", true);
                mat.SetOverrideTag("RenderType", "Opaque");
                mat.SetInt("_ZWrite", 1);

                // Blend mode cho Opaque
                mat.SetInt("_SrcBlend", (int)BlendMode.One);
                mat.SetInt("_DstBlend", (int)BlendMode.Zero);
                break;

            case SurfaceType.Transparent:
                mat.SetFloat("_Surface", 1); // Transparent
                mat.renderQueue = (int)RenderQueue.Transparent;
                mat.SetShaderPassEnabled("DepthOnly", false);
                mat.SetShaderPassEnabled("ShadowCaster", false);
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_ZWrite", 0);

                // Blend mode cho Transparent
                mat.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
                break;
        }
    }

}
