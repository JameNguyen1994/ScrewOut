Shader "MatCap/Bumped/Textured Multiply"
{
    Properties
    {
        _MainTex ("Base (RGBA)", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _MatCap ("MatCap (RGBA)", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        [Toggle(MATCAP_ACCURATE)] _MatCapAccurate ("Accurate Calculation", Int) = 0

        [Enum(Opaque,0,Transparent,1)] _Surface ("Surface Type", Int) = 0

        [HideInInspector] _SrcBlend ("_SrcBlend", Float) = 1
        [HideInInspector] _DstBlend ("_DstBlend", Float) = 0
        [HideInInspector] _ZWrite ("_ZWrite", Float) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Pass
        {
            Blend [_SrcBlend] [_DstBlend]
            ZWrite [_ZWrite]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma shader_feature MATCAP_ACCURATE
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv_bump : TEXCOORD1;
            #if MATCAP_ACCURATE
                fixed3 tSpace0 : TEXCOORD2;
                fixed3 tSpace1 : TEXCOORD3;
                fixed3 tSpace2 : TEXCOORD4;
                UNITY_FOG_COORDS(5)
            #else
                float3 c0 : TEXCOORD2;
                float3 c1 : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            #endif
            };

            uniform float4 _MainTex_ST;
            uniform float4 _BumpMap_ST;
            uniform float4 _BaseColor;

            v2f vert (appdata_tan v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.uv_bump = TRANSFORM_TEX(v.texcoord,_BumpMap);

            #if MATCAP_ACCURATE
                fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
                fixed3 worldTangent = UnityObjectToWorldDir(v.tangent.xyz);
                fixed3 worldBinormal = cross(worldNormal, worldTangent) * v.tangent.w;
                o.tSpace0 = fixed3(worldTangent.x, worldBinormal.x, worldNormal.x);
                o.tSpace1 = fixed3(worldTangent.y, worldBinormal.y, worldNormal.y);
                o.tSpace2 = fixed3(worldTangent.z, worldBinormal.z, worldNormal.z);
            #else
                v.normal = normalize(v.normal);
                v.tangent = normalize(v.tangent);
                TANGENT_SPACE_ROTATION;
                o.c0 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[0].xyz));
                o.c1 = mul(rotation, normalize(UNITY_MATRIX_IT_MV[1].xyz));
            #endif

                UNITY_TRANSFER_FOG(o, o.pos);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _BumpMap;
            sampler2D _MatCap;

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 tex = tex2D(_MainTex, i.uv) * _BaseColor;
                fixed3 normals = UnpackNormal(tex2D(_BumpMap, i.uv_bump));

                float4 matcapCol;

            #if MATCAP_ACCURATE
                float3 worldNorm;
                worldNorm.x = dot(i.tSpace0.xyz, normals);
                worldNorm.y = dot(i.tSpace1.xyz, normals);
                worldNorm.z = dot(i.tSpace2.xyz, normals);
                worldNorm = mul((float3x3)UNITY_MATRIX_V, worldNorm);
                matcapCol = tex2D(_MatCap, worldNorm.xy * 0.5 + 0.5);
            #else
                half2 capCoord = half2(dot(i.c0, normals), dot(i.c1, normals));
                matcapCol = tex2D(_MatCap, capCoord*0.5+0.5);
            #endif

                // Multiply màu
                float4 mc;
                mc.rgb = matcapCol.rgb * tex.rgb * unity_ColorSpaceDouble.rgb;
                // Alpha trong suốt
                mc.a   = tex.a * matcapCol.a * _BaseColor.a;

                UNITY_APPLY_FOG(i.fogCoord, mc);
                return mc;
            }
            ENDCG
        }
    }

    CustomEditor "SurfaceShaderGUI"
    Fallback "Transparent/Diffuse"
}
