using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMoveTestScript : MonoBehaviour
{
    private const float gravity = -9.81f;
    private LayerMask lm;
    [SerializeField] private float speed;
    [SerializeField] private float accelerate;
    [SerializeField] private float jumpForce;
    private float moveVec = 0;
    private float velocity = 0;

    private float distanceToCheck = 0.5f;
    private bool isGrounded;
    private bool isJumpReady;
    private bool jumpKey;
    private bool jumpKeyUp;
    private bool isJumping;

    // Start is called before the first frame update
    void Start()
    {
        lm = ~(1<<gameObject.layer);
        isJumpReady = true;
        distanceToCheck = gameObject.GetComponent<BoxCollider2D>().size.y * 0.7f;
    }

    // Update is called once per frame
    void Update()
    {
        ConditionUpdate();
    }
    void FixedUpdate()
    {
        Move(new Vector3(HorizontalForce(), VerticalForce()) * Time.deltaTime);
        // MoveToGround();
    }


    private void ConditionUpdate()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, distanceToCheck, lm);
        moveVec += (Input.GetAxisRaw("Horizontal")-moveVec) * accelerate;
        if(Input.GetAxisRaw("Horizontal") == 0) moveVec += (Input.GetAxisRaw("Horizontal")-moveVec) * accelerate;
        moveVec = Mathf.Clamp(moveVec, -1, 1);

        jumpKey = Input.GetKey(KeyCode.W);
        if(!jumpKeyUp) jumpKeyUp = Input.GetKeyUp(KeyCode.W);
        if(isGrounded && jumpKeyUp)
        {
            isJumpReady = true;
            jumpKeyUp = false;
        }
        if(isJumpReady && jumpKey)
        {
            isJumping = true;
            if(jumpKeyUp) isJumpReady = false;
            if(velocity/jumpForce >= 0.9f) isJumpReady = false;
        }
        else isJumping = false;
    }
    private void Move(Vector3 force) => transform.Translate(force);

    private float VerticalForce()
    {
        velocity += gravity * Time.deltaTime;
        if(isGrounded && velocity < 0) velocity = 0;
        if(isJumping)
        {
            if(velocity <= 0) velocity = jumpForce/2;
            velocity = Mathf.Sin(velocity/jumpForce) * jumpForce * 1.2f;
        }
        return velocity;
    }
    private float HorizontalForce()
    {
        return moveVec * speed;
    }

    private void MoveToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceToCheck, 1<<LayerMask.NameToLayer("OneWayPlatform"));
        if(hit) transform.position = new Vector3(hit.point.x, hit.point.y+distanceToCheck, transform.position.z);
    }
}
