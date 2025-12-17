Shader "UI/TileScrollInfinite"
{
    Properties
    {
        [MainTexture]_MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Tiling("Tiling (X,Y)", Vector) = (1,1,0,0)
        _Offset("Offset (X,Y)", Vector) = (0,0,0,0)
    }

    SubShader
    {
        Tags{
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _Tiling;
            float4 _Offset;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _Tiling.xy + _Offset.xy;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Sử dụng frac để tạo lặp vô hạn
                float2 tiledUV = frac(i.uv);
                fixed4 col = tex2D(_MainTex, tiledUV) * i.color;
                return col;
            }
            ENDCG
        }
    }
}
