using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase
{
    public float maxDistance = 1.06f;

    public GameObject bullet;//�Ѿ� ����
    

    protected override void OnEnable() {}

    public override bool Move(Vector2 dir)
    {
        hzForce = (dir-(Vector2)transform.position).normalized.x;
        transform.position = Vector2.MoveTowards(transform.position, dir, Time.deltaTime * movementSpeed);
        return base.Move(Vector2.right * Mathf.Sign(hzForce));
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
