//From https://github.com/candycat1992/NPR_Lab/blob/master/Assets/Shaders/PencilContour.shader

Shader "HandDrawn/PencilContour"
{

	HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#pragma multi_compile _Tex1 _Tex2 _Tex3 _Tex4 _Tex5

			#ifdef _Tex1
			#define _EdgeTex _EdgeTex1
			#define sampler_EdgeTex sampler_EdgeTex1

			#elif _Tex2
			#define _EdgeTex _EdgeTex2
			#define sampler_EdgeTex sampler_EdgeTex2

			#elif _Tex3
			#define _EdgeTex _EdgeTex3
			#define sampler_EdgeTex sampler_EdgeTex3

			#elif _Tex4
			#define _EdgeTex _EdgeTex4
			#define sampler_EdgeTex sampler_EdgeTex4

			#elif _Tex5
			#define _EdgeTex _EdgeTex5
			#define sampler_EdgeTex sampler_EdgeTex5
			#endif

			#pragma multi_compile _Contour1 _Contour2 _Contour3

			TEXTURE2D(_CameraColorTexture);
			SAMPLER(sampler_CameraColorTexture);

			TEXTURE2D (_EdgeTex);
			SAMPLER(sampler_EdgeTex);

			TEXTURE2D (_NoiseTex);
			SAMPLER(sampler_NoiseTex);

			TEXTURE2D (_NoiseAlpha);
			SAMPLER(sampler_NoiseAlpha);

		CBUFFER_START(UnityPerMaterial)
			//Contour
			float _ErrorPeriod;
			float _ErrorRange;
			float _NoiseAmount;
			float _EdgeOnly;
			float4 _Color;
			float _NoiseTilling;

			//Contour time
			float _TimeMultiplier;
			float _Synchronize;

			//Alpha
			float _UseNoiseAlpha;
			float _NoiseScaleAlpha;
			float _MultiplierAlpha;


			//Alpha time
			float _UseFlow;
			float _SpeedXAlpha;
			float _SpeedYAlpha;
			float _UseRandomization;
			float _TimeMultiplierAlpha;
			
		CBUFFER_END

			struct Attributes
			{
				float4 positionOS : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
		
			struct VaryingsSimple
			{
				float4 positionCS : SV_POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			VaryingsSimple VertexSimple(Attributes input)
			{
				VaryingsSimple output = (VaryingsSimple)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
				output.uv = input.uv;

				return output;
			}

			float4 FragContour(VaryingsSimple input) : SV_Target 
			{
				float offset = input.uv.x + input.uv.y;


				float edge;

				float2 uv[3];
				float constTime = floor((_Time.x*_TimeMultiplier)%2) *0.5;
				float time[3];

				float noise;
				if(constTime == 0)
					noise = (SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex, input.uv * _NoiseTilling).r - 0.5) * _NoiseAmount;
				else 
					noise = (SAMPLE_TEXTURE2D(_NoiseTex,sampler_NoiseTex, input.uv * _NoiseTilling + 10 * _NoiseTilling).r - 0.5) * _NoiseAmount;
				
				if(_Synchronize)
				{
					time[0] = constTime;
					time[1] = constTime + 1.12;
					time[2] = constTime + 3.12;
				}
				else
				{
					time[0] = floor((_Time.x*_TimeMultiplier + 0.443)%2) *0.5 ;
					time[1] = floor((_Time.x*_TimeMultiplier + 0.743)%2) *0.5  + 1.12;
					time[2] = floor((_Time.x*_TimeMultiplier + 0.122)%2) *0.5  + 3.12;
				}

				#ifdef _Contour1
				uv[0] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 0.0 + time[0]) + noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x + time[0]) + noise);
				edge = SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[0]).r;
				
				#elif _Contour2
				uv[0] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 0.0 + time[0]) + noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x + time[0]) + noise);
				uv[1] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 1.047 + time[1]) + noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x +3.142 + time[1]) - noise);
				edge = SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[0]).r * SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[1]).r;
				
				#elif _Contour3
				uv[0] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 0.0 + time[0]) + noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x + time[0]) + noise);
								uv[1] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 1.047 + time[1] ) + noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x +3.142 + time[1] ) - noise);
				uv[2] = input.uv + float2(_ErrorRange * sin(_ErrorPeriod * input.uv.y + 2.094 + time[2]) - noise, _ErrorRange * sin(_ErrorPeriod * input.uv.x + 1.571 + time[2]) - noise);
				edge = SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[0]).r * SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[1]).r * SAMPLE_TEXTURE2D(_EdgeTex,sampler_EdgeTex, uv[2]).r;
				
				#endif
			
				//Now edges are black and rest is white			
				//We are changing edges color to white and rest to black
				edge = -edge + 1;

				float alpha = 1;
				if(_UseNoiseAlpha)
				{
					float2 uva = input.uv * _NoiseScaleAlpha;
					if(_UseFlow)
					{
						uva -= float2(_Time.x * _SpeedXAlpha, _Time.x * _SpeedYAlpha);
					}

					if(_UseRandomization)
					{
						float time = floor((_Time.x*_TimeMultiplierAlpha)%2) *0.5;
						if(time) uva += float2(-2*_NoiseScaleAlpha,2*_NoiseScaleAlpha);
					}

					float alphaNoise = SAMPLE_TEXTURE2D(_NoiseAlpha,sampler_NoiseAlpha,uva ).r * _MultiplierAlpha;
					alpha = alphaNoise;
				}

				edge *= alpha;
				float3 withEdgeColor = lerp (SAMPLE_TEXTURE2D(_CameraColorTexture,sampler_CameraColorTexture, input.uv),_Color, edge*_Color.a).rgb;

				return float4(withEdgeColor, 1);
			}

	ENDHLSL

	SubShader
    {
		Blend SrcAlpha OneMinusSrcAlpha
		Pass 
		{ 
			Name "PencilContour"
            HLSLPROGRAM
			#pragma vertex VertexSimple
			#pragma fragment FragContour
			ENDHLSL
		}
    }
}
