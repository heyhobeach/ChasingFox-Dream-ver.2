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
            new Vector2(-size.x + 0.1f, -size.y - 0.05f),
            new Vector2(size.x - 0.1f, -size.y - 0.05f)
        };
    }

    [HideInInspector] public PlatformScript currentPlatform;
    private bool _isGrounded;
    public bool isGrounded { get => currentPlatform || _isGrounded; }
    public Vector2 normal { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch(CheckMapType(collision))
        {
            case MapType.Ground:
                SetPosition(collision);
                _isGrounded = true;
                normal = collision.GetContact(0).normal;
                if(currentPlatform) currentPlatform = null;
                break;
            case MapType.Platform:
                var psc = collision.gameObject.GetComponent<PlatformScript>();
                if(!currentPlatform) 
                {
                    SetPosition(collision);
                    if(!_isGrounded) normal = collision.GetContact(0).normal;
                    currentPlatform = psc;
                }
                break;
            case MapType.None:
                var spr = collision.gameObject.GetComponent<PlatformScript>();
                if(spr && spr.dObject == PlatformScript.downJumpObject.DIAGONAL && collision.collider.bounds.center.y > col.bounds.center.y)
                    spr.RemoveColliderMask(1<<target.gameObject.layer);
                break;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        switch(CheckMapType(collision))
        {
            case MapType.Ground:
                _isGrounded = true;
                normal = collision.GetContact(0).normal;
                if(currentPlatform) currentPlatform = null;
                break;
            case MapType.Platform:
                var psc = collision.gameObject.GetComponent<PlatformScript>();
                if(!currentPlatform) 
                {
                    if(!_isGrounded) normal = collision.GetContact(0).normal;
                    currentPlatform = psc;
                }
                break;
            case MapType.None:
                var spr = collision.gameObject.GetComponent<PlatformScript>();
                if(spr && spr.dObject == PlatformScript.downJumpObject.DIAGONAL && collision.collider.bounds.center.y > col.bounds.center.y)
                    spr.RemoveColliderMask(1<<target.gameObject.layer);
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Map")) _isGrounded = false;
        else if(collision.gameObject.CompareTag("platform")) currentPlatform = null;
    }

    private void Awake()
    {
        col = GetComponent<EdgeCollider2D>();
    }

    private void Update()
    {
        transform.position = target.position;
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
    /// <param name="ref angle">충돌각 반환 (0 ~ 180)</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(Collision2D collision, ref float angle)
    {
        if(!(collision.gameObject.CompareTag("Map") || collision.gameObject.CompareTag("platform")) || collision.contactCount <= 0) return MapType.None;
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, collision.contacts[0].normal));
        if(collision.gameObject.CompareTag("platform") && angle <= 50) return MapType.Platform;
        else if(collision.gameObject.CompareTag("platform") && angle > 50) return MapType.None;
        if(angle <= 45) return MapType.Ground;
        else if(angle >= 135) return MapType.Floor;
        else return MapType.Wall;
    }

    private void SetPosition(Collision2D collision)
    {
        ContactPoint2D contact = collision.contacts[0];
        var nor = contact.normal;
        var distance = contact.separation;
        target.transform.position += (Vector3) nor*distance;
    }
}
