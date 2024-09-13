using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemy : EnemyUnit
{
    public GameObject MeleeAttack;//�Ѿ� ����
    private SpriteRenderer effectRenderer;

    protected override void Start()
    {
        base.Start();
        MeleeAttack.SetActive(true);
        attackDistance = MeleeAttack.GetComponent<BoxCollider2D>().bounds.extents.x + MeleeAttack.transform.localPosition.x;
        MeleeAttack.SetActive(false);
        MeleeAttack.GetComponent<MaleeAttack>().Set(1, gameObject);
        effectRenderer = MeleeAttack.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override bool AttackCheck(Vector3 attackPos)
    {
        var pos = attackPos-transform.position;
        bool inRange = (pos.magnitude < attackDistance) && (pos.magnitude >= attackDistance*(1-attackRange));
        bool isForword = Mathf.Sign(pos.normalized.x)>0&&!spriteRenderer.flipX ? true : Mathf.Sign(pos.normalized.x)<0&&spriteRenderer.flipX ? true : false;
        var hit = Physics2D.Raycast(transform.position, pos, pos.magnitude, 1<<LayerMask.NameToLayer("Map"));
        if(ControllerChecker() || hit || !inRange || !isForword) return false;
        else return true;
    }

    public override bool Attack(Vector3 attackPos)
    {
        Vector2 subvec = attackPos - transform.position;
        float deg = Mathf.Atan2(subvec.y, subvec.x) ;//mathf.de
        //deg*=Mathf.Deg2Rad;//라디안으로 바꿔주기는 하는데 이렇게 하면 좀 문제생김
        //Debug.Log(deg);
        MeleeAttack.transform.localPosition = new Vector3(Mathf.Cos(deg), Mathf.Sin(deg)*2,transform.localPosition.z);
        if(subvec.x < 0) effectRenderer.flipX = true;
        else effectRenderer.flipX = false;
        // MeleeAttack.transform.localEulerAngles = new Vector3(0, 0, Quaternion.FromToRotation(Vector2.up, transform.position - MeleeAttack.transform.position).eulerAngles.z - 90);

        return base.Attack(attackPos);
    }
}
