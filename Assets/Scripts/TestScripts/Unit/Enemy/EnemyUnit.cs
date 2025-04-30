using System;
using System.Collections;
using Com.LuisPedroFonseca.ProCamera2D;
using Damageables;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase, IDamageable, ISelectObject
{
    
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

    protected override void Start()
    {
        base.Start();
        UnitState = UnitState.Default;
        health = _maxHealth;
        
        rg = GetComponent<Rigidbody2D>();
        mpb = new MaterialPropertyBlock();
    }
    protected void LateUpdate()
    { 
        mpb.SetTexture("_MainTex", spriteRenderer.sprite.texture);
        spriteRenderer.SetPropertyBlock(mpb);
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
        if(ControllerChecker()) 
        {
            hzForce = 0;
            base.Move(Vector2.zero);
            return false;
        }
        hzForce = (dir-(Vector2)transform.position).normalized.x;
        transform.position = Vector2.MoveTowards(transform.position, dir, Time.deltaTime * movementSpeed);
        return base.Move(Vector2.right * Mathf.Sign(hzForce));
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public virtual bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-transform.position;
        bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        var hit = Physics2D.Raycast(transform.position+Vector3.up, pos.normalized, pos.magnitude, 1<<LayerMask.NameToLayer("Map")|1<<LayerMask.NameToLayer("Wall"));
        if((hit && !hit.collider.isTrigger) || isForword) return false;
        else return true;
    }

    void IDamageable.Death()
    {
        base.Death();
        invalidation = true;
        Leave();
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
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        var rg = GetComponent<Rigidbody2D>();
        rg.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
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