using System;

namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// A volume component that holds settings for the color lookup effect.
    /// </summary>
    [Serializable, VolumeComponentMenu("Lighting/Global Light Override")]
    [SupportedOnRenderPipeline(typeof(UniversalRenderPipelineAsset))]
    // [URPHelpURL("integration-with-post-processing")]
    public sealed class GlobalLightOverride : VolumeComponent, IPostProcessComponent
    {
        public ColorParameter color = new ColorParameter(Color.white, true);

        public bool IsActive() => color.overrideState;

        public bool IsTileCompatible() => true;

    }
}
