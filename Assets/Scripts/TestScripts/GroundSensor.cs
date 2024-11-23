using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    private Rigidbody2D target;
    private EdgeCollider2D col;

    public void Set(Rigidbody2D target, Vector2 offset, Vector2 size)
    {
        if(!col) col = GetComponent<EdgeCollider2D>();
        this.target = target;
        col.offset = offset;
        col.points = new Vector2[] {
            new Vector2(-size.x * 0.95f, -size.y - 0.05f),
            new Vector2(size.x * 0.95f, -size.y - 0.05f),
        };
    }

    [HideInInspector] public PlatformScript currentPlatform;
    private bool _isGrounded;
    public bool isGrounded { get => currentPlatform || _isGrounded; }
    public Vector2 normal { get => _isGrounded ? groundNormal : platformNormal; }
    private Vector2 groundNormal = Vector2.up;
    private Vector2 platformNormal = Vector2.up;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CheckCollision(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Map")) _isGrounded = false;
        if(collision.gameObject.CompareTag("platform")) 
        {
            currentPlatform?.AddColliderMask(1 << target.gameObject.layer);
            currentPlatform = null;
        }
    }

    private void Awake() => col = GetComponent<EdgeCollider2D>();

    private void Update() => transform.position = target.position;

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
    /// <param name="ref angle">충돌각 반환 (0 ~ 180)</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision, ref float angle)
    {
        if(!(collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("platform")) || collision.contactCount <= 0) return MapType.None;
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal));
        point = collision.GetContact(0).point;
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
                    platformNormal = collision.GetContact(0).normal;
                    currentPlatform = psc;
                }
                break;
            case MapType.None:
                var spr = collision.gameObject.GetComponent<PlatformScript>();
                switch(spr.dObject)
                {
                    case PlatformScript.downJumpObject.STRAIGHT:
                        if(!currentPlatform) 
                        {
                            var psc = collision.gameObject.GetComponent<PlatformScript>();
                            platformNormal = collision.GetContact(0).normal;
                            currentPlatform = psc;
                        }
                        break;
                    case PlatformScript.downJumpObject.DIAGONAL:
                        spr.RemoveColliderMask(1<<target.gameObject.layer);
                        break;
                }
                break;
        }
    }

    Vector2 point = Vector2.zero;
    private void OnDrawGizmo()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point, 0.1f);
    }
}
