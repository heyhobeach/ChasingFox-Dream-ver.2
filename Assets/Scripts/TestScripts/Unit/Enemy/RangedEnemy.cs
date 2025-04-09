using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : EnemyUnit
{
    public GameObject bullet;//�Ѿ� ����
    public float bulletSpeed;

    protected override void Start()
    {
        base.Start();
        if(bullet == null) bullet = Resources.Load<GameObject>("Prefabs/Bullet");
    }

    public override bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-(transform.position+Vector3.up);
        bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        bool isInner = pos.magnitude < boxSizeX*2;
        var hit = Physics2D.CircleCast(transform.position+Vector3.up, 0.25f, pos, pos.magnitude, 1<<LayerMask.NameToLayer("Map"));
        if(ControllerChecker() || hit || !isForword || isInner) return false;
        else return true;
    }

    public override bool Attack(Vector3 attackPos)
    {
        GameObject _bullet = Instantiate(bullet);
        GameObject gObj = this.gameObject;
        shootingAnimationController.targetPosition = attackPos;
        _bullet.GetComponent<Bullet>().Set(
            shootingAnimationController.GetShootPosition(), 
            attackPos, 
            shootingAnimationController.GetShootRotation(), 
            1, 
            bulletSpeed, 
            gObj
        );
        

        return base.Attack(attackPos);
    }
}
