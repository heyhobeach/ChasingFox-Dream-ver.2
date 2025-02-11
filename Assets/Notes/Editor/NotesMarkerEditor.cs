using System;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Timeline;

// Editor used by the Timeline window to customize the appearance of a NotesMarker

[CustomTimelineEditor(typeof(NotesMarker))]
public class NotesMarkerEditor : MarkerEditor
{
    // Set a constant for the transparency of overlays
    const float k_OverlayAlpha = 0.5f;

    // Override this method to draw a vertical line over the Timeline window's contents.
    public override void DrawOverlay(IMarker marker, MarkerUIStates uiState, MarkerOverlayRegion region)
    {
        // Check if marker is not NotesMarker. Set notes as local variable.
        if (marker is not NotesMarker notes)
        {
            return; // If not, return without drawing an overlay
        }

        // If NotesMarker, check if Show Line Overlay property is true
        if (notes.showLineOverlay)
        {
            DrawLineOverlay(notes.color, region); // if Show Line Overlay is true, call function to draw vertical line
        }
    }

    static void DrawLineOverlay(Color color, MarkerOverlayRegion region)
    {
        // Calculate a rectangle that uses the full timeline region's height and marker width
        Rect overlayLineRect = new Rect(region.markerRegion.x,
            region.timelineRegion.y,
            region.markerRegion.width,
            region.timelineRegion.height);

        // Set the color with an extra alpha value adjustment, then draw the rectangle
        Color overlayLineColor = new Color(color.r, color.g, color.b, color.a * k_OverlayAlpha);
        EditorGUI.DrawRect(overlayLineRect, overlayLineColor);
    }
}
