using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private Animator animator;
    void Start() => animator = GetComponent<Animator>();

    void Update()
    {
        if(animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f) Destroy(gameObject);
    }

}
