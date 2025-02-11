using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 출처 : https://github.com/Unity-Technologies/ui-toolkit-manual-code-examples/blob/2023.2/create-aspect-ratios-custom-control/AspectRatio.cs
/// </summary>
[UxmlElement("AspectRatioElement")]
public partial class AspectRatioElement : VisualElement
{
    // Padding elements to keep the aspect ratio.
    public int ratioWidth = 16;
    public int ratioHeight = 9;

    public AspectRatioElement()
    {
        // Update the padding elements when the geometry changes.
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        // Update the padding elements when the element is attached to a panel.
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }

    static void UpdateAspectAfterEvent(EventBase evt)
    {
        var element = evt.target as AspectRatioElement;
        element?.UpdateAspect();
    }

    private void ClearPadding()
    {
        style.paddingLeft = 0;
        style.paddingRight = 0;
        style.paddingBottom = 0;
        style.paddingTop = 0;
    }
    
    // Update the padding.
    private void UpdateAspect()
    {
        var designRatio = (float)ratioWidth / ratioHeight;
        var currRatio = resolvedStyle.width / resolvedStyle.height;
        var diff = currRatio - designRatio;
        
        if (ratioWidth <= 0.0f || ratioHeight <= 0.0f)
        {
            ClearPadding();
            Debug.LogError($"[AspectRatio] Invalid width:{ratioWidth} or height:{ratioHeight}");
            return;
        }

        if (float.IsNaN(resolvedStyle.width) || float.IsNaN(resolvedStyle.height))
        {
            return;
        }
        
        if (diff > 0.01f)
        {
            var w = (resolvedStyle.width - (resolvedStyle.height * designRatio)) * 0.5f;
            style.paddingLeft = w;
            style.paddingRight = w;
            style.paddingTop = 0;
            style.paddingBottom = 0;
        }
        else if (diff < -0.01f)
        {
            var h = (resolvedStyle.height - (resolvedStyle.width * (1/designRatio))) * 0.5f;
            style.paddingLeft= 0;
            style.paddingRight = 0;
            style.paddingTop = h;
            style.paddingBottom = h;
        }
        else
        {
            ClearPadding();
        }
    }
}