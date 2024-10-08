using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class StopScript : MonoBehaviour//monoBehaviour뺄수 있지 않을까
{
    // Start is called before the first frame update

    public PlayableDirector testp;

    public void Update()
    {
        Debug.Log("타임라인 시간"+testp.time);
    }
}
