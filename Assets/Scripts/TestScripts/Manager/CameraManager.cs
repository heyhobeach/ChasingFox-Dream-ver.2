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
    public ProCamera2DRooms proCamera2DRooms;

    public float ChangeSize { set => ProCamera2D.Instance.UpdateScreenSize(value, 0.5f); }

    public void AddTarget(Transform transform) => ProCamera2D.Instance.AddCameraTarget(transform, 1, 1, 1);
    public void RemoveTarget(Transform transform) => ProCamera2D.Instance.RemoveCameraTarget(transform, 1);

    void Awake()
    {
        if(instance == null) instance = this;
        proCamera2DShake = GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = GetComponent<ProCamera2DPointerInfluence>();
        proCamera2DRooms = GetComponent<ProCamera2DRooms>();
    }
}
