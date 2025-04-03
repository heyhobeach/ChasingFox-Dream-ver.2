using UnityEngine;
using UnityEngine.Assertions;

public class GroundSensor : MonoBehaviour
{
    private Rigidbody2D target;
    // private Rigidbody2D thisRg;
    private CircleCollider2D col;

    private Vector2 offset = Vector2.zero;
    private Vector2 size = Vector2.zero;

    public bool isGrounded;
    public Vector2 normal = Vector2.up;
    private Vector2 contactPoint;

    public void Set(Rigidbody2D target, Collider2D targetCol)
    {
        Assert.IsNotNull(target, "target is Null");
        Assert.IsNotNull(target, "target Collider is Null");
        
        if(!col) col = GetComponent<CircleCollider2D>();
        // thisRg = GetComponent<Rigidbody2D>();
        this.target = target;
        size = targetCol.bounds.size * 0.5f;
        col.radius = size.x;
        offset = targetCol.offset;
        col.offset = offset + (Vector2.down * (size.y - col.radius));
        contactPoint = target.position;
    }

    private void OnCollisionEnter2D(Collision2D collision) => CollisionCheck(collision);
    private void OnCollisionStay2D(Collision2D collision) => CollisionCheck(collision);
    private void OnCollisionExit2D(Collision2D collision) => isGrounded = false;

    private void Awake() => col = GetComponent<CircleCollider2D>();

    private void FixedUpdate()
    {
        normal = ((Vector2)transform.position + col.offset) - contactPoint;
        normal = normal.normalized;

        if(isGrounded) Debug.DrawRay(transform.position, normal, Color.red);

        transform.position = target.position;
    }

    private void CollisionCheck(Collision2D collision)
    {
        isGrounded = false;
        contactPoint = transform.position;
        foreach(var contact in collision.contacts)
        {
            float angle = 0;
            if(CheckMapType(contact, ref angle) == MapType.Ground) 
            {
                isGrounded = true;
                if(contact.point.y > contactPoint.y) 
                {
                    contactPoint = contact.point;
                }
            }
        }
    }

    /// <summary>
    /// 충돌면의 MapType을 반환
    /// </summary>
    /// <param name="contact">충돌 지점점</param>
    /// <param name="angle">충돌각 반환 (0 ~ 180)</param>
    /// <returns>충돌면의 MapType</returns>
    protected MapType CheckMapType(ContactPoint2D contact, ref float angle)
    {
        angle = Mathf.Abs(Vector2.Angle(Vector2.up, contact.normal));
        if(angle <= 50) return MapType.Ground;
        else if(angle >= 130) return MapType.Floor;
        else return MapType.Wall;
    }
}
