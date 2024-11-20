using UnityEngine;

public class GroundSensor : MonoBehaviour
{
    Rigidbody2D target;
    float boxOffsetY;
    float boxSizeY;

    public void Set(Rigidbody2D target, float offset, float size)
    {
        this.target = target;
        boxOffsetY = offset;
        boxSizeY = size;
    }

    public PlatformScript currentPlatform;
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
                collision.gameObject.GetComponent<PlatformScript>()?.RemoveColliderMask(1<<target.gameObject.layer);
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
                collision.gameObject.GetComponent<PlatformScript>()?.RemoveColliderMask(1<<target.gameObject.layer);
                break;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Map")) _isGrounded = false;
        else if(collision.gameObject.CompareTag("platform")) currentPlatform = null;
    }

    private void Update()
    {
        transform.position = target.position + (Vector2.down*0.05f);
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
        point = collision.contacts[0].point;
        if(collision.gameObject.CompareTag("platform") && angle <= 50) return MapType.Platform;
        else if(collision.gameObject.CompareTag("platform") && angle > 50) return MapType.None;
        if(angle <= 45 || collision.gameObject.tag.Equals("ground")) return MapType.Ground;
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

    Vector2 point;
    void OnDrawGizmos()
    {
        if(point != null) Gizmos.DrawSphere(point, 0.5f);
    }
}
