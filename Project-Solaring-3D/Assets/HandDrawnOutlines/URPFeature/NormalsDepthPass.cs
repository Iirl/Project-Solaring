using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace HandDrawnOutlines
{
    public class NormalsDepthPass : ScriptableRenderPass
    {
        private readonly RenderTargetHandle target;

        private readonly Material material;
        private readonly List<ShaderTagId> shaderTagIdList;

        private FilteringSettings filteringSettings;
        private RenderStateBlock m_RenderStateBlock;

        public NormalsDepthPass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, bool useDepthMask,int number)
        {
            material = new Material(Shader.Find("HandDrawn/NormalsDepth"));
            material.SetFloat("_UseDepthMask", useDepthMask ? 1 : 0);
            material.EnableKeyword("_Tex" + number.ToString());

            target.Init("_DepthNormalsTex" + number.ToString());


            shaderTagIdList = new List<ShaderTagId>()
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefoultUnlit"),
                new ShaderTagId("DepthOnly")
            };

            //todo: Do we need that
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_RenderStateBlock.mask |= RenderStateMask.Depth;
            m_RenderStateBlock.depthState = new DepthState(true, CompareFunction.LessEqual);

            this.renderPassEvent = renderPassEvent;
            filteringSettings = new FilteringSettings(RenderQueueRange.all, outlinesLayerMask);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            normalsTextureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(target.id, normalsTextureDescriptor, FilterMode.Bilinear);
            ConfigureTarget(target.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!material)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "_NormalsDepth")))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = material;
                drawingSettings.enableDynamicBatching = true;

                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref m_RenderStateBlock);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(target.id);
        }
    }
}