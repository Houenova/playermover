using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverplayer : MonoBehaviour
{
    private Vector2 vecGravity; 


    private bool isFacingRight = true;
    private bool isJumping;

    [SerializeField] private float dashingVelocity = 14f;
    [SerializeField] private float dashingTime = 0.2f;
    private Vector2 dashingDir;
    private bool isDashing;
    private bool canDash = true;


    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.4f;
    private Vector2 wallJumpingPower = new Vector2(8f, 16f);

    [SerializeField] private Rigidbody2D rb;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    
    [SerializeField] float fallMultiplier;

    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;

    [SerializeField] private TrailRenderer tr;


private void Start()
{
    vecGravity = new Vector2(0, -Physics2D.gravity.y);
}

void Update()
{
    if (rb.velocity.y <0 )
    {
        rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;
    }


if (Input.GetButtonDown("Dash") && canDash)
    {
        isDashing = true;
        canDash = false;
        tr.emitting = true;
        dashingDir = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    
        if (dashingDir == Vector2.zero)
        {
            dashingDir = new Vector2(transform.localScale.x, 0f);
        }
        StartCoroutine(StopDashing());
    }
    if (isDashing)
    {
        rb.velocity = dashingDir.normalized * dashingVelocity;
        return;
    }
    if (IsGrounded())
    {
        canDash = true;
    }

    if (isWalled())
    {
        canDash = true;
    }


    horizontal = Input.GetAxisRaw("Horizontal");


    if (IsGrounded())
    {
        coyoteTimeCounter = coyoteTime;
    }
    else
    {
        coyoteTimeCounter -= Time.deltaTime;
    }

    if (Input.GetButtonDown("Jump"))
    {
        jumpBufferCounter = jumpBufferTime;
    }
    else
    {
        jumpBufferCounter -= Time.deltaTime;
    }

    if (coyoteTimeCounter > 0f && jumpBufferCounter > 0f && !isJumping)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

        jumpBufferCounter = 0f;

        StartCoroutine(JumpCooldown());
    }

    if  (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
    {
        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
    
        coyoteTimeCounter = 0f;
    }

    



    Flip();

    WallSlide();
    WallJump();
}

    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            Vector3 localScale = transform.localScale;
            isFacingRight = !isFacingRight;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    private IEnumerator JumpCooldown()
    {
        isJumping = true;
        yield return new WaitForSeconds(0.4f);
        isJumping = false;
    }

    private bool isWalled()
    {
        return  Physics2D.OverlapCircle(wallCheck.position, 0.2f ,wallLayer);
    }

    private void WallJump()
    {
        if(isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else 
        {
            wallJumpingCounter -= Time.deltaTime;
        }
        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;
            
            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight =!isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
           }
           Invoke(nameof(StopWallJumping), wallJumpingDirection);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void WallSlide()
    {
        if (isWalled() && !IsGrounded() && horizontal!= 0f )
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else 
        {
            isWallSliding = false;
        }
    }
    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        tr.emitting =true;
        isDashing = false;
    }


   
}
