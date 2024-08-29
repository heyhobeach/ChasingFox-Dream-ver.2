using System;
using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase
{
    public float attackDistance = 1;
    [Range(0, 1)] public float attackRange = 1;

    public GameObject bullet;//�Ѿ� ����
    
    public bool isAttacking 
    {
        get
        {
            if(shootingAnimationController == null) return anim.GetCurrentAnimatorStateInfo(0).IsName("Attack");
            else return shootingAnimationController.isAttackAni;
        }
    }

    protected override void OnEnable() {}

    public override bool Move(Vector2 dir)
    {
        hzForce = (dir-(Vector2)transform.position).normalized.x;
        transform.position = Vector2.MoveTowards(transform.position, dir, Time.deltaTime * movementSpeed);
        return base.Move(Vector2.right * Mathf.Sign(hzForce));
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-transform.position;
        float deg = Mathf.Atan2(pos.y, pos.x);//mathf.de
        deg *= Mathf.Rad2Deg;
        bool inAngle = Mathf.Abs(deg) <= 180;
        bool inRange = (pos.magnitude < attackDistance) && pos.magnitude >= attackDistance*(1-attackRange);
        bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        // bool isInner = Math.Abs(attackPos.x - transform.position.x) < boxSizeX;
        bool isInner = pos.magnitude < boxSizeX;
        var hit = Physics2D.Raycast(transform.position, pos, pos.magnitude, 1<<LayerMask.NameToLayer("Map"));
        if(ControllerChecker() || hit || !inRange || !inAngle || !isForword || isInner) return false;
        else return true;
    }

    public override bool Attack(Vector3 attackPos)
    {
        if(shootingAnimationController != null) shootingAnimationController.targetPosition = attackPos;

        GameObject _bullet = Instantiate(bullet, transform.position, transform.rotation);

        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, attackPos, Vector3.zero, 1, 1, gObj, (Vector2)(attackPos-transform.position).normalized);
        return true;
    }

    public void ShootingAni(bool b)
    {
        switch(b)
        {
            case true:
            shootingAnimationController.AttackAni();
            break;
            case false:
            shootingAnimationController.NomalAni();
            break;
        }
    }
}
