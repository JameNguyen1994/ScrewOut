Shader "Custom/URP/CustomMobileLit"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _Metallic ("Metallic", Range(0,1)) = 0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _EmissionColor ("Emission", Color) = (0,0,0,0)

        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimStrength ("Rim Strength", Range(0,2)) = 0.5
        _RimPower ("Rim Power", Range(0.1,8)) = 2.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _NORMALMAP

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            float4 _BaseColor;
            float _Metallic;
            float _Smoothness;
            float4 _EmissionColor;
            float4 _RimColor;
            float _RimStrength;
            float _RimPower;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float3 bitangentWS : TEXCOORD4;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.positionWS = TransformObjectToWorld(v.positionOS).xyz;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);

                float3 tangentWS = TransformObjectToWorldDir(v.tangentOS.xyz);
                float tangentSign = v.tangentOS.w * unity_WorldTransformParams.w;
                float3 bitangentWS = cross(o.normalWS, tangentWS) * tangentSign;

                o.tangentWS = tangentWS;
                o.bitangentWS = bitangentWS;
                o.uv = v.uv;
                return o;
            }

            float3 SampleNormal(float2 uv, float3 normalWS, float3 tangentWS, float3 bitangentWS)
            {
                #ifdef _NORMALMAP
                float3 n = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv).xyz * 2 - 1;
                float3x3 TBN = float3x3(tangentWS, bitangentWS, normalWS);
                return normalize(mul(TBN, n));
                #else
                return normalize(normalWS);
                #endif
            }

            float4 frag(Varyings i) : SV_Target
            {
                float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _BaseColor;
                float3 albedo = tex.rgb;

                Light mainLight = GetMainLight();
                float3 L = mainLight.direction;
                float3 lightCol = mainLight.color;

                float3 N = SampleNormal(i.uv, normalize(i.normalWS), i.tangentWS, i.bitangentWS);
                float3 V = normalize(_WorldSpaceCameraPos - i.positionWS);

                float NdotL = max(0, dot(N, L));
                float3 diffuse = albedo * NdotL * lightCol;

                float3 H = normalize(L + V);
                float spec = pow(max(dot(N, H), 0.0), 8 + _Smoothness * 64);
                float3 specular = spec * lerp(0.04, 1.0, _Metallic) * lightCol;

                float3 ambient = albedo * UNITY_LIGHTMODEL_AMBIENT.rgb;
                float3 emission = _EmissionColor.rgb;

                float rim = pow(1 - saturate(dot(N, V)), _RimPower);
                float3 rimLight = rim * _RimColor.rgb * _RimStrength;

                float3 finalRGB = diffuse + specular + rimLight + ambient + emission;
                return float4(finalRGB, tex.a);
            }

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}