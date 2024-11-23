using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 출처 : https://discussions.unity.com/t/ui-toolkit-text-best-fit/248895/5
/// </summary>
public class LabelAutoFit : Label
{
    [UnityEngine.Scripting.Preserve]
    public new class UxmlFactory : UxmlFactory<LabelAutoFit, UxmlTraits> { }

    [UnityEngine.Scripting.Preserve]
    public new class UxmlTraits : Label.UxmlTraits
    {
        public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription { get { yield break; } }

        public override void Init(VisualElement visualElement, IUxmlAttributes attributes, CreationContext creationContext)
        {
            base.Init(visualElement, attributes, creationContext);
        }
    }

    public LabelAutoFit()
    {
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent e)
    {
        UpdateFontSize();
    }

    private void UpdateFontSize()
    {
        // Set width to auto temporarily to get the actual width of the label
        var currentFontSize = MeasureTextSize(text, 0, MeasureMode.Undefined, 0, MeasureMode.Undefined);

        var multiplier = resolvedStyle.width / (currentFontSize.x * 1.25f);
        var newFontSize = Mathf.RoundToInt(Mathf.Clamp(multiplier * currentFontSize.y, 1, resolvedStyle.height));

        if (Mathf.RoundToInt(currentFontSize.y) != newFontSize)
            style.fontSize = new StyleLength(new Length(newFontSize));
    }
}