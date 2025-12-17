Shader "UI/URP/RedAlertOuterBorder"
{
    Properties
    {
        _Color("Alert Color", Color) = (1, 0, 0, 1)
        _BorderSize("Border Size", Range(0, 0.5)) = 0.1
        _Softness("Softness", Range(0, 0.5)) = 0.15
        _PulseSpeed("Pulse Speed", Range(0.1, 20)) = 6
        _PulseIntensity("Pulse Intensity", Range(0, 3)) = 1
    }

    SubShader
    {
        Tags {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "OuterBorderAlert"
            Tags { "LightMode"="Universal2D" }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float _BorderSize;
            float _Softness;
            float _PulseSpeed;
            float _PulseIntensity;

            // -------------------------
            // Vertex
            // -------------------------
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            // -------------------------
            // Fragment
            // -------------------------
            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                // Tính khoảng cách đến mép gần nhất (top, bottom, left, right)
                float distToEdge = min(
                    min(uv.x, 1 - uv.x),
                    min(uv.y, 1 - uv.y)
                );

                // Viền ngoài = fade từ mép → vào trong
                float border = 1 - smoothstep(_BorderSize, _BorderSize + _Softness, distToEdge);

                if (border <= 0)
                    return float4(0,0,0,0);

                // Pulse animation
                float pulse = (sin(_Time.y * _PulseSpeed) * 0.5 + 0.5) * _PulseIntensity;

                float4 col = _Color * pulse;
                col.a *= border;

                return col;
            }

            ENDHLSL
        }
    }
}
