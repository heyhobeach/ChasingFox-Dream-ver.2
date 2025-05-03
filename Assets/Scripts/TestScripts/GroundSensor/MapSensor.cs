using UnityEngine;

public class MapSensor : MonoBehaviour
{
    public GroundSensor groundSensor;
    public PlatformSensor platformSensor;
    public MapType groundType 
    { 
        get
        {
            return groundSensor.isGrounded ? MapType.Ground 
                    : platformSensor.onPlatform ? MapType.Platform 
                    : MapType.None;
        }
    }

    public PlatformScript currentPlatform 
    { 
        get => platformSensor.currentPlatform; 
        set => platformSensor.currentPlatform = value; 
    }
    public bool isGrounded { get => platformSensor.onPlatform || groundSensor.isGrounded; }
    public Vector2 normal { 
        get
        {
            if(platformSensor.onPlatform) return platformSensor.normal;
            return groundSensor.normal;
        }
    }

    public void Set(Rigidbody2D target, Collider2D targetCol)
    {
        Debug.Log(targetCol.bounds.size);
        groundSensor.Set(target, targetCol);
        platformSensor.Set(target, targetCol);
    }
}
