using UnityEngine;
using UnityEngine.VFX;

public class RainContoroller : MonoBehaviour
{
    public VisualEffect targetVFX;

    [Range(0f, 100f)]
    public float strength = 100f;
    [Range(-50f, 50f)]
    public float windForce = 10f;
    [Range(1f, 50f)]
    public float range = 2.5f;
    [Range(1f, 50f)]
    public float height = 5f;

    void Start()
    {
        if (targetVFX == null) targetVFX = GetComponent<VisualEffect>();
        UpdateVFXProperties();
    }

    [ContextMenu("Update Properties")]
    void UpdateVFXProperties()
    {
        if(strength == 0) targetVFX.Stop();
        else targetVFX.Play();
        targetVFX.SetFloat("strength", strength);
        targetVFX.SetFloat("windForce", windForce);
        targetVFX.SetFloat("range", range);
        targetVFX.SetFloat("height", height);
    }
    public void UpdateVFXProperties(float strength, float windForce, float range, float height)
    {
        this.strength = strength;
        this.windForce = windForce;
        this.range = range;
        this.height = height;
        UpdateVFXProperties();
    }

    private void OnValidate()
    {
        if (targetVFX == null) targetVFX = GetComponent<VisualEffect>();
        if (targetVFX != null) UpdateVFXProperties();
    }
}