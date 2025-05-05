using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour
{
    public struct State
    {
        public float maxHorizontalInfluence;
        public float maxVerticalInfluence;
        public float influenceSmoothness;
        public float changeSize;
    }

    public ProCamera2DShake proCamera2DShake;
    public ProCamera2DPointerInfluence proCamera2DPointerInfluence;
    public ProCamera2DRooms proCamera2DRooms;

    public float ChangeSize { set => ProCamera2D.Instance.UpdateScreenSize(value, 0.5f); }

    public void SetState(State state)
    {
        proCamera2DPointerInfluence.MaxHorizontalInfluence = state.maxHorizontalInfluence;
        proCamera2DPointerInfluence.MaxVerticalInfluence = state.maxVerticalInfluence;
        proCamera2DPointerInfluence.InfluenceSmoothness = state.influenceSmoothness;
        ChangeSize = state.changeSize;
    }

    public void AddTarget(Transform transform) => ProCamera2D.Instance.AddCameraTarget(transform, 1, 1, 1);
    public void RemoveTarget(Transform transform) => ProCamera2D.Instance.RemoveCameraTarget(transform, 1);

    private void OnDestroy()
    {
        ServiceLocator.Unregister(this);
    }
    void Awake()
    {
        ServiceLocator.Register(this);
        proCamera2DShake = GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = GetComponent<ProCamera2DPointerInfluence>();
        proCamera2DRooms = GetComponent<ProCamera2DRooms>();
    }
}
