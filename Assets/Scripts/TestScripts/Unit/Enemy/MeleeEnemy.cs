using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit, IDoorInteractable
{
    public GameObject MeleeAttack;//�Ѿ� ����
    private SpriteRenderer effectRenderer;
    private bool _canInteract { get => UnitState == UnitState.Default; }
    public bool canInteract { get => _canInteract; }

    protected override void Start()
    {
        base.Start();
        MeleeAttack.SetActive(true);
        MeleeAttack.SetActive(false);
        MeleeAttack.GetComponent<MaleeAttack>().Set(1, gameObject);
        effectRenderer = MeleeAttack.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-transform.position;
        bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        var hit = Physics2D.Raycast(transform.position+Vector3.up, pos.normalized, pos.magnitude, 1<<LayerMask.NameToLayer("Map")|1<<LayerMask.NameToLayer("Wall"));
        if(hit || !isForword) return false;
        else return true;
    }

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker()) return false;
        Vector2 subvec = (attackPos - (transform.position+Vector3.up)).normalized;
        MeleeAttack.transform.position = transform.position + (Vector3)subvec;
        if(subvec.x < 0) effectRenderer.flipX = true;
        else effectRenderer.flipX = false;

        return base.Attack(attackPos);
    }
}
