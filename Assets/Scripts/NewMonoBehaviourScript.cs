using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public CompositeCollider2D compositeCollider2D;

    public ContactFilter2D contactFilter; // 필터 설정
    public List<Collider2D> colliders;
    // Update is called once per frame
    void Update()
    {
        int count =compositeCollider2D.Overlap(colliders);
        if (count == 0)
        {
            Debug.Log("발견된것이 업음");
        }
        if (count > 0)
        {
            Debug.Log("겹친 콜라이더의 수: " + count);
            for (int i = 0; i < count; i++)
            {
                Debug.Log("겹친 객체: " + colliders[i].name);
            }
        }
    }
}
