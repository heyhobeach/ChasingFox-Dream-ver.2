using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PixelCametaTest : MonoBehaviour
{
    [Range(1, 100)] public int pixelate;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        source.filterMode = FilterMode.Point;
        RenderTexture rt = RenderTexture.GetTemporary(source.width / pixelate, source.height / pixelate, 0, source.format);
        rt.filterMode = FilterMode.Point;
        Graphics.Blit(source, rt);
        Graphics.Blit(rt, destination);
        RenderTexture.ReleaseTemporary(rt);
    }
}
