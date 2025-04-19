using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class PlatformSensor : MonoBehaviour
{
    private Rigidbody2D target;
    private EdgeCollider2D col;

    private Vector2 size = Vector2.zero;

    Vector2[] defaultPoints = new Vector2[2];
    Vector2[] groundPoints = new Vector2[2];
    Vector2[] onPlaotformPoints = new Vector2[2];

    [DisableInspector] public PlatformScript currentPlatform;
    public bool onPlatform;
    public Vector2 normal = Vector2.up;

    public void Set(Rigidbody2D target, Collider2D targetCol)
    {
        Assert.IsNotNull(target, "target is Null");
        Assert.IsNotNull(target, "target Collider is Null");
        
        if(!col) col = GetComponent<EdgeCollider2D>();
        this.target = target;
        col.offset = targetCol.offset;
        size = targetCol.bounds.size * 0.5f;

        defaultPoints[0] = new Vector2(-size.x + 0.05f, -size.y - 0.1f);
        defaultPoints[1] = new Vector2(size.x - 0.05f, -size.y - 0.1f);
        groundPoints[0] = new Vector2(-size.x - 0.1f, -size.y - 0.1f);
        groundPoints[1] = new Vector2(size.x + 0.1f, -size.y - 0.1f);
        onPlaotformPoints[0] = new Vector2(-size.x + 0.1f, -size.y - 0.1f);
        onPlaotformPoints[1] = new Vector2(size.x - 0.1f, -size.y - 0.1f);

        col.points = defaultPoints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isDelay) CheckCollision(collision);
    }
    private void OnCollisionStay2D(Collision2D collision) => CheckCollision(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("platform")) 
        {
            var temp = collision.gameObject.GetComponent<PlatformScript>();
            if(temp.Equals(currentPlatform)) 
            {
                currentPlatform = null;
                onPlatform = false;
                normal = Vector2.up;
                if(gameObject.activeSelf) StartCoroutine(Delay());
            }
        }
    }

    private void Awake() => col = GetComponent<EdgeCollider2D>();

    private void FixedUpdate()
    {
        transform.position = target.position;

        if(currentPlatform) col.points = onPlaotformPoints;
        else
        {
            if(GameManager.fps < 120)
            {
                var temp = 0.01f * (120 / GameManager.fps);
                groundPoints[0] = new Vector2(-size.x - temp, -size.y - 0.1f);
                groundPoints[1] = new Vector2(size.x + temp, -size.y - 0.1f);
                col.points = groundPoints;
            }
            else col.points = defaultPoints;
            onPlatform = false;
        }

        if(onPlatform) Debug.DrawRay(transform.position, normal, Color.red);
    }

    private void CheckCollision(Collision2D collision)
    {
        var psc = collision.gameObject.GetComponent<PlatformScript>();
        switch(CheckMapType(collision))
        {
            case MapType.Platform:
                if(!currentPlatform) 
                {
                    currentPlatform = psc;
                    normal = collision.GetContact(0).normal;
                    onPlatform = true;
                }
                break;
            case MapType.Floor:
                psc.RemoveColliderMask(1 << target.gameObject.layer);
                break;
            case MapType.Wall:
                switch(psc.dObject)
                {
                    case PlatformScript.downJumpObject.STRAIGHT:
                        if(!currentPlatform) 
                        {
                            currentPlatform = psc;
                            psc.AddColliderMask(1 << target.gameObject.layer);
                            normal = Vector2.up;
                            onPlatform = true;
                        }
                        break;
                    case PlatformScript.downJumpObject.DIAGONAL:
                        psc.RemoveColliderMask(1 << target.gameObject.layer);
                        normal = Vector2.up;
                        break;
                }
                break;
        }
    }

    private bool isDelay;
    private IEnumerator Delay()
    {
        isDelay = true;
        yield return new WaitForFixedUpdate();
        isDelay = false;
    }
    
    /// <summary>
    /// 충돌면의 MapType을 반환
    /// </summary>
    /// <param name="collision">충돌체</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision)
    {
        float angle = 0;
        return CheckMapType(collision, ref angle);
    }
    /// <summary>
    /// 충돌면의 MapType을 반환
    /// </summary>
    /// <param name="collision">충돌체</param>
    /// <param name="angle">충돌각 반환 (0 ~ 180)</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision, ref float angle)
    {
        Debug.Assert(collision.contactCount > 0, "콜리전 충돌이 감지되지 않음");
        if(!collision.gameObject.CompareTag("platform") || collision.contactCount <= 0) return MapType.None;
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.GetContact(collision.contactCount-1).normal));
        if(angle <= 50) return MapType.Platform;
        else if(angle > 130) return MapType.Floor;
        else return MapType.Wall;
    }
}
