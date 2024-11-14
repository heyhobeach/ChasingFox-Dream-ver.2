using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharactorImage", menuName = "ScriptableObjectDatas/CharactorImage Data", order = int.MaxValue)]
public class ScriptorbleObjectTest : ScriptableObject
{

    [SerializeField]
    public string char_name;
    [SerializeField]
    public Sprite sprite;

}
