using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedEnemy : EnemyUnit, IDoorInteractable
{
    public GameObject bullet;//�Ѿ� ����
    public float bulletSpeed;

    private bool _canInteract { get => UnitState == UnitState.Default; }
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
        var hits = Physics2D.RaycastAll(shootingAnimationController.GetShootPosition(), pos.normalized, pos.magnitude, 1<<LayerMask.NameToLayer("Map")|1<<LayerMask.NameToLayer("Wall")).Where(x => !x.collider.isTrigger);
        if(hits.Count() == 0) return true;
        else return false;
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
            1, 
            bulletSpeed, 
            gObj
        );
        

        return base.Attack(attackPos);
    }
}
