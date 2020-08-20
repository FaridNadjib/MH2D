using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the base player projectile behaviour. 
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("The projectiles type and its speed:")]
    [SerializeField] ActiveWeaponType type;
    public ActiveWeaponType Type { get => type; }
    [SerializeField] public float projectileSpeed;
    float currentDamage;
    [SerializeField] protected float normalDamage;
    [SerializeField] float stealthDamage;

    [Header("The lifetime of the projectile:")]
    [SerializeField] float projectileLifeTime;
    [SerializeField] float disappearTime;
    float lifeTimeCounter;
    float disappearTimeCounter;

    protected Rigidbody2D rb;
    HingeJoint2D joint;
    TrailRenderer trail;

    [Header("Bomb related:")]
    [SerializeField] GameObject bombExplosion;

    [Header("Sounds")]
    AudioClip flyingSound;
    AudioClip collisionSound;

    // Change angle of projectile while it hasnt hit anything.
    bool hasHit;
    // Ensure that the projectile creates a joint connection only once on collision.
    protected bool onlyOnce = false;

    // In some cases we may want to change color or alpha of our projectiles.
    SpriteRenderer image;
    Color startColor;
    Color endColor;

    // Platformspear related.
    PlatformEffector2D platformEffector;
    BoxCollider2D platformEffectorBoxCol;

    /// <summary>
    /// Initialize some projectile values. (Maybe we do this in Start())
    /// </summary>
    private void Awake()
    {
        // Get all the components.
        rb = GetComponent<Rigidbody2D>();
        joint = GetComponent<HingeJoint2D>();
        if(joint != null)
            joint.enabled = false;

        // Save the colors to be able to fade out the projectile later.
        image = GetComponent<SpriteRenderer>();
        startColor = image.color;
        endColor = new Color(startColor.r, startColor.g, startColor.b, 0);

        lifeTimeCounter = 0;

        // Platformspear related.
        if(Type == ActiveWeaponType.SpearPlatform)
        {
            platformEffector = GetComponent<PlatformEffector2D>();
            platformEffectorBoxCol = GetComponent<BoxCollider2D>();
        }
        //platformEffector = GetComponent<PlatformEffector2D>();
        //if (platformEffector != null)
        //    platformEffectorBoxCol = GetComponent<BoxCollider2D>();

        trail = GetComponent<TrailRenderer>();
    }

    /// <summary>
    /// Everytime these projectiles are enabled(since they are stored in a pool) reset some values to default ones.
    /// </summary>
    protected virtual void OnEnable()
    {
        Setup();
    }

    protected void Setup()
    {
        lifeTimeCounter = 0f;
        disappearTimeCounter = 0f;
        image.color = startColor;
        image.enabled = true;

        if (joint != null)
        {
            joint.enabled = false;
            rb.freezeRotation = true;
        }
        onlyOnce = false;

        hasHit = false;
        SetProjectileLayer();

        rb.isKinematic = false;

        if (Type == ActiveWeaponType.SpearPlatform)
        {
            platformEffector.enabled = false;
            platformEffectorBoxCol.enabled = false;
        }

        if (gameObject.GetComponent<Collider2D>() != null)
        {
            gameObject.GetComponent<Collider2D>().enabled = true;
        }

        if (Type == ActiveWeaponType.BombMega || Type == ActiveWeaponType.BombSticky)
        {
            if (bombExplosion != null)
                bombExplosion.SetActive(false);
        }

        if (trail != null)
            trail.emitting = true;
    }

    protected virtual void SetProjectileLayer()
    {
        gameObject.layer = 9;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RotateTowardsDirection();
        FadeOut();
    }

    /// <summary>
    /// As long as no collision appeared make the projectile rotate towards its flying direction.
    /// </summary>
    protected void RotateTowardsDirection()
    {
        if (!hasHit)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        }
    }

    /// <summary>
    /// Make the projectile  disappear after some time and finally give it back to pool once its dead.
    /// </summary>
    protected void FadeOut()
    {
        if (lifeTimeCounter < projectileLifeTime - disappearTime)
            lifeTimeCounter += Time.deltaTime;
        else if (lifeTimeCounter < projectileLifeTime)
        {
            disappearTimeCounter += Time.deltaTime;
            image.color = Color.Lerp(startColor, endColor, disappearTimeCounter / disappearTime);
            lifeTimeCounter += Time.deltaTime;

            if(Type == ActiveWeaponType.BombMega || Type == ActiveWeaponType.BombSticky)
            {
                if(bombExplosion != null)
                {
                    bombExplosion.SetActive(true);
                    image.enabled = false;
                }
            }
        }
        else
            AddToPool();
    }

    protected virtual void AddToPool()
    {
        ObjectPoolsController.instance.AddToPool(gameObject, Type.ToString());
    }

    /// <summary>
    /// Check if an collision occured and activate the hingejoint2D for spears and arrow types,
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        hasHit = true;

        SetAsChildOfCharacter(collision);

        // The stickybomb sticks to the object it collided with.
        //if (!onlyOnce && type == ActiveWeaponType.BombSticky)
        //{
        //    if (collision.gameObject.GetComponent<ProjectilesWillBounceFromMe>() != null)
        //    {

        //    }
        //    else
        //    {
        //        //rb.velocity = Vector2.zero;
        //        rb.isKinematic = true;
        //        rb.velocity = Vector2.zero;
        //        rb.freezeRotation = true;
        //        //transform.SetParent(collision.gameObject.transform);
        //        onlyOnce = true;
        //    }
        //    Debug.Log("Gotcalled");

        //}

        ConnectToCollisionObject(collision);

        SetGroundLayer();
    }

    protected virtual void ConnectToCollisionObject(Collision2D collision)
    {
        // For spear and arrow types, make them connect via joint with the collision object.
        if (!onlyOnce && joint != null)
        {
            if (collision.gameObject.GetComponent<ProjectilesWillBounceFromMe>() != null)
            {
                // Gameobjects with that empty script attached to them wont allow projectiles to create joints with them.
                //rb.isKinematic = true;
                //transform.SetParent(collision.gameObject.transform);
                //gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            }
            else
            {
                joint.anchor = transform.InverseTransformPoint(collision.contacts[0].point);
                //joint.connectedBody = collision.gameObject.GetComponent<Rigidbody2D>();
                joint.connectedAnchor = collision.transform.InverseTransformPoint(collision.contacts[0].point);
                //transform.SetParent(collision.gameObject.transform);
                joint.enabled = true;

                if (Type == ActiveWeaponType.SpearPlatform)
                {
                    rb.isKinematic = true;
                    rb.velocity = Vector2.zero;
                    gameObject.GetComponent<PlatformEffector2D>().enabled = true;
                    gameObject.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            

            rb.freezeRotation = false;
            onlyOnce = true;
        }
    }

    // Set the layer to default so the playercan collide with the projectiles again.
    protected virtual void SetGroundLayer()
    {
        gameObject.layer = 13;
    }

    /// <summary>
    /// On collision with an enemy the rigidbody and collider will be disabled and the projectile will become child of the enemy.
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void SetAsChildOfCharacter(Collision2D collision)
    {

        if (!onlyOnce && collision.gameObject.GetComponent<Enemy>() != null || !onlyOnce && Type == ActiveWeaponType.BombSticky)
        {
            transform.SetParent(collision.gameObject.transform);
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
            onlyOnce = true;
            if(collision.gameObject.GetComponent<Enemy>() != null)
                collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(currentDamage);

            if (gameObject.GetComponent<Collider2D>() != null)
            {
                gameObject.GetComponent<Collider2D>().enabled = false;
            }

            if(Type == ActiveWeaponType.BombSticky)
            {
                //rb.freezeRotation = true;
                rb.angularVelocity = 0f;
            }
            if (trail != null)
                trail.emitting = false;
        }
    }

    /// <summary>
    /// Adds a force the the projectile.
    /// </summary>
    /// <param name="direction">The direction of the force that should be applied.</param>
    public void ShootProjectile(Vector2 direction, bool stealth)
    {
        if (stealth)
            currentDamage = stealthDamage;
        else
            currentDamage = normalDamage;

        rb.AddForce(direction * projectileSpeed, ForceMode2D.Impulse);
    }
}
