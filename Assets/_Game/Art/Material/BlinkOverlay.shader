Shader "Custom/BlinkOverlay"
{
    Properties
    {
        _BlinkColor("Blink Color", Color) = (1,0,0,1)
        _BlinkStrength("Blink Strength", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags{
            "RenderPipeline"="UniversalRenderPipeline"
            "Queue"="Transparent+1"
            "RenderType"="Transparent"
        }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "BlinkOverlayPass"
            Tags { "LightMode"="UniversalForward" } // ép URP forward pass
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BlinkColor;
                float  _BlinkStrength;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            float4 frag (Varyings IN) : SV_Target
            {
                return float4(_BlinkColor.rgb * _BlinkStrength, _BlinkColor.a);
            }
            ENDHLSL
        }
    }
}
