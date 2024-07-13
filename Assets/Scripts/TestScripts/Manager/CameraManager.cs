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

    void Awake()
    {
        if(instance == null) instance = this;
        proCamera2DShake = Camera.main.GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = Camera.main.GetComponent<ProCamera2DPointerInfluence>();
    }
}
