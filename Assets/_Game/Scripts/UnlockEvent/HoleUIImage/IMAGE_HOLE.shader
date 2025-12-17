Shader "UI/HoleShaderURP"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _SmallTex ("Small Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _HoleCenter ("Hole Center", Vector) = (0.5, 0.5, 0, 0)
        _HoleSize ("Hole Size", Vector) = (0.1, 0.1, 0, 0)
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_SmallTex);
            SAMPLER(sampler_SmallTex);

            float4 _Color;
            float2 _HoleCenter;
            float2 _HoleSize;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                half4 finalColor = texColor * _Color;

                // Tính UV của Small Image
                float2 holeMin = _HoleCenter - _HoleSize * 0.5;
                float2 uvInHole = (IN.uv - holeMin) / _HoleSize;

                // Kiểm tra nếu UV nằm trong phạm vi của Small Image
                if (uvInHole.x >= 0.0 && uvInHole.x <= 1.0 &&
                    uvInHole.y >= 0.0 && uvInHole.y <= 1.0)
                {
                    // Lấy màu của Small Image
                    half4 smallTexColor = SAMPLE_TEXTURE2D(_SmallTex, sampler_SmallTex, uvInHole);

                    // Nếu pixel của Small Image có màu (alpha > 0), đục lỗ
                    if (smallTexColor.a > 0.0)
                    {
                        finalColor.a = 0.0;
                    }
                   finalColor.a *= (1 - smallTexColor.a);

                }

                return finalColor;
            }
            ENDHLSL
        }
    }
}
