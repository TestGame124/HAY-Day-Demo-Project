Shader "URP/Particles/OutlineMobile"
{
    Properties
    {
        // Main
        [MainTexture] _BaseMap ("Particle Texture", 2D) = "white" {}
        [MainColor]   _BaseColor ("Tint", Color) = (1,1,1,1)

        // Outline
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width (World)", Range(0,0.1)) = 0.01

        // Optional: helps cut fringing on sprites
        _AlphaCutoff ("Alpha Cutoff (Outline)", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "PreviewType"="Plane"
        }

        // Common particle-friendly state
        ZWrite Off
        ZTest LEqual
        Cull Off

        HLSLINCLUDE
        #pragma target 2.0

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        TEXTURE2D(_BaseMap);
        SAMPLER(sampler_BaseMap);

        CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;
            float4 _BaseColor;

            float4 _OutlineColor;
            float  _OutlineWidth;
            float  _AlphaCutoff;
        CBUFFER_END

        struct Attributes
        {
            float4 positionOS : POSITION;
            float3 normalOS   : NORMAL;
            float2 uv         : TEXCOORD0;
            float4 color      : COLOR;      // Particle system vertex color (lifetime color, etc.)
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
            float2 uv          : TEXCOORD0;
            float4 color       : COLOR;
        };

        inline float4 SampleParticle(float2 uv, float4 vtxColor)
        {
            float2 uvT = TRANSFORM_TEX(uv, _BaseMap);
            float4 tex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uvT);
            return tex * _BaseColor * vtxColor;
        }

        ENDHLSL

        // -------- OUTLINE PASS --------
        // Renders an expanded silhouette behind the particle.
        Pass
        {
            Name "Outline"
            Tags { "LightMode"="UniversalForward" }

            // Draw only the backfaces of the expanded mesh to create a clean border
            Cull Front

            // Typical particle blending; change to One One for additive look
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vertOutline
            #pragma fragment fragOutline

            Varyings vertOutline(Attributes IN)
            {
                Varyings OUT;

                // Expand along normal in object space (cheap).
                // For particle quads, normals exist; for some billboards they may be constant, which is fine.
                float3 posOS = IN.positionOS.xyz + (IN.normalOS * _OutlineWidth);

                float4 posWS = float4(TransformObjectToWorld(posOS), 1.0);
                OUT.positionHCS = TransformWorldToHClip(posWS.xyz);

                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 fragOutline(Varyings IN) : SV_Target
            {
                // Use alpha from the particle texture so outline follows sprite shape
                float4 p = SampleParticle(IN.uv, IN.color);

                // Optional cutoff to avoid outlining faint transparent pixels
                clip(p.a - _AlphaCutoff);

                // Outline color alpha is modulated by particle alpha
                return half4(_OutlineColor.rgb, _OutlineColor.a * p.a);
            }
            ENDHLSL
        }

        // -------- MAIN PASS --------
        Pass
        {
            Name "Particle"
            Tags { "LightMode"="UniversalForward" }

            // Standard alpha blending for particles
            // For additive particles, use: Blend One One
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float4 posWS = float4(TransformObjectToWorld(IN.positionOS.xyz), 1.0);
                OUT.positionHCS = TransformWorldToHClip(posWS.xyz);
                OUT.uv = IN.uv;
                OUT.color = IN.color;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 p = SampleParticle(IN.uv, IN.color);
                return (half4)p;
            }
            ENDHLSL
        }
    }

    FallBack Off
}