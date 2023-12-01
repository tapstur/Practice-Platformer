using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{

    int horiAxis;
    int jumpCounter = 0;
    bool isWallSliding;
    float wallSlideStartTime;
    float jumpCheck;
    int vertAxis;

    [SerializeField] float maxWallSlideHangTime = 1f;
    [SerializeField] int maxJumpCount = 2;

    [SerializeField] float moveSpeed = 5;
    [SerializeField] Vector2 jumpForce;

    Rigidbody2D myRB;
    Animator myAnim;
    [SerializeField] BoxCollider2D groundCheckCollider;
    [SerializeField] PolygonCollider2D wallSlideCollider;
    CapsuleCollider2D myCol;
    CircleCollider2D environmentCol;
    SpriteRenderer mySR;

    private void Awake()
    {
        myRB = GetComponent<Rigidbody2D>();
        mySR = GetComponentInChildren<SpriteRenderer>();
        myAnim = GetComponentInChildren<Animator>();
        myCol = GetComponent<CapsuleCollider2D>();
    }

    void FixedUpdate()
    {
        
        PlayerRun();

        if (isWallSliding)
        {
            WallSlide();
        }
        if (!isWallSliding)
        {
            myAnim.SetBool("isWalled", false);
            JumpingFallingCheck();
            FlipSprite();
        }
        if(myRB.gravityScale == 0)
        {
            StartCoroutine(ResetGravityInSeconds(1f));
        }
    }

    private IEnumerator ResetGravityInSeconds(float time)
    {
        yield return new WaitForSeconds(time);

        myRB.gravityScale = 3.5f;
    }

    private void WallSlide()
    {
        if (Time.time - wallSlideStartTime < maxWallSlideHangTime)
        {
            myAnim.SetBool("isJumping", false);
            myAnim.SetBool("isWalled", true);
            myRB.gravityScale = 0f;
        }
        else myRB.gravityScale = 3.5f;
    }

    private void JumpingFallingCheck()
    {
        if (groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnim.SetBool("isFalling", false);
        }
        if (myRB.velocity.y < 0 && !groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnim.SetBool("isJumping", false);
            myAnim.SetBool("isFalling", true);
        }
    }

    private void PlayerRun()
    {
        myRB.velocity = new Vector2(horiAxis * moveSpeed, myRB.velocity.y);

        bool playerHasHorizontalSpeed = Mathf.Abs(myRB.velocity.x) > Mathf.Epsilon;

        if (!isWallSliding && groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            myAnim.SetBool("isRunning", playerHasHorizontalSpeed);
        }
    }

    //Multiple Trigger colliders set up here

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Solid Platforms")
        {
            jumpCounter = 0;
            myAnim.SetInteger("jumpCount", jumpCounter);

            if (wallSlideCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) &&
                !groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) &&
                !isWallSliding)
            {
                isWallSliding = true;
                myRB.velocity = Vector2.zero;
                wallSlideStartTime = Time.time;
            }
            else if (groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                myAnim.SetBool("isJumping", false);
                isWallSliding = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isWallSliding)
        {
            myAnim.SetBool("isWalled", false);
            isWallSliding = false;
            myRB.gravityScale = 3.5f;
        }
    }

    private void FlipSprite()
    {

        if (horiAxis < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (horiAxis > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }

        else transform.localScale = transform.localScale;
    }

    #region Input Actions

    public void OnMove(InputValue value)
    {
        horiAxis = Mathf.RoundToInt(value.Get<Vector2>().x);
        vertAxis = Mathf.RoundToInt(value.Get<Vector2>().y);
    }

    public void OnJump(InputValue value)
    {
        //groundCheckCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))
        
        if (jumpCounter < maxJumpCount)
        {
            //Maybe add code to allow for wall jump to opposite side of sliding
            myRB.gravityScale = 3.5f;
            myRB.velocity = Vector2.zero;
            myRB.AddForce(jumpForce, ForceMode2D.Impulse);
            myAnim.SetBool("isJumping", true);
            
            isWallSliding = false;
            jumpCounter++;
            myAnim.SetInteger("jumpCount", jumpCounter);
        }
    }

    #endregion

}
