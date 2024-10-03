using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QTE_Prerequisites : MonoBehaviour
{
    /// <summary>
    /// 조건 충족 여부를 반환하는 Property
    /// </summary>
    public abstract bool isSatisfied { get; set; }
}
