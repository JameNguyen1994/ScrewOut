Shader "Custom/LitBlinkColor"
{
    Properties
    {
        // Standard Lit
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _Smoothness("Smoothness", Range(0,1)) = 0.5

        // Blink
        _BlinkColor("Blink Color", Color) = (1,0,0,1)
        _BlinkSpeed("Blink Speed", Float) = 2.0
        _BlinkStrength("Blink Strength", Range(0,1)) = 1.0

        // Surface Options (ẩn, do ShaderGUI quản lý)
        [HideInInspector] _WorkflowMode("Workflow Mode", Float) = 1.0
        [HideInInspector] _Surface("__surface", Float) = 0.0         // 0=Opaque,1=Transparent
        [HideInInspector] _Blend("__blend", Float) = 0.0             // 0=Alpha,1=Premultiply
        [HideInInspector] _Cull("__cull", Float) = 2.0               // 0=Off,1=Front,2=Back
        [HideInInspector] _AlphaClip("__clip", Float) = 0.0
        [HideInInspector] _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
        [HideInInspector] _ReceiveShadows("Receive Shadows", Float) = 1.0
        [HideInInspector] _ZWrite("__zw", Float) = 1.0
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
        [HideInInspector] _DstBlend("__dst", Float) = 0.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend [_SrcBlend][_DstBlend]
            ZWrite [_ZWrite]
            Cull [_Cull]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 positionWS  : TEXCOORD1;
                float3 normalWS    : TEXCOORD2;
                float2 uv          : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseMap_ST;
            float4 _BaseColor;
            float4 _BlinkColor;
            float _BlinkSpeed;
            float _BlinkStrength;
            float _Smoothness;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                VertexPositionInputs posInputs = GetVertexPositionInputs(IN.positionOS.xyz);
                OUT.positionHCS = posInputs.positionCS;
                OUT.positionWS = posInputs.positionWS;
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Base + Blink
                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half3 baseColor = baseTex.rgb * _BaseColor.rgb;
                float blinkFactor = (sin(_Time.y * _BlinkSpeed) * 0.5 + 0.5) * _BlinkStrength;
                half3 finalColor = lerp(baseColor, _BlinkColor.rgb, blinkFactor);

                // SurfaceData
                SurfaceData surfaceData;
                surfaceData.albedo = finalColor;
                surfaceData.alpha = baseTex.a * _BaseColor.a;
                surfaceData.metallic = 0;
                surfaceData.specular = 0;
                surfaceData.smoothness = _Smoothness;
                surfaceData.normalTS = float3(0,0,1);
                surfaceData.occlusion = 1;
                surfaceData.emission = 0;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;

                // InputData
                InputData inputData = (InputData)0;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalize(IN.normalWS);
                inputData.viewDirectionWS = normalize(GetWorldSpaceViewDir(IN.positionWS));

                return UniversalFragmentPBR(inputData, surfaceData);
            }
            ENDHLSL
        }
    }

    CustomEditor "UnityEditor.Rendering.Universal.ShaderGUI.LitShader"
}
