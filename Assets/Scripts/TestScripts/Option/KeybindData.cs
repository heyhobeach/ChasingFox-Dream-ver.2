using System;
using System.Collections;
using UnityEngine;

public class KeybindData : ICloneable, IEnumerable
{
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode jump;
    public KeyCode crouch;
    public KeyCode attack;
    public KeyCode reload;
    public KeyCode dash;
    public KeyCode formChange;
    public KeyCode skill1;
    public KeyCode retry;

    public object Clone()
    {
        return new KeybindData()
        {
            moveLeft = this.moveLeft,
            moveRight = this.moveRight,
            jump = this.jump,
            crouch = this.crouch,
            attack = this.attack,
            reload = this.reload,
            dash = this.dash,
            formChange = this.formChange,
            skill1 = this.skill1,
            retry = this.retry
        };
    }

    public IEnumerator GetEnumerator()
    {
        yield return moveLeft;
        yield return moveRight;
        yield return jump;
        yield return crouch;
        yield return attack;
        yield return reload;
        yield return dash;
        yield return formChange;
        yield return skill1;
        yield return retry;
    }
}
