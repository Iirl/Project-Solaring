using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace HandDrawnOutlines
{
    public class HandDrawnOutlinesFeature : ScriptableRendererFeature
    {
        private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

        [SerializeField,Range(1,5)] private int ID = 1;
        [SerializeField] private LayerMask layerMask = ~0;
        [SerializeField] private bool UseDepthMask = true;

        [SerializeField] public HandDrawnOutlinesSettings handDrawnOutlinesSettings;

        private NormalsDepthPass normalsDepthPass;
        private MaskPass maskPass;
        private HandDrawnPass HandDrawnPass;

        public override void Create()
        {
            if (!handDrawnOutlinesSettings) return;

            if (UseDepthMask) maskPass = new MaskPass(renderPassEvent, layerMask, ID);
            normalsDepthPass = new NormalsDepthPass(renderPassEvent, layerMask, UseDepthMask, ID);
            HandDrawnPass = new HandDrawnPass(renderPassEvent, handDrawnOutlinesSettings, ID);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (!handDrawnOutlinesSettings) return;
            if (UseDepthMask)
            {
                //maskPass.Setup();
                renderer.EnqueuePass(maskPass);
            }
            renderer.EnqueuePass(normalsDepthPass);
            HandDrawnPass.Setup(renderer);
            renderer.EnqueuePass(HandDrawnPass);
        }

        
    }
}