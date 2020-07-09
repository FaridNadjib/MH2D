using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class controls the base movement behaviour of the player. It handles the input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Maincamera:")]
    [SerializeField] Camera maincam;

    [Header("Walking related variables:")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [Range(0f,1f)]
    [SerializeField] float airSpeed;
    float moveInput;
    float speed;
    bool facingRight = true;
    bool isSprinting;
    bool isSliding;
    bool blockInput = false;

    [Header("Groundcheck related variables:")]
    [SerializeField] Transform groundPosition;
    [SerializeField] float groundCheckRadius;
    [SerializeField] LayerMask whatIsGround;
    bool isGrounded;

    [Header("Jumping related variables:")]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpTime;
    float jumpTimeCounter;
    bool isJumping;

    [Header("Crouching related variables:")]
    [SerializeField] bool isCrouching;
    [SerializeField] float crouchSpeed;
    [SerializeField] Transform ceilingCheck;
    [SerializeField] float ceilingCheckRadius;
    bool canStandUp;

    [Header("Shooting related variables:")]
    [SerializeField] Transform leftHandWeaponPos;
    [SerializeField] Transform rightHandWeaponPos;
    [SerializeField] float offset;
    [SerializeField] float shootTime;
    [SerializeField] float shootDuration;
    Vector2 projectilePosition;
    ActiveWeaponHand activeHand;
    float shootTimeCounter;
    float projectileSpeedMult;
    bool isShooting;
    bool canShootAgain = true;
    Vector2 direction;
    GameObject tempProj;
    ActiveWeaponType activeWeapon;

    [Header("Projectile trajectory related variables:")]
    [SerializeField] bool enableLine = true;
    [SerializeField] LineRenderer line;
    [SerializeField] int numberOfPoints;
    [SerializeField] float spaceBetweenPoints;

    [Header("Weapon equipment positions:")]
    [SerializeField] SpriteRenderer rightHandWeapon;
    [SerializeField] SpriteRenderer leftHandWeapon;

    Rigidbody2D rb;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        // Set the default vaulues.
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        speed = walkSpeed;

        // Subscribe to event which gets triggered when equipped weapon is changed.
        PlayerWeaponChanger.instance.OnWeaponChanged += (ActiveWeaponType activeWeapon, ActiveWeaponHand activeHand, Sprite weaponIcon) => { this.activeWeapon = activeWeapon;
            this.activeHand = activeHand;
            // Get the right weapon sprite.
            GameObject tempProjectile = ObjectPoolsController.instance.GetFromPool(activeWeapon.ToString());
            if (this.activeHand == ActiveWeaponHand.Left)
                leftHandWeapon.sprite = tempProjectile.GetComponent<SpriteRenderer>().sprite;
            else
                rightHandWeapon.sprite = tempProjectile.GetComponent<SpriteRenderer>().sprite;
            ObjectPoolsController.instance.AddToPool(tempProjectile, activeWeapon.ToString());
        };
    }

    // Update is called once per frame
    void Update()
    {
        // As long as the player isnt shooting or input was blocked, check for his input.
        if (!isShooting && !blockInput)
        {
            // Get horizontalinput and flip the player. Do the groundcheck.
            moveInput = Input.GetAxisRaw("Horizontal");
            if (moveInput > 0)
                Flip(true);
            else if (moveInput < 0)
                Flip(false);
            isGrounded = Physics2D.OverlapCircle(groundPosition.position, groundCheckRadius, whatIsGround);

            // Jump calculations.
            JumpCheck();

            // Set the the player speed and the animations for sprinting, walking and sliding.
            if (isGrounded)
            {
                if (moveInput == 0)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isSprinting", false);
                    if (isSliding)
                    {
                        canStandUp = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround);
                        if (canStandUp)
                        {
                            isCrouching = true;
                            anim.SetBool("isCrouching", true);
                            speed = crouchSpeed;
                            isSliding = false;
                        }
                        else if (!canStandUp)
                        {
                            anim.SetBool("isSliding", false);
                            isSliding = false;
                        }
                    }
                }
                else
                {
                    anim.SetBool("isWalking", true);
                    if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
                    {
                        anim.SetBool("isSprinting", true);
                        isSprinting = true;
                        speed = sprintSpeed;
                        if (Input.GetKeyDown(KeyCode.S) && isSprinting)
                        {
                            anim.SetTrigger("slideDown");
                            anim.SetBool("isSliding", true);
                            isSliding = true;
                        }
                    }
                    else if (isCrouching)
                        speed = crouchSpeed;
                    else
                    {
                        anim.SetBool("isSprinting", false);                       
                        isSprinting = false;                        
                        speed = walkSpeed;
                        if(isSliding && !isSprinting)
                        {
                            canStandUp = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround);
                            if (canStandUp)
                            {
                                isCrouching = true;
                                anim.SetBool("isCrouching", true);
                                speed = crouchSpeed;
                                isSliding = false;
                            }
                            else if (!canStandUp)
                            {
                                anim.SetBool("isSliding", false);
                                isSliding = false;
                            }
                        }
                    }
                }
                // Crouch calculations.
                CrouchCheck();
            }                      
        }

        // Shooting calculations.
        ShootingCheck();
    
    }

    private void FixedUpdate()
    {
        // Set the players velocity according to its horizontal input. In case aircontrol is wanted calculate that too.
        if(isGrounded)
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        else if(!isGrounded)
            rb.velocity = new Vector2(moveInput * speed * airSpeed, rb.velocity.y);
    }

    /// <summary>
    /// Filps the playercharacter by rotate his y by 180 degrees.
    /// </summary>
    /// <param name="faceDirectionChange">The direction the player should face: true = right, false = face left.</param>
    private void Flip(bool faceDirectionChange)
    {
        facingRight = faceDirectionChange;

        if (facingRight)
            transform.eulerAngles = new Vector3(0, 0, 0);
        else
            transform.eulerAngles = new Vector3(0, 180, 0);
    }

    /// <summary>
    /// This method does the jump calculations, its called in the Update every frame.
    /// </summary>
    private void JumpCheck()
    {
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
                isJumping = false;
        }

        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;

        if (isGrounded)
            anim.SetBool("isJumping", false);
        else
            anim.SetBool("isJumping", true);
    }

    /// <summary>
    /// This method does the crouch calculations and is called in the Update once per frame.
    /// </summary>
    private void CrouchCheck()
    {
        // crouching
        canStandUp = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround);
        
        if (Input.GetKeyDown(KeyCode.S) && !isSprinting)
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        }
        else if (!Input.GetKey(KeyCode.S) && !canStandUp)
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        }
    }

    private void ShootingCheck()
    {
        // Only start shooting if hes on the ground and not moving and time passed after his last shot. Set the animation according to active weapon hand.
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && moveInput == 0 && canShootAgain)
        {
            isShooting = true;
            canShootAgain = false;
            if (activeHand == ActiveWeaponHand.Left)
                anim.SetTrigger("tenseBow");
            else
                anim.SetTrigger("tenseRightHandShot");
            shootTimeCounter = 0.0f;
            projectileSpeedMult = 0.9f;
        }

        // Count the time how long the fire button is pressed, to determine how strong the shot will be and if the player can already shoot again.
        if (!canShootAgain || isShooting)
        {
            if (shootTimeCounter < shootTime)
                shootTimeCounter += Time.deltaTime;
            else
                shootTimeCounter = shootTime;
            if (shootTimeCounter >= shootDuration)
                canShootAgain = true;
        }

        // As long as the player is aiming, update the animation and make him build up power for harder shots.
        if (isShooting)
        {
            // Calculate the direction the player is pointing towards.       
            Vector2 mousePos = maincam.ScreenToWorldPoint(Input.mousePosition);
            projectilePosition = activeHand == ActiveWeaponHand.Left ? leftHandWeaponPos.position : rightHandWeaponPos.position;
            direction = mousePos - projectilePosition;

            // Stay in the bounds of the animation. Camp the direction.
            if (direction.x < 3 && facingRight)
                direction.x = 3;
            else if (direction.x > -3 && !facingRight)
                direction.x = -3;
            if (direction.y > 10)
                direction.y = 10;
            else if (direction.y < -2)
                direction.y = -2;

            // Filp the player depending on where he is aiming.
            Vector2 aimDirection = mousePos - (Vector2)transform.position;
            if (isShooting && aimDirection.x > 0)
                Flip(true);
            else if (isShooting && aimDirection.x < 0)
                Flip(false);

            // Set the animation according to where the players is pointing.
            anim.SetFloat("shotAngle", aimDirection.normalized.y);
            
            //if (shootTimeCounter >= 0.3f)
            //    projectileSpeedMult = 1f;
            //if (shootTimeCounter >= 0.6f)
            //    projectileSpeedMult = 1.1f;
            //if (shootTimeCounter >= shootTime)
            //    projectileSpeedMult = 1.3f;

            projectileSpeedMult = Mathf.Lerp(0.9f, 1.8f, shootTimeCounter / shootTime);

            // Draw the trajectory line if that option is enabled.
            if (enableLine)
                DrawLine();

            // If the key was released activate the projectile.
            if (Input.GetKeyUp(KeyCode.LeftControl) && isGrounded)
            {
                GameObject tempProjectile = ObjectPoolsController.instance.GetFromPool(activeWeapon.ToString());
                tempProj = tempProjectile;
                tempProjectile.transform.position = projectilePosition;
                if (activeHand == ActiveWeaponHand.Left)
                    tempProjectile.transform.rotation = leftHandWeaponPos.rotation;
                else
                {
                    tempProjectile.transform.rotation = rightHandWeaponPos.rotation;
                    // Not elegant but i had a "bug" where spears got wrong rotation due to players rotaion. Fixed it that way.
                    Vector2 scale = tempProjectile.transform.localScale;
                    if (facingRight && scale.x < 0)
                        scale.x *= -1;
                    else if (!facingRight && scale.x > 0)
                        scale.x *= -1;
                    tempProjectile.transform.localScale = scale;
                }
                tempProjectile.SetActive(true);
                tempProjectile.GetComponent<Projectile>().ShootProjectile(direction.normalized * projectileSpeedMult);


                if (activeHand == ActiveWeaponHand.Left)
                    anim.SetTrigger("releaseBow");
                else
                    anim.SetTrigger("releaseRightHandShot");
                isShooting = false;
                line.enabled = false;
            }
        }           
    }

    Vector2 PointPosition(float time)
    {
        Vector2 pos = new Vector2();
        if(tempProj != null)
        {
            float velo = tempProj.GetComponent<Projectile>().projectileSpeed;
            float graScale = tempProj.GetComponent<Rigidbody2D>().gravityScale;
            direction = (Vector2)maincam.ScreenToWorldPoint(Input.mousePosition) - projectilePosition;

            if (direction.x < 3 && facingRight)
                direction.x = 3;
            else if (direction.x > -3 && !facingRight)
                direction.x = -3;
            if (direction.y > 10)
                direction.y = 10;
            else if (direction.y < -2)
                direction.y = -2;
            pos = projectilePosition + (direction.normalized * velo * projectileSpeedMult * time) + 0.5f * (Physics2D.gravity * graScale) * (time * time);
        }              
        return pos;
    }

    /// <summary>
    /// Draw a line showing where the projectile will fly.
    /// </summary>
    void DrawLine()
    {
        line.enabled = true;
        line.positionCount = numberOfPoints;

        for (int i = 0; i < numberOfPoints; i++)
            line.SetPosition(i, new Vector3(PointPosition(i * spaceBetweenPoints).x, PointPosition(i * spaceBetweenPoints).y, 0));
    }
}
