using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 유닛의 상태를 정의하는 열거형
public enum UnitState
{
    Default,        // 기본
    Dash,           // 대시
    FormChange,     // 폼체인지
    HoldingWall,    // 벽잡기
    Air,            // 공중
    KnockBack,      // 넉백
    Stiffen,        // 경직
    Stiffen_er,     // 더 강한 경직
    Death,          // 사망
    Pause           // 일시정지
}


// 키 입력 상태를 정의하는 열거형
public enum KeyState
{
    // 키 입력 없음 | 키 다운 상태 | 키 누르고 있는 상태 | 키 업 상태
    None, KeyDown, KeyStay, KeyUp
}

/// 맵의 종류를 정의하는 열거형
public enum MapType
{
    None, Ground, Wall, Platform, Ceiling
}

/// <summary>
/// 유닛의 동작을 정의한 인터페이스
/// </summary>
public interface IUnitController
{
    /// <summary>
    /// 점프 동작을 수행
    /// </summary>
    /// <param name="jumpKey">점프 키 입력 상태</param>
    /// <returns>점프 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Jump(KeyState jumpKey);

    /// <summary>
    /// 이동 동작을 수행
    /// </summary>
    /// <param name="dir">이동 방향</param>
    /// <returns>이동 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Move(float dir);

    /// <summary>
    /// 크라우치 동작을 수행
    /// </summary>
    /// <param name="crouchKey">앉기 키 입력 상태</param>
    /// <returns>앉기 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Crouch(KeyState crouchKey);

    /// <summary>
    /// 공격 동작을 수행
    /// </summary>
    /// <param name="clickPos">클릭 위치</param>
    /// <returns>공격 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Attack(Vector3 clickPos);

    /// <summary>
    /// 대시 동작을 수행
    /// </summary>
    /// <returns>대시 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Dash();

    /// <summary>
    /// 폼체인지 동작을 수행
    /// </summary>
    /// <returns>폼체인지 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool FormChange();

    /// <summary>
    /// 재장전 동작을 수행
    /// </summary>
    /// <returns>재장전 동작이 성공적으로 수행되었을 시 true를 반환</returns>
    bool Reload();

    ///<summary>
    ///슬로우 모션
    /// </summary>
    //void SlowMotion();

    ///<summary>
    ///원래대로 복귀
    /// </summary>

   //void ResetMotion();
}
