Shader "HandDrawn/NormalsDepth"
{
     HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        #pragma multi_compile _Tex1 _Tex2 _Tex3 _Tex4 _Tex5

			#ifdef _Tex1
			#define _SceneMask _SceneMask1
			#define sampler_SceneMask sampler_SceneMask1

			#elif _Tex2
			#define _SceneMask _SceneMask2
			#define sampler_SceneMask sampler_SceneMask2

			#elif _Tex3
			#define _SceneMask _SceneMask3
			#define sampler_SceneMask sampler_SceneMask3

			#elif _Tex4
			#define _SceneMask _SceneMask4
			#define sampler_SceneMask sampler_SceneMask4

			#elif _Tex5
			#define _SceneMask _SceneMask5
			#define sampler_SceneMask sampler_SceneMask5

			#endif

        TEXTURE2D (_SceneMask);
		SAMPLER (sampler_SceneMask);

        float _UseDepthMask;

        struct Attributes
        {
            float4 position     : POSITION;
            float2 texcoord     : TEXCOORD0;
            float3 normal       : NORMAL;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct Varyings
        {
            float2 uv           : TEXCOORD0;
            float4 positionCS   : SV_POSITION;
            float3 viewNormal   : NORMAL;
            float4 screenPos    : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        Varyings vert(Attributes input)
        {
            Varyings output = (Varyings)0;
            UNITY_SETUP_INSTANCE_ID(input);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

            VertexPositionInputs positionInputs = GetVertexPositionInputs(input.position.xyz);

            output.uv = input.texcoord;
            output.positionCS = positionInputs.positionCS;
            output.viewNormal = input.normal;
            output.screenPos = positionInputs.positionNDC;
            return output;
        }

        float4 frag(Varyings input) : SV_TARGET
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

            if(_UseDepthMask)
            {
                float4 mask = SAMPLE_TEXTURE2D(_SceneMask, sampler_SceneMask, input.screenPos.xy / input.screenPos.w);

                if (mask.r > input.positionCS.z)
		        {
		    	    return float4(0,0,0,1);
		        }
            }

            //return 1;
            return float4(normalize(input.viewNormal) , input.positionCS.z);
         }

    ENDHLSL

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline"  }

        Pass
        {
            Name "NormalsDepth"

            ZWrite Off
            Lighting Off

            HLSLPROGRAM
	        #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag
            ENDHLSL
        }
    }
}
