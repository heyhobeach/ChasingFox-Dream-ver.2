using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase
{
    public float maxDistance = 1.06f;
    
    protected float hzVel;

    public GameObject bullet;//�Ѿ� ����
    

    protected override void OnEnable() {}

    public override bool Move(Vector2 dir)
    {
        hzVel += dir.x == 0 ? -hzVel * accelerate * Time.deltaTime : (dir.x-hzForce/movementSpeed) * accelerate * Time.deltaTime; // 가속도만큼 입력 방향에 힘을 추가
        if(dir.x == 0 && Mathf.Abs(hzVel) < 0.01f) hzVel = 0;
        transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime);
        return base.Move(dir);
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker() || Physics2D.Raycast(transform.position, attackPos-transform.position, Mathf.Infinity, 1<<LayerMask.NameToLayer("Ground"))) return false;
        GameObject _bullet = Instantiate(bullet, transform.position, transform.rotation);

        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, attackPos, Vector3.zero, 1, 1, gObj, (Vector2)(attackPos-transform.position).normalized);
        return true;
    }
}
