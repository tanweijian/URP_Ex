Shader "Hidden/PP/GaussianBlur"
{
	HLSLINCLUDE

	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

	TEXTURE2D_X(_BlitTexture); SAMPLER(sampler_BlitTexture);
	float4 _BlurOffset;

	struct Attributes
	{
		uint vertexID : SV_VertexID;
	};

	struct Varyings
	{
		float4 positionCS : POSITION;
		float2 uv         : TEXCOORD0;	
		float4 uv01       : TEXCOORD1;
		float4 uv23       : TEXCOORD2;
		float4 uv45       : TEXCOORD3;
	};

	Varyings GaussianBlurVertex(Attributes input)
	{
		Varyings output = (Varyings)0;

		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
		output.uv  = GetFullScreenTriangleTexCoord(input.vertexID);
		
		output.uv01 = output.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1);
		output.uv23 = output.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 2.0;
		output.uv45 = output.uv.xyxy + _BlurOffset.xyxy * float4(1, 1, -1, -1) * 6.0;
		
		return output;
	}

	half4 GaussianBlurFragment(Varyings input): SV_Target
	{
		half4 color = float4(0, 0, 0, 0);
		
		color += 0.40 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv);
		color += 0.15 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv01.xy);
		color += 0.15 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv01.zw);
		color += 0.10 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv23.xy);
		color += 0.10 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv23.zw);
		color += 0.05 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv45.xy);
		color += 0.05 * SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.uv45.zw);
		
		return color;
	}
	
	ENDHLSL
	
	SubShader
	{
		ZWrite Off ZTest Always Blend Off Cull Off

		Pass
		{
			HLSLPROGRAM
			
			#pragma vertex GaussianBlurVertex
			#pragma fragment GaussianBlurFragment
			
			ENDHLSL
		}
	}
}