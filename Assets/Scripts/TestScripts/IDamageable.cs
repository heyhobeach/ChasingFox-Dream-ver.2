using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 데미지를 받을 수 있는 동작을 정의한 인터페이스
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// 대상의 무적 여부를 나타내는 속성
    /// true인 경우, 데미지를 받지 않음
    /// </summary>
    public bool invalidation { get; set; }

    /// <summary>
    /// 대상의 최대 체력을 나타내는 속성
    /// </summary>
    public int maxHealth { get; set; }

    /// <summary>
    /// 대상의 현재 체력을 나타내는 속성
    /// </summary>
    public int health { get; set; }

    /// <summary>
    /// 대상에게 지정된 데미지를 가함
    /// </summary>
    /// <param name="dmg">가할 데미지 양</param>
    /// <param name="action">데미지가 가해진 후 실행할 액션 (옵션)</param>
    /// <returns>데미지가 성공적으로 적용되었는지 여부를 반환</returns>
    bool GetDamage(int dmg, Action action = null)
    {
        if (health <= 0 || invalidation) return false; 
        if(action != null) action();
        health -= dmg;
        if (health <= 0) Death();
        return true;
    }

    /// <summary>
    /// 사망 동작을 수행
    /// </summary>
    void Death();
}
