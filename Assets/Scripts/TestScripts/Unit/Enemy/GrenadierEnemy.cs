using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadierEnemy : EnemyUnit, IDoorInteractable
{
    // TODO : RangedEnemy 코드 복붙함, 맞게 수정해야함
    public GameObject bullet;//�Ѿ� ����
    public float bulletSpeed;

    private bool _canInteract { get => unitState == UnitState.Default; }
    public bool canInteract { get => _canInteract; }

    protected override void Start()
    {
        base.Start();
        if(bullet == null) bullet = Resources.Load<GameObject>("Prefabs/Bullet");
    }

    public override bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-transform.position;
        // bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        var hit = Physics2D.Raycast(transform.position+Vector3.up, pos.normalized, pos.magnitude, 1<<LayerMask.NameToLayer("Map")|1<<LayerMask.NameToLayer("Wall"));
        if(hit) return false;
        else return true;
    }

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker()) return false;
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
