using UnityEngine;

public class FallingTrap : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public HingeJoint2D joint2D;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (joint2D != null)
        {
            joint2D.useConnectedAnchor = false;
            joint2D.connectedBody = null;
        }
    }
}
