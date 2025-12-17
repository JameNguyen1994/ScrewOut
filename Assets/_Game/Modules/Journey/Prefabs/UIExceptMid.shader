Shader "UI/CutMiddle_DoubleMirror"
{
    Properties
    {
        [MainTexture]_MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _ExceptWidth("Except Width", Range(0,0.5)) = 0.2
        _GapY("Vertical Gap", Range(0,1)) = 0.05
    }

    SubShader
    {
        Tags{"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
        LOD 100

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
            float _ExceptWidth;
            float _GapY;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Loại bỏ phần giữa
                float leftLimit = _ExceptWidth;
                float rightLimit = 1.0 - _ExceptWidth;

                if (i.uv.x > leftLimit && i.uv.x < rightLimit)
                    discard;

                // Nhân đôi trên-dưới
                float2 uv = i.uv;
                float2 uvTop = uv;
                float2 uvBottom = uv;

                // Dời vị trí trên - dưới
                uvTop.y = saturate(uv.y + _GapY);
                uvBottom.y = saturate(1.0 - uv.y + _GapY);

                fixed4 colTop = tex2D(_MainTex, uvTop);
                fixed4 colBottom = tex2D(_MainTex, uvBottom);

                // Kết hợp cả 2 lớp (additive blend nhẹ)
                fixed4 col = lerp(colTop, colBottom, step(0.5, uv.y));
                col.a *= i.color.a;
                col.rgb *= i.color.rgb;
                return col;
            }
            ENDCG
        }
    }
}
