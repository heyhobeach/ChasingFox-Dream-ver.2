using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTE_LookTarget : QTE_Prerequisites
{
    public override bool isSatisfySatisfy { get => isLook; set => isLook = value; }

    private bool isLook;
    private Vector3 colSize;

    private void Start() => colSize = new Vector2(GetComponent<Collider2D>().bounds.extents.x * 0.75f, GetComponent<Collider2D>().bounds.extents.y * 0.75f);

    private void Update()
    {
        var temp = transform.position - MousePos();
        temp.x = Mathf.Abs(temp.x);
        temp.y = Mathf.Abs(temp.y);
        if(temp.x > colSize.x || temp.y > colSize.y) isLook = false;
        else isLook = true;
    }

    public Vector3 MousePos()//클릭한 좌료를 보내주며 현재 공격 클릭시 캐릭터의 바라보는 방향도 변해야한다고 생각해서 필요했던 부분
    {
        var screenPoint = Input.mousePosition;//마우스 위치 가져옴
        screenPoint.z = 0;
        return Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
