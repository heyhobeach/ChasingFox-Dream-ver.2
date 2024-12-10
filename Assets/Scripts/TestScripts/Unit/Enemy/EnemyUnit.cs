using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Damageables;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase, IDamageable
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

    protected override void OnEnable() {}
    protected override void Start()
    {
        base.Start();
        unitState = UnitState.Default;
        health = _maxHealth;
    }

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

    public override void StopAllC()
    {
        invalidation = true;
    }

    public void DeathFeedBack(Vector2 dir)
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //rb.linearVelocityX = 2000 * Mathf.Sign(dir.x);
        //hzForce= 2000 * Mathf.Sign(dir.x);
        rb.AddForceX(1000    * Mathf.Sign(dir.x), ForceMode2D.Force);
        Debug.Log("벨로시티" + rb.linearVelocityX);
    }
}
