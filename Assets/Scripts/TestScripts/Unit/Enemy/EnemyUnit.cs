using System;
using Damageables;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase, IDamageable, ISelectObject
{
    public float attackDistance = 1;
    [Range(0, 1)] public float attackRange = 1;
    
    public bool isAttacking 
    {
        get
        {
            if(shootingAnimationController == null) return anim.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            else return shootingAnimationController.isAttackAni;
        }
    }

    [SerializeField] private int _maxHealth;
    public int maxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int health { get; set; }
    public bool invalidation { get; set; }

    private MaterialPropertyBlock mpb;
    private bool canSelect;

    protected override void OnEnable() {}
    protected override void Start()
    {
        base.Start();
        unitState = UnitState.Default;
        health = _maxHealth;
        
        mpb = new MaterialPropertyBlock();
    }

    public void OnMouseEnter() => Select();
    public void OnMouseOver()
    {
        Deselect();
        Select();
    }
    public void OnMouseExit() => Deselect();

    public override bool Move(Vector2 dir)
    {
        hzForce = (dir-(Vector2)transform.position).normalized.x;
        transform.position = Vector2.MoveTowards(transform.position, dir, Time.deltaTime * movementSpeed);
        return base.Move(Vector2.right * Mathf.Sign(hzForce));
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public virtual bool AttackCheck(Vector3 attackPos) => true;

    public void SetAni(bool b)
    {
        if(shootingAnimationController == null) return;
        switch(b)
        {
            case true: shootingAnimationController.AttackAni(); break;
            case false: shootingAnimationController.NomalAni(); break;
        }
    }

    void IDamageable.Death()
    {
        base.Death();
        invalidation = true;
        mpb.SetFloat("_Hovered", 0);
        mpb.SetFloat("_Selected", 0);
        spriteRenderer.SetPropertyBlock(mpb);
    }

    public void DeathFeedBack(Vector2 dir)
    {
        anim.applyRootMotion = false;
        var rg = GetComponent<Rigidbody2D>();
        rg.excludeLayers |= 1 << LayerMask.NameToLayer("Player");
        rg.constraints ^= RigidbodyConstraints2D.FreezePosition;
        rg.bodyType = RigidbodyType2D.Dynamic;
        var col = GetComponent<Collider2D>();
        col.isTrigger = false;
        rg.AddForceX(Mathf.Sign(((Vector2)transform.position - dir).normalized.x) * 4, ForceMode2D.Impulse);
    }

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
