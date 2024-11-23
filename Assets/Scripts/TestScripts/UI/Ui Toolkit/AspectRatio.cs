using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 출처 : https://github.com/Unity-Technologies/ui-toolkit-manual-code-examples/blob/2023.2/create-aspect-ratios-custom-control/AspectRatio.cs
/// </summary>
public partial class AspectRatioElement : VisualElement
{
    public int RatioWidth
    {
        get => _ratioWidth;
        set
        {
            _ratioWidth = value;
            UpdateAspect();
        }
    }

    public int RatioHeight
    {
        get => _ratioHeight;
        set
        {
            _ratioHeight = value;
            UpdateAspect();
        }
    }
    // Padding elements to keep the aspect ratio.
    private int _ratioWidth = 16;
    private int _ratioHeight = 9;

    public AspectRatioElement()
    {
        // Update the padding elements when the geometry changes.
        RegisterCallback<GeometryChangedEvent>(UpdateAspectAfterEvent);
        // Update the padding elements when the element is attached to a panel.
        RegisterCallback<AttachToPanelEvent>(UpdateAspectAfterEvent);
    }

    public new class UxmlFactory : UxmlFactory<AspectRatioElement, UxmlTraits> {}
    public new class UxmlTraits : VisualElement.UxmlTraits
    {

        private UxmlIntAttributeDescription m_ratioWidth = new UxmlIntAttributeDescription { name = "Ratio-Width", defaultValue = 16 };
        private UxmlIntAttributeDescription m_ratioHeight = new UxmlIntAttributeDescription { name = "Ratio-Height", defaultValue = 9 };

        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        {
            get { yield break; }
        }

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            ((AspectRatioElement)ve).RatioWidth = m_ratioWidth.GetValueFromBag(bag, cc);
            ((AspectRatioElement)ve).RatioHeight = m_ratioHeight.GetValueFromBag(bag, cc);
        }
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
        var designRatio = (float)RatioWidth / RatioHeight;
        var currRatio = resolvedStyle.width / resolvedStyle.height;
        var diff = currRatio - designRatio;
        
        if (RatioWidth <= 0.0f || RatioHeight <= 0.0f)
        {
            ClearPadding();
            Debug.LogError($"[AspectRatio] Invalid width:{RatioWidth} or height:{RatioHeight}");
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