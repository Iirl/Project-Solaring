using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;

namespace HandDrawnOutlines
{
    public class HandDrawnPass : ScriptableRenderPass
    {
        private readonly Material outlineMaterial;
        private readonly Material edgeDetecionMaterial;

        private RenderTargetIdentifier cameraColorTarget;

        private RenderTargetHandle temporaryBuffer;
        private RenderTargetHandle edgeBuffer;

#if !UNITY_2020_2_OR_NEWER // v8
        private ScriptableRenderer renderer;
#endif

        public HandDrawnPass(RenderPassEvent renderPassEvent, HandDrawnOutlinesSettings outlinesSettings, int number)
        {
            if (!outlinesSettings) return;

            this.renderPassEvent = renderPassEvent;

            outlinesSettings.EnableIdsKeywords(number);
            outlineMaterial = outlinesSettings.OutlineMaterial;
            edgeDetecionMaterial = outlinesSettings.EdgeDetectionMaterial;
            

            edgeBuffer.Init("_EdgeTex" + number.ToString());
        }

        public void Setup(ScriptableRenderer renderer)
        {
#if !UNITY_2020_2_OR_NEWER // v10+
            this.renderer = renderer;
#endif
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (!outlineMaterial || !edgeDetecionMaterial) return;

            // Set Source / Destination
#if UNITY_2020_2_OR_NEWER // v10+
				var renderer = renderingData.cameraData.renderer;
#else // v8
            // For older versions, cameraData.renderer is internal so can't be accessed. Will pass it through from AddRenderPasses instead
            var renderer = this.renderer;
#endif

            cameraColorTarget = renderer.cameraColorTarget;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(
                "_HandDrawnPass")))
            {
                RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
                cmd.GetTemporaryRT(temporaryBuffer.id, opaqueDescriptor, FilterMode.Bilinear);
                cmd.GetTemporaryRT(edgeBuffer.id, opaqueDescriptor, FilterMode.Bilinear);

                Blit(cmd, cameraColorTarget, edgeBuffer.id, edgeDetecionMaterial, 0);
                Blit(cmd, cameraColorTarget, temporaryBuffer.Identifier(), outlineMaterial, 0);
                Blit(cmd, temporaryBuffer.Identifier(), cameraColorTarget);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(temporaryBuffer.id);
            cmd.ReleaseTemporaryRT(edgeBuffer.id);
        }
    }

}
