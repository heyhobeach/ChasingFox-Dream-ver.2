using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class EnemyUnit : UnitBase
{
    public float maxDistance = 1.06f;

    public GameObject bullet;//�Ѿ� ����
    // public GameObject[] bullets;//���� ����� �� �ϸ� Ȥ�ó� �ʿ��ұ� ����� �� �κ� ����� �� �ϴ��� 

    protected override void OnEnable() { GameObject.FindGameObjectWithTag("Player"); unitState = UnitState.Default; }

    public override bool Move(float dir)
    {
        return base.Move(dir);
    }

    public override bool Crouch(KeyState crouchKey) => false;

    public override bool Jump(KeyState jumpKey) => false;

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker()) return false;
        GameObject _bullet = Instantiate(bullet, transform.position, transform.rotation);

        GameObject gObj = this.gameObject;
        _bullet.GetComponent<Bullet>().Set(transform.position, attackPos, Vector3.zero, 1, 1, gObj, (Vector2)(attackPos-transform.position).normalized);
        return true;
    }
}
