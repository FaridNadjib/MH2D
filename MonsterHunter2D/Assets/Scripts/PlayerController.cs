﻿using System;
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
    [SerializeField] float sprintStaminaConsumption;
    [Range(0f,1f)]
    [SerializeField] float airSpeed;
    private float moveInput;
    public float MoveInput { get => moveInput; }
    float speed;
    bool facingRight = true;
    bool isSprinting;
    bool isSliding;
    public bool blockInput = false;

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
    [SerializeField] float shootStaminaConsumption;
    Vector2 projectilePosition;
    ActiveWeaponHand activeHand;
    float shootTimeCounter;
    float projectileSpeedMult;
    bool isShooting;
    bool canShootAgain = true;
    Vector2 direction;
    GameObject tempProj;
    ActiveWeaponType activeWeapon;

    // All the munition counter properties.
    public int CurrentArrows { get; private set; }
    public int CurrentSpears { get; private set; }
    public int CurrentPlatformSpears { get; private set; }
    public int CurrentStickyBombs { get; private set; }
    public int CurrentMegaBombs { get; private set; }

    public Dictionary<ActiveWeaponType, int> currentMunition = new Dictionary<ActiveWeaponType, int>();

    [Header("Projectile trajectory related variables:")]
    [SerializeField] bool enableLine = true;
    [SerializeField] LineRenderer line;
    [SerializeField] int numberOfPoints;
    [SerializeField] float spaceBetweenPoints;

    [Header("Weapon equipment positions:")]
    [SerializeField] SpriteRenderer rightHandWeapon;
    [SerializeField] SpriteRenderer leftHandWeapon;

    [Header("Invisibility related:")]
    [SerializeField] bool invisible = false;
    public bool Invisible { get => invisible; } 
    private bool canHide = true;
    public bool CanHide { set => canHide = value; }
    [SerializeField] float invisibileTime;
    [SerializeField] float invisibleCounter;
    [SerializeField] bool insideBush = false;
    public bool InsideBush { get => insideBush; set => insideBush = value; }
    private bool stealthSoundHasPlayed = false;
    // Event to update invisible fill amount.
    public delegate void InvisibleChanged(float invisibleFillAmount);
    public event InvisibleChanged OnInvisibleChanged;


    [Header("Damage taken related:")]
    [SerializeField] float invincibleTime;
    [SerializeField] float inputBlockedTime;
    bool damageTaken = false;
    float damageTakenTimer;
    [SerializeField] SpriteRenderer[] spritesToBlink;
    [SerializeField] float blinkIntervall;
    float blinkTimer;
    bool swapColors;
    [SerializeField] Color dmgTakenColor1;
    [SerializeField] Color dmgTakenColor2;
    [SerializeField] ParticleSystem blood1;
    [SerializeField] ParticleSystem blood2;

    [Header("The Player stats will save all progress:")]
    [SerializeField] PlayerStats playerStats;
    Rigidbody2D rb;
    Animator anim;

    [Header("The player physics material:")]
    [SerializeField] PhysicsMaterial2D physicsmat;

    //Connections to other scripts:
    CharacterResources characterResources;
    CharacterSounds characterSounds;
    RagdollController ragdoll;

    // Use this to check if the player is currently alive or not.
    public bool IsAlive { get; private set; } = true;

    // Start is called before the first frame update
    void Start()
    {
        // Set the default vaulues.
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        speed = walkSpeed;

        // Get the other scripts related to the player, all are on the same gameobject.
        characterResources = GetComponent<CharacterResources>();
        characterSounds = GetComponent<CharacterSounds>();
        ragdoll = GetComponent<RagdollController>();

        characterResources.SetHealth(playerStats.MaxHealth);
        characterResources.SetStamina(playerStats.MaxStamina);

        // Restock the palyers ammo.
        currentMunition.Add(ActiveWeaponType.ArrowNormal, playerStats.MaxArrows);
        currentMunition.Add(ActiveWeaponType.SpearNormal, playerStats.MaxSpears);
        currentMunition.Add(ActiveWeaponType.SpearPlatform, playerStats.MaxPlatformspears);
        currentMunition.Add(ActiveWeaponType.BombNormal, playerStats.MaxBombNormal);
        currentMunition.Add(ActiveWeaponType.BombSticky, playerStats.MaxStickyBomb);
        currentMunition.Add(ActiveWeaponType.BombMega, playerStats.MaxMegaBomb);

        // Get some defalut values from playerstats.
        invisibileTime = playerStats.MaxInvisibilityTime;

        // Subscribe to event which gets triggered when equipped weapon is changed.
        PlayerWeaponChanger.instance.OnWeaponChanged += (ActiveWeaponType activeWeapon, ActiveWeaponHand activeHand, Sprite weaponIcon) => { this.activeWeapon = activeWeapon;
            this.activeHand = activeHand;
            // Get the right weapon sprite.
            GameObject tempProjectile = ObjectPoolsController.instance.GetFromPool(activeWeapon.ToString());
            if (this.activeHand == ActiveWeaponHand.Left)
                leftHandWeapon.sprite = tempProjectile.GetComponent<SpriteRenderer>().sprite;
            else
                rightHandWeapon.sprite = tempProjectile.GetComponent<SpriteRenderer>().sprite;
            if(currentMunition.ContainsKey(activeWeapon))
                UIManager.instance.AmmoChanged(currentMunition[activeWeapon].ToString(), activeHand);
            ObjectPoolsController.instance.AddToPool(tempProjectile, activeWeapon.ToString());
        };

        playerStats.OnStatsChanged += () => {
            if (currentMunition.ContainsKey(playerStats.WeaponType))
            {
                currentMunition[playerStats.WeaponType] += playerStats.Amount;
                if(activeWeapon == playerStats.WeaponType)
                {
                    UIManager.instance.AmmoChanged(currentMunition[playerStats.WeaponType].ToString(), activeHand);
                }
            }
        };

        // Subscribe to event which get triggered when health of this units reaches 0.
        characterResources.OnUnitDied += () => {
            // Activate the ragdoll and disable movement.
            blockInput = true;
            anim.enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            ragdoll.EnableRagdoll();
            IsAlive = false;
            characterSounds.PlaySound(CharacterSounds.Sound.Dead, 0, true, false);
            playerStats.CurrentCrystals = 0;
            // Todo check if no more lifes left.
            playerStats.NumberOfHearts--;

            if (playerStats.NumberOfHearts > 0)
                UIManager.instance.ShowRetryMenu();
            else if(playerStats.NumberOfHearts<1)
            { 
                //show game end screen
            }

            playerStats.ValuesChanged();
            // ToDo: show message on screen to press l to reload last save or esc to open menu.
        };


        // Set the player pos based on last checkpoint.
        if(playerStats.SpawnPosX != 666)
        {
            transform.position = new Vector3(playerStats.SpawnPosX, playerStats.SpawnPosY, 0);
        }
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

            if (!isGrounded)
            {
                characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                characterSounds.StopSound(CharacterSounds.Sound.Walking);
                characterSounds.StopSound(CharacterSounds.Sound.Sprinting);
            }

            // Jump calculations.
            JumpCheck();

            // Set the the player speed and the animations for sprinting, walking and sliding.
            if (isGrounded)
            {
                if (moveInput == 0)
                {
                    characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                    characterSounds.StopSound(CharacterSounds.Sound.Walking);
                    characterSounds.StopSound(CharacterSounds.Sound.Sprinting);

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
                            characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                        }
                        else if (!canStandUp)
                        {
                            anim.SetBool("isSliding", false);
                            isSliding = false;
                            characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                        }
                    }
                }
                else
                {
                    if (!characterSounds.IsPlaying(CharacterSounds.Sound.Walking) && !characterSounds.IsPlaying(CharacterSounds.Sound.Sprinting) && !characterSounds.IsPlaying(CharacterSounds.Sound.Sliding) && !characterSounds.IsPlaying(CharacterSounds.Sound.Stealth))
                        characterSounds.PlaySound(CharacterSounds.Sound.Walking, 0, false, true);

                    anim.SetBool("isWalking", true);

                    if (Input.GetKey(KeyCode.LeftShift) && !isCrouching && characterResources.HasStamina)
                    {
                        characterResources.ReduceStamina(sprintStaminaConsumption * Time.deltaTime);
                        anim.SetBool("isSprinting", true);
                        isSprinting = true;
                        speed = sprintSpeed;

                        if (!characterSounds.IsPlaying(CharacterSounds.Sound.Sprinting) && !characterSounds.IsPlaying(CharacterSounds.Sound.Sliding) && !characterSounds.IsPlaying(CharacterSounds.Sound.Stealth))
                            characterSounds.PlaySound(CharacterSounds.Sound.Sprinting, 0, false, true);
                        

                        if (Input.GetKeyDown(KeyCode.S) && isSprinting && characterResources.HasStamina)
                        {
                            anim.SetTrigger("slideDown");
                            anim.SetBool("isSliding", true);
                            isSliding = true;

                            if (!characterSounds.IsPlaying(CharacterSounds.Sound.Sliding) && !characterSounds.IsPlaying(CharacterSounds.Sound.Stealth))
                                characterSounds.PlaySound(CharacterSounds.Sound.Sliding, 0, false, true);
                        }
                    }
                    else if (isCrouching)
                        speed = crouchSpeed;
                    else
                    {
                        anim.SetBool("isSprinting", false);                       
                        isSprinting = false;                        
                        speed = walkSpeed;

                        characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                        characterSounds.StopSound(CharacterSounds.Sound.Sprinting);


                        if(isSliding && !isSprinting)
                        {
                            canStandUp = Physics2D.OverlapCircle(ceilingCheck.position, ceilingCheckRadius, whatIsGround);
                            if (canStandUp)
                            {
                                isCrouching = true;
                                anim.SetBool("isCrouching", true);
                                speed = crouchSpeed;
                                isSliding = false;
                                characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                            }
                            else if (!canStandUp)
                            {
                                anim.SetBool("isSliding", false);
                                isSliding = false;
                                characterSounds.StopSound(CharacterSounds.Sound.Sliding);
                            }
                        }
                    }
                }
                // Crouch calculations.
                CrouchCheck();
            }                      
        }

        StealthCheck();

        // Shooting calculations.
        if(!blockInput)
            ShootingCheck();

        // Change the friction of the physicsmaterial of the player based on if he is grounded.
        if (isGrounded)
            physicsmat.friction = 1f;
        else
            physicsmat.friction = 0f;


        if (Input.GetKeyUp(KeyCode.K))
        {
            characterResources.ReduceHealth(20f);
        }

        // If the player was hurt, block his input for a short amount of time and make him invincible for a slightly longer period of time.
        if (damageTaken && IsAlive)
        {
            BlinkPlayer();
            if (damageTakenTimer < inputBlockedTime)
            {
                damageTakenTimer += Time.deltaTime;
            }
            else if (damageTakenTimer < invincibleTime)
            {
                damageTakenTimer += Time.deltaTime;
                blockInput = false;
                //Debug.Log(Mathf.Sin(Time.time));
            }
            else
            {
                damageTaken = false;
                damageTakenTimer = 0f;
                gameObject.layer = 8;
                StopBlinkPlayer();
            }
        }
    }

    /// <summary>
    /// Starts the timer when being outside of the object that lets the player be invisible.
    /// </summary>
    private void StealthCheck()
    {
        if (invisible && !insideBush)
        {
            invisibleCounter += Time.deltaTime;

            OnInvisibleChanged?.Invoke(invisibleCounter/invisibileTime);

            if (invisibleCounter < invisibileTime)
                return;
            
            MakeVisible();
        }
    }

    /// <summary>
    /// Turns the player visible again.
    /// </summary>
    private void MakeVisible()
    {
        if (invisible)
        {
            invisible = false;
            ChangeAlpha(invisible);
            invisibleCounter = 0f;
            stealthSoundHasPlayed = false;
            OnInvisibleChanged?.Invoke(1f);
        }
    }

    /// <summary>
    /// Changes the alpha of all player sprites to make him appear invisible or visible.
    /// </summary>
    /// <param name="invisible"></param>
    private void ChangeAlpha(bool invisible)
    {
        for (int i = 0; i < spritesToBlink.Length; i++)
        {
            if (invisible)
                spritesToBlink[i].color = new Color(spritesToBlink[i].color.r, spritesToBlink[i].color.g, spritesToBlink[i].color.b, 0.5f);
            else
                spritesToBlink[i].color = new Color(spritesToBlink[i].color.r, spritesToBlink[i].color.g, spritesToBlink[i].color.b, 1f);
        }
    }

    private void FixedUpdate()
    {
        // Set the players velocity according to its horizontal input. In case aircontrol is wanted calculate that too.
        if(isGrounded && !blockInput)
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        else if(!isGrounded && !blockInput)
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
            characterSounds.PlaySound(CharacterSounds.Sound.Jump, 0, true, false);
            MakeVisible();
        }
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;

                characterSounds.PlaySound(CharacterSounds.Sound.Jump, 0, true, false);
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
        
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S)) && !isSprinting)
        {
            if (insideBush && canHide)
            {
                invisible = true;
                invisibleCounter = 0f;
                ChangeAlpha(invisible);

                if (!characterSounds.IsPlaying(CharacterSounds.Sound.Stealth) && !stealthSoundHasPlayed)
                {
                    characterSounds.PlaySound(CharacterSounds.Sound.Stealth, 0, false, false);
                    stealthSoundHasPlayed = true;
                }
            }

            isCrouching = true;
            anim.SetBool("isCrouching", true);
        }
        else if (!Input.GetKey(KeyCode.S) && !canStandUp)
        {
            MakeVisible(); 
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        }
    }

    private void ShootingCheck()
    {
        // Only start shooting if hes on the ground and not moving and time passed after his last shot. Set the animation according to active weapon hand.
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded && moveInput == 0 && canShootAgain && characterResources.HasStamina &&  currentMunition[activeWeapon] > 0)
        {
            isShooting = true;
            canShootAgain = false;
            if (activeHand == ActiveWeaponHand.Left)
            {
                characterSounds.PlaySound(CharacterSounds.Sound.RangedAttacking, 0, false, false);
                anim.SetTrigger("tenseBow");
            }
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
            // Spend stamina while the bow is tensed.
            characterResources.ReduceStamina(shootStaminaConsumption * Time.deltaTime);

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
            if (Input.GetKeyUp(KeyCode.LeftControl) && isGrounded || !characterResources.HasStamina || blockInput)
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
                tempProjectile.GetComponent<Projectile>().ShootProjectile(direction.normalized * projectileSpeedMult, Invisible);

                MakeVisible();
                

                if (activeHand == ActiveWeaponHand.Left)
                {
                    characterSounds.PlaySound(CharacterSounds.Sound.Shoot, 0, false, false);
                    anim.SetTrigger("releaseBow");
                }
                else
                    anim.SetTrigger("releaseRightHandShot");
                isShooting = false;
                line.enabled = false;

                // Decrease the munition amount and tell the ui to update;
                currentMunition[activeWeapon]--;
                UIManager.instance.AmmoChanged(currentMunition[activeWeapon].ToString(), activeHand);
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

    //public void RespawnPlayer(Vector2 position)
    //{
    //    // Set the playerstats to default values.
    //    characterResources.RestoreValues();

    //    // Disable the ragdoll system and give the control back to the player.
    //    ragdoll.DisableRagdoll();
    //    blockInput = false;
    //    anim.enabled = true;
    //    gameObject.GetComponent<Collider2D>().enabled = true;
    //    rb.isKinematic = false;
    //    IsAlive = true;

    //    // Set the position of the player.
    //    transform.position = position;
    //}

    public void SetSpawnPosition(float xPos, float yPos)
    {
        playerStats.SpawnPosX = xPos;
        playerStats.SpawnPosY = yPos;
    }

    public void RefeillAllValues()
    {
        // Restock the palyers ammo.
        currentMunition[ActiveWeaponType.ArrowNormal] = playerStats.MaxArrows;
        currentMunition[ActiveWeaponType.SpearNormal] = playerStats.MaxSpears;
        currentMunition[ActiveWeaponType.SpearPlatform] = playerStats.MaxPlatformspears;
        currentMunition[ActiveWeaponType.BombNormal] = playerStats.MaxBombNormal;
        currentMunition[ActiveWeaponType.BombSticky] = playerStats.MaxStickyBomb;
        currentMunition[ActiveWeaponType.BombMega] = playerStats.MaxMegaBomb;

        UIManager.instance.AmmoChanged("", ActiveWeaponHand.Right1);
        UIManager.instance.AmmoChanged(currentMunition[ActiveWeaponType.ArrowNormal].ToString(), ActiveWeaponHand.Left);

        // ToDo: Dont refresh players health.
        // Restock other Values. 
        //characterResources.SetHealth(playerStats.MaxHealth);
        characterResources.SetStamina(playerStats.MaxStamina);
    }

    public void BlockInput(bool block)
    {
        blockInput = block;
        moveInput = 0;
        rb.velocity = Vector3.zero;
        anim.SetBool("isWalking", false);
        anim.SetBool("isJumping", false);
        anim.SetBool("isSliding", false);
        anim.SetBool("isSprinting", false);

        //Test:

        characterSounds.StopAllSounds();
    }

    public void SetKinematic(bool kinematic)
    {
        if (kinematic)
            rb.isKinematic = true;
        else if (!kinematic)
            rb.isKinematic = false;
    }

    public void ApplyRecoil(Vector3 direction, float strength, Vector2 ? pos, bool block)
    {
        BlockInput(block);
        rb.AddForce(direction * strength, ForceMode2D.Impulse);
        damageTaken = true;
        gameObject.layer = 14;
        characterSounds.PlaySound(CharacterSounds.Sound.Hit, 0, true, false);

        if (pos == null)
            pos = transform.position;

        if (blood1 != null)
        {
            blood1.transform.position = (Vector2)pos;
            blood1.Play();
        }
        if (blood2 != null)
        {
            blood2.transform.position = (Vector2)pos;
            blood2.Play();
        }
    }

    public void BlinkPlayer()
    {
        if(blinkTimer < blinkIntervall)
        {
            blinkTimer += Time.deltaTime;
            for (int i = 0; i < spritesToBlink.Length; i++)
            {
                if (swapColors)
                {
                    spritesToBlink[i].color = Color.Lerp(dmgTakenColor1, dmgTakenColor2, blinkTimer / blinkIntervall);
                }
                else
                {
                    spritesToBlink[i].color = Color.Lerp(dmgTakenColor2, dmgTakenColor1, blinkTimer / blinkIntervall);
                }
            }
        }
        else
        {
            blinkTimer = 0;
            swapColors = !swapColors;
        }
    }

    void StopBlinkPlayer()
    {
        blinkTimer = 0;
        for (int i = 0; i < spritesToBlink.Length; i++)
        {
                spritesToBlink[i].color = Color.white;
        }
    }
}
