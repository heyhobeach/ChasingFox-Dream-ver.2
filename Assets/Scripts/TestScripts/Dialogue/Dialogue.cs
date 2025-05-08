using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    [Tooltip("캐릭터 이름")]
    public string name;

    [Tooltip("대사 내용")]
    public string[] context;
    public string choice;
    public string id;
    public string[] command;
    public string image_name;
    public string dir;
    public string[] problem;
    public string[] correct_answer;
    public string[] hidden_answer;
    public string[] mode;
}
[System.Serializable]
public class DialogueEvent
{
    public string name;
    public Vector2 line;//대사 추출을 위해?
    public Dialogue[] dialouses;//여러명이 말 하는것이기 대문에
    
}
