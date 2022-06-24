Shader "HandDrawn/EdgeDetecion"
{
	Properties
	{
	
	}
	HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma multi_compile _Tex1 _Tex2 _Tex3 _Tex4 _Tex5

			#ifdef _Tex1
			#define _DepthNormalsTex			_DepthNormalsTex1
			#define sampler_DepthNormalsTex		sampler_DepthNormalsTex1
			#define _DepthNormalsTex_TexelSize	_DepthNormalsTex1_TexelSize
			#elif _Tex2
			#define _DepthNormalsTex _DepthNormalsTex2
			#define sampler_DepthNormalsTex  sampler_DepthNormalsTex2
			#define _DepthNormalsTex_TexelSize _DepthNormalsTex2_TexelSize
			#elif _Tex3
			#define _DepthNormalsTex _DepthNormalsTex3
			#define sampler_DepthNormalsTex  sampler_DepthNormalsTex3
			#define _DepthNormalsTex_TexelSize _DepthNormalsTex3_TexelSize
			#elif _Tex4
			#define _DepthNormalsTex _DepthNormalsTex4
			#define sampler_DepthNormalsTex  sampler_DepthNormalsTex4
			#define _DepthNormalsTex_TexelSize _DepthNormalsTex4_TexelSize
			#elif _Tex5
			#define _DepthNormalsTex _DepthNormalsTex5
			#define sampler_DepthNormalsTex  sampler_DepthNormalsTex5
			#define _DepthNormalsTex_TexelSize _DepthNormalsTex5_TexelSize
			#endif

			TEXTURE2D (_DepthNormalsTex);
			SAMPLER (sampler_DepthNormalsTex);

		CBUFFER_START(UnityPerMaterial)
			float2 _DepthNormalsTex_TexelSize;

			float _Thickness;

			//Used in advanced roberts cross
			float _DepthThreshold;
			float _DepthNormalThreshold;
			float _NormalThreshold;
		CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float3 viewSpaceDir : TEXCOORD0;
				float2 uv : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			Varyings vertedge(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.uv = input.uv;
				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				//output.viewSpaceDir = mul(UNITY_MATRIX_I_P,output.positionCS).xyz;

				VertexPositionInputs positionInputs = GetVertexPositionInputs(input.positionOS.xyz);
				//output.positionCS = positionInputs.positionCS; // Clip Space
				//output.positionWS = positionInputs.positionWS; // World Space
				output.viewSpaceDir = positionInputs.positionVS; // View Space

				return output;
			}

			half4 fragedge(Varyings input) : SV_TARGET
			{
				float2 bottomLeftUV = input.uv - float2(_DepthNormalsTex_TexelSize.x, _DepthNormalsTex_TexelSize.y) * _Thickness;
				float2 topRightUV = input.uv + float2(_DepthNormalsTex_TexelSize.x, _DepthNormalsTex_TexelSize.y) * _Thickness;  
				float2 bottomRightUV = input.uv + float2(_DepthNormalsTex_TexelSize.x * _Thickness, -_DepthNormalsTex_TexelSize.y * _Thickness);
				float2 topLeftUV = input.uv + float2(-_DepthNormalsTex_TexelSize.x * _Thickness, _DepthNormalsTex_TexelSize.y * _Thickness);

				float3 normal0 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, bottomLeftUV).rgb;
				float3 normal1 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, topRightUV).rgb;
				float3 normal2 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, bottomRightUV).rgb;
				float3 normal3 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, topLeftUV).rgb;

				float depth0 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, bottomLeftUV).a;
				float depth1 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, topRightUV).a;
				float depth2 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, bottomRightUV).a;
				float depth3 = SAMPLE_TEXTURE2D(_DepthNormalsTex, sampler_DepthNormalsTex, topLeftUV).a;

				// Transform the view normal from the 0...1 range to the -1...1 range.
				float3 viewNormal = normal0 * 2 - 1;
				float NdotV = 1 - dot(viewNormal, -input.viewSpaceDir);
				//float NdotV = viewNormal;

				// Return a value in the 0...1 range depending on where NdotV lies 
				// between _DepthNormalThreshold and 1.
				float normalThreshold01 = saturate((NdotV - _DepthNormalThreshold) / (1 - _DepthNormalThreshold));
				// Scale the threshold, and add 1 so that it is in the range of 1..._NormalThresholdScale + 1.
				float normalThreshold = normalThreshold01 + 1;

				// Modulate the threshold by the existing depth value;
				// pixels further from the screen will require smaller differences
				// to draw an edge.
				float depthThreshold = _DepthThreshold * depth0 * normalThreshold;

				float depthFiniteDifference0 = depth1 - depth0;
				float depthFiniteDifference1 = depth3 - depth2;
				// edgeDepth is calculated using the Roberts cross operator.
				// The same operation is applied to the normal below.
				// https://en.wikipedia.org/wiki/Roberts_cross
				float edgeDepth = sqrt(pow(depthFiniteDifference0, 2) + pow(depthFiniteDifference1, 2)) * 100;
				edgeDepth = edgeDepth > depthThreshold ? 0 : 1;

				float3 normalFiniteDifference0 = normal1 - normal0;
				float3 normalFiniteDifference1 = normal3 - normal2;
				// Dot the finite differences with themselves to transform the 
				// three-dimensional values to scalars.
				float edgeNormal = sqrt(dot(normalFiniteDifference0, normalFiniteDifference0) + dot(normalFiniteDifference1, normalFiniteDifference1));
				edgeNormal = edgeNormal > _NormalThreshold ? 0 : 1;
		
				float edge = min(edgeDepth, edgeNormal);
				return edge;

			}

	ENDHLSL

	SubShader
    {
		ZWrite Off
		Lighting Off

		Pass
        {
			//Name "AdvancedRobertsCross"
            HLSLPROGRAM
            #pragma vertex vertedge
            #pragma fragment fragedge
            ENDHLSL
        }
    }
}
