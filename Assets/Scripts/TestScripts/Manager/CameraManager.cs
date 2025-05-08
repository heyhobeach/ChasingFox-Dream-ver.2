using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.Playables;

public class CameraManager : MonoBehaviour, ISizeOverrider
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

    private float _newSize;
    private int _SOOrder = 2000;
    public int SOOrder { get => _SOOrder; set => _SOOrder = value; }

    public void SetState(State state)
    {
        proCamera2DPointerInfluence.MaxHorizontalInfluence = state.maxHorizontalInfluence;
        proCamera2DPointerInfluence.MaxVerticalInfluence = state.maxVerticalInfluence;
        proCamera2DPointerInfluence.InfluenceSmoothness = state.influenceSmoothness;
        _newSize = state.changeSize;
    }

    public void AddTarget(Transform transform) => ProCamera2D.Instance.AddCameraTarget(transform, 1, 1, 1);
    public void RemoveTarget(Transform transform) => ProCamera2D.Instance.RemoveCameraTarget(transform, 1);

    private void OnDestroy()
    {
        ProCamera2D.Instance.RemoveSizeOverrider(this);
        proCamera2DRooms.OnStartedTransition.RemoveAllListeners();
        proCamera2DRooms.OnFinishedTransition.RemoveAllListeners();
        ServiceLocator.Unregister(this);
    }
    void Awake()
    {
        ServiceLocator.Register(this);
        proCamera2DShake = GetComponent<ProCamera2DShake>();
        proCamera2DPointerInfluence = GetComponent<ProCamera2DPointerInfluence>();
        proCamera2DRooms = GetComponent<ProCamera2DRooms>();
        ProCamera2D.Instance.AddSizeOverrider(this);
    }

    // private float t;
    public float OverrideSize(float deltaTime, float originalSize)
    {
        if (!enabled) return originalSize;

        // t += deltaTime;
        // if(Mathf.Abs(Mathf.Abs(originalSize) - Mathf.Abs(_newSize)) < Mathf.Epsilon) t = 0;

        return Utils.EaseFromTo(originalSize, _newSize, deltaTime*10);
    }
}
