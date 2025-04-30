using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class K9Enemy : EnemyUnit
{
    public GameObject MeleeAttack;//�Ѿ� ����
    private SpriteRenderer effectRenderer;

    protected override void Start()
    {
        base.Start();
        MeleeAttack.SetActive(true);
        MeleeAttack.SetActive(false);
        MeleeAttack.GetComponent<MaleeAttack>().Set(1, gameObject);
        effectRenderer = MeleeAttack.transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public override bool Attack(Vector3 attackPos)
    {
        if(ControllerChecker()) return false;
        Vector2 subvec = attackPos - (transform.position+Vector3.up);
        MeleeAttack.transform.position = attackPos;
        if(subvec.x < 0) effectRenderer.flipX = true;
        else effectRenderer.flipX = false;

        return base.Attack(attackPos);
    }
}
