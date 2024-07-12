using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;
    public static CameraManager Instance { get => instance; }

    public ProCamera2DShake proCamera2DShake;
    public ProCamera2DPointerInfluence proCamera2DPointerInfluence;

    public float ChangeSize { get; set; }

    void Awake()
    {
        if(instance == null) instance = this;
        proCamera2DShake = Camera.main.GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = Camera.main.GetComponent<ProCamera2DPointerInfluence>();
    }

    void Update()
    {
        if(!ChangeSize.Equals(Camera.main.orthographicSize))
        {
            Camera.main.orthographicSize = Mathf.Ceil(Mathf.Lerp(Camera.main.orthographicSize, ChangeSize, 5f * Time.deltaTime) * 10000) * 0.0001f;
            // Camera.main.orthographicSize += Mathf.Ceil((ChangeSize - Camera.main.orthographicSize) * Time.deltaTime * 1000) * 0.001f;
            // Camera.main.orthographicSize = Mathf.Floor(Camera.main.orthographicSize * 100) * 0.01f;
        }
    }
}
