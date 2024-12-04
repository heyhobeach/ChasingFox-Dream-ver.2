using UnityEngine;
using UnityEngine.Assertions;

public class GroundSensor : MonoBehaviour
{
    private Rigidbody2D target;
    private EdgeCollider2D col;
    private Vector2 startAP = Vector2.zero;
    private Vector2 endAP = Vector2.zero;

    [HideInInspector] public PlatformScript currentPlatform;
    private bool _isGrounded;
    public bool isGrounded { get => currentPlatform || _isGrounded; }
    public Vector2 normal { get => _isGrounded ? groundNormal : platformNormal; }
    private Vector2 groundNormal = Vector2.up;
    private Vector2 platformNormal = Vector2.up;

    public void Set(Rigidbody2D target, Collider2D targetCol)
    {
        Assert.IsNotNull(target, "target is Null");
        Assert.IsNotNull(target, "target Collider is Null");
        
        if(!col) col = GetComponent<EdgeCollider2D>();
        this.target = target;
        col.offset = targetCol.offset;
        var temp = targetCol.bounds.size * 0.5f;
        col.points = new Vector2[] {
            new Vector2(-temp.x + 0.05f, -temp.y - 0.05f),
            new Vector2(temp.x - 0.05f, -temp.y - 0.05f),
        };
        startAP = new Vector2(-temp.x + 0.05f, -temp.y - 0.05f);
        endAP = new Vector2(temp.x - 0.05f, -temp.y - 0.05f);
    }

    private void OnCollisionEnter2D(Collision2D collision) => CheckCollision(collision);
    private void OnCollisionStay2D(Collision2D collision) => CheckCollision(collision);
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Map")) _isGrounded = false;
        if(collision.gameObject.CompareTag("platform")) 
        {
            var temp = collision.gameObject.GetComponent<PlatformScript>();
            if(currentPlatform && temp && currentPlatform.Equals(temp))
            {
                currentPlatform.AddColliderMask(1 << target.gameObject.layer);
                currentPlatform = null;
            }
        }
    }

    private void Awake()
    {
        col = GetComponent<EdgeCollider2D>();
    }

    private void FixedUpdate()
    {
        transform.position = target.position;
        if(_isGrounded)
        {
            col.useAdjacentStartPoint = true;
            col.useAdjacentEndPoint = true;
            var temp = 0.05f * (120 / GameManager.fps) + 0.05f;
            col.adjacentStartPoint = new Vector2(startAP.x - temp, startAP.y);
            col.adjacentEndPoint = new Vector2(endAP.x + temp, endAP.y);
        }
        else
        {
            col.useAdjacentStartPoint = false;
            col.useAdjacentEndPoint = false;
        }
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
        if(!(collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("platform")) || collision.contactCount <= 0) return MapType.None;
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal));
        if(collision.gameObject.CompareTag("platform") && angle <= 50) return MapType.Platform;
        else if(collision.gameObject.CompareTag("platform") && angle > 50) return MapType.None;
        if(angle <= 45) return MapType.Ground;
        else if(angle >= 135) return MapType.Floor;
        else return MapType.Wall;
    }

    private void CheckCollision(Collision2D collision)
    {
        switch(CheckMapType(collision))
        {
            case MapType.Ground:
                _isGrounded = true;
                groundNormal = collision.GetContact(0).normal;
                break;
            case MapType.Platform:
                if(!currentPlatform) 
                {
                    var psc = collision.gameObject.GetComponent<PlatformScript>();
                    switch(psc.dObject)
                    {
                        case PlatformScript.downJumpObject.STRAIGHT:
                            platformNormal = Vector2.up;
                            break;
                        case PlatformScript.downJumpObject.DIAGONAL:
                            platformNormal = collision.GetContact(0).normal;
                            break;
                    }
                    currentPlatform = psc;
                }
                break;
            case MapType.None:
                var spr = collision.gameObject.GetComponent<PlatformScript>();
                switch(spr?.dObject)
                {
                    case PlatformScript.downJumpObject.STRAIGHT:
                        if(!currentPlatform) 
                        {
                            platformNormal = Vector2.up;
                            currentPlatform = spr;
                        }
                        break;
                    case PlatformScript.downJumpObject.DIAGONAL:
                        spr.RemoveColliderMask(1<<target.gameObject.layer);
                        break;
                }
                break;
        }
    }
}
