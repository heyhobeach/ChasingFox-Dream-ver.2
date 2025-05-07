using UnityEngine;

public class QTE_Select : MonoBehaviour, ISelectObject
{
    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock mpb;
    private bool canSelect;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        spriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_Hovered", 0);
        mpb.SetFloat("_Selected", 0);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    public void OnMouseEnter() => Select();
    public void OnMouseOver()
    {
        Deselect();
        Select();
    }
    public void OnMouseExit() => Deselect();

    public void Hover()
    {
        canSelect = true;
        mpb.SetFloat("_Hovered", 1);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    public void Leave()
    {
        canSelect = false;
        mpb.SetFloat("_Hovered", 0);
        mpb.SetFloat("_Selected", 0);
        spriteRenderer.SetPropertyBlock(mpb);        
    }
    public void Select()
    {
        if(!canSelect) return;
        mpb.SetFloat("_Selected", 1);
        spriteRenderer.SetPropertyBlock(mpb);
    }
    public void Deselect()
    {
        mpb.SetFloat("_Selected", 0);
        spriteRenderer.SetPropertyBlock(mpb);
    }
}
