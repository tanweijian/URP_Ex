Shader "Hidden/PP/KawaseBlur"
{
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_BlitTexture);
	SAMPLER(sampler_BlitTexture);
	float4 _BlitTexture_TexelSize;
	float _Offset;

	struct Attributes
	{
		uint vertexID : SV_VertexID;
	};

	struct Varyings
	{
		float4 positionCS : POSITION;
		float2 uv         : TEXCOORD0;	
	};

	Varyings KawaseBlurVertex(Attributes input)
	{
		Varyings output = (Varyings)0;

		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.uv  = GetFullScreenTriangleTexCoord(input.vertexID);
		
		return output;
	}

	half4 KawaseBlurFragment(Varyings input): SV_Target
	{
		half4 color = float4(0, 0, 0, 0);

		color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(_Offset + 0.5, _Offset + 0.5) * _BlitTexture_TexelSize.xy);
		color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(_Offset - 0.5, _Offset + 0.5) * _BlitTexture_TexelSize.xy);
		color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(_Offset - 0.5, _Offset - 0.5) * _BlitTexture_TexelSize.xy);
		color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv + float2(_Offset + 0.5, _Offset - 0.5) * _BlitTexture_TexelSize.xy);
		
		return color;
	}
	
	ENDHLSL
	
	SubShader
	{
		ZWrite Off ZTest Always Blend Off Cull Off

		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex KawaseBlurVertex
			#pragma fragment KawaseBlurFragment
			
			ENDHLSL
		}
	}
}