using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float jumpForce;
    [SerializeField] float speed;
    [SerializeField] Transform groundPosition;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float jumpTime;

    Rigidbody2D rb;
    float moveInput;

    bool isGrounded;
    bool isJumping;

    float jumpTimeCounter;
    bool facingRight = true;

    bool isShooting;
    [SerializeField] GameObject arrow;
    [SerializeField] Transform arrowPos;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!isShooting)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            isGrounded = Physics2D.OverlapCircle(groundPosition.position, groundCheckRadius, whatIsGround);

            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger("takeOff");
                isJumping = true;
                jumpTimeCounter = jumpTime;
                rb.velocity = Vector2.up * jumpForce;
            }
            if (Input.GetKey(KeyCode.Space) && isJumping)
            {
                if (jumpTimeCounter > 0)
                {
                    rb.velocity = Vector2.up * jumpForce;
                    jumpTimeCounter -= Time.deltaTime;
                }
                else
                {
                    isJumping = false;
                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
            }

            if (isGrounded)
                anim.SetBool("isJumping", false);
            else
            {
                anim.SetBool("isJumping", true);
            }


            // If the input is moving the player right and the player is facing left...
            if (moveInput > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (moveInput < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }

            if (moveInput == 0)
                anim.SetBool("isRunning", false);
            else
            {
                anim.SetBool("isRunning", true);
            }
        }

        


        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            isShooting = true;
            anim.SetTrigger("tenseBow");
            
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Debug.Log(arrow.transform.rotation);
            anim.SetTrigger("releaseBow");
            Quaternion tmpRot = arrowPos.gameObject.transform.rotation;
            GameObject tempArrow = Instantiate(arrow, arrowPos.position, tmpRot);
            tempArrow.GetComponent<Rigidbody2D>().AddForce(Vector2.right * 25, ForceMode2D.Impulse);
            isShooting = false;
        }



    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        if (isJumping)
        {
    
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
