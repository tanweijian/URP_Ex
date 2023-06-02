Shader "UI/DefaultEx"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"
            
            HLSLPROGRAM
            
            #pragma vertex DefaultPassVertex
            #pragma fragment DefaultPassFragment
            #pragma target 2.0

            #include "UICommon.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma multi_compile _ _RENDER_COLORSPACE_GAMMA
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
        
            struct Attributes
            {
                float4 positionOS : POSITION; 
                float4 color      : COLOR;
                float4 uv         : TEXCOORD0;  // z, w is param
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color       : COLOR;
                float4 uv         : TEXCOORD0;
                float4 mask       : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half4 _Color;
            CBUFFER_END
            
            float4 _ClipRect;
            half4 _TextureSampleAdd;
            float _UIMaskSoftnessX;
            float _UIMaskSoftnessY;
            int _UIVertexColorAlwaysGammaSpace;

            Varyings DefaultPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                
                const float4 positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionCS = positionCS;

                float2 pixelSize = positionCS.w;
                pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

                const float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
                output.uv.xy = TRANSFORM_TEX(input.uv.xy, _MainTex);
                output.mask = float4(input.positionOS.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

            #if defined(_RENDER_COLORSPACE_GAMMA)
                if (!_UIVertexColorAlwaysGammaSpace)
                {
                    input.color.rgb = UILinearToGamma(input.color.rgb);
                }
            #else
                if (_UIVertexColorAlwaysGammaSpace)
                {
                    if(!IsGammaSpace())
                    {
                        input.color.rgb = UIGammaToLinear(input.color.rgb);
                    }
                }
            #endif
            
                output.uv.zw = input.uv.zw;

                output.color = input.color * _Color;
                return output;
            }

            half4 DefaultPassFragment(Varyings input) : SV_Target
            {
                //Round up the alpha color coming from the interpolator (to 1.0/256.0 steps)
                //The incoming alpha could have numerical instability, which makes it very sensible to
                //HDR color transparency blend, when it blends with the world's texture.
                const half alphaPrecision = half(0xff);
                const half invAlphaPrecision = half(1.0 / alphaPrecision);
                input.color.a = round(input.color.a * alphaPrecision) * invAlphaPrecision;

                half4 color = input.color * (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv.xy) + _TextureSampleAdd);

            #ifdef UNITY_UI_CLIP_RECT
                half2 m = saturate((_ClipRect.zw - _ClipRect.xy - abs(input.mask.xy)) * input.mask.zw);
                color.a *= m.x * m.y;
            #endif

            #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
            #endif

            #if defined(_RENDER_COLORSPACE_GAMMA)
                float3 luminance = half3(0.0396819152, 0.458021790, 0.00609653955);
            #else
                float3 luminance = half3(0.2126729000, 0.715152200, 0.07217500000);
            #endif
                color.rgb = lerp(dot(color.rgb, luminance), color.rgb, step(input.uv.z, 0.8));

                color.rgb *= color.a;

                return color;
            }
            
            ENDHLSL
        }
    }
}
