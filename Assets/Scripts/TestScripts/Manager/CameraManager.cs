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

    public float ChangeSize { set => ProCamera2D.Instance.UpdateScreenSize(value, 0.5f); }
    // public float CameraOffsetY { set => cameraOffsetY = value; }

    void Awake()
    {
        if(instance == null) instance = this;
        proCamera2DShake = Camera.main.GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = Camera.main.GetComponent<ProCamera2DPointerInfluence>();
    }

    void Update()
    {
        // ProCamera2D.Instance.OffsetY = cameraOffsetY;
        // if(!ChangeSize.Equals(Camera.main.orthographicSize))
        // {
        //     Camera.main.orthographicSize = Mathf.Ceil(Mathf.Lerp(Camera.main.orthographicSize, ChangeSize, 5f * Time.deltaTime) * 10000) * 0.0001f;
        // }
    }
}
