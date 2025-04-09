using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class GlobalLightOverrideRenderGraph : ScriptableRendererFeature
{
    class GlobalLightOverridePass : ScriptableRenderPass
    {
        private GlobalLightOverride volumeComponent;

        public GlobalLightOverridePass()
        {
            profilingSampler = new ProfilingSampler("GlobalLightOverride Pass");
        }

        internal class PassData {}

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            volumeComponent = stack.GetComponent<GlobalLightOverride>();

            if (volumeComponent == null || !volumeComponent.IsActive())
                return;

            using (var builder = renderGraph.AddUnsafePass<PassData>("GlobalLightOverride Pass", out var passData, profilingSampler))
            {
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);
                
                builder.SetRenderFunc((PassData data, UnsafeGraphContext context) => 
                {
                    using (new ProfilingScope(context.cmd, profilingSampler))
                    {
                        context.cmd.ClearRenderTarget(true, true, volumeComponent.color.value);
                    }
                });
            }
        }
    }

    [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
    private GlobalLightOverridePass customPass;

    public override void Create()
    {
        customPass = new GlobalLightOverridePass();
        customPass.renderPassEvent = renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (customPass == null)
            return;

        renderer.EnqueuePass(customPass);
    }
}
