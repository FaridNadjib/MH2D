using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[RequireComponent(typeof(CapsuleCollider2D), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class EnemyOld : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] protected bool moveBetweenWaypoints = false;
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] private bool randomNextWaypoint = false;
    [SerializeField] protected float waitAtWaypointTime;
    protected int targetWaypointIndex = 0;
    [Space(3)]

    [Header("Movement")]
    [SerializeField] protected float standardSpeed;
    [SerializeField] protected float attackSpeed;
    [Tooltip("The offset-range from the player position where the enemy will move to after the attack")]
    [SerializeField] private Vector2 minMaxPlayerOffsetX, minMaxPlayerOffsetY;
    [Header("Sine")]
    [SerializeField] protected bool moveInSine;
    [SerializeField] protected float frequency;
    [SerializeField] protected float magnitude;
    [Header("Jump")]
    [SerializeField] protected Vector2 jumpForce;
    [SerializeField] protected float waitAfterJumpTime;
    protected float currentSpeed;
    [Header("Curve")]
    [Tooltip("Should this enemy move in a curve or in straight line?")]
    [SerializeField] protected bool moveInCurve;
    [Tooltip("The offset-range from the player position where the curve point will be created")]
    [SerializeField] private Vector2 minMaxCurvePointOffsetX, minMaxCurvePointOffsetY;
    protected Vector2 curvePoint;
    [Range(0, 1)] private float currentCurvePos;
    protected Vector3 nextPos;
    protected Vector3 startPos;
    [SerializeField] protected float waitAfterHitTime;
    protected float currentWaitTime = 0f;
    [Space(3)]

    [Header("States")]
    [SerializeField] protected State activeState;
    public enum State { Unalerted, Alerted, Attacking, Hit, Dead }
    private State lastState;
    [Space(3)]

    [Header("References")]
    [SerializeField] private ParticleSystem particles;

    protected bool alertedOnce = false;
    private bool canHit = true;
    public bool CanHit { get => canHit; set => canHit = value; }

    protected Animator anim;
    protected PlayerController target;
    private Vector2 forceDirection;
    protected CharacterResources resources;
    protected Rigidbody2D rb;

    private void Awake()
    {
        activeState = State.Unalerted;
        anim = GetComponent<Animator>();
        resources = GetComponent<CharacterResources>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    private void Start() 
    {
        resources.OnUnitDied += () =>
        {
            // Activate the ragdoll and disable movement.
            //blockInput = true;
            activeState = State.Dead;
            //anim.enabled = false;
            //gameObject.GetComponent<Collider2D>().enabled = false;
            //rb.velocity = Vector2.zero;
            //rb.isKinematic = true;
            //ragdoll.EnableRagdoll();
            //IsAlive = false;
            // ToDo: show message on screen to press l to reload last save or esc to open menu.
        };
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (activeState == State.Unalerted || activeState == State.Dead)
            return;
        else if (activeState == State.Hit)
            WaitAfterHit();
        else
            Move();
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !alertedOnce)
        {
            alertedOnce = true;
            target = other.GetComponent<PlayerController>();

            activeState = State.Alerted;

            if (!moveBetweenWaypoints)
                SetupNextMovement();

            anim.SetBool("isWakingUp", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Projectile>() != null && activeState != State.Dead)
        {
            Hit(other);
        }
    }

    private void Hit(Collision2D other)
    {
        alertedOnce = true;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (activeState == State.Unalerted)
        {
            lastState = State.Attacking;
            SetupNextMovement();
            CalculateCurvePoint();
        }
        else if (activeState != State.Hit)
            lastState = activeState;

        //int damage = other.gameObject.GetComponent<Projectile>().damageValue;
        float damage = 5f;
        resources.ReduceHealth(damage);

        if (anim.GetBool("isWakingUp") == false)
        {
            anim.SetBool("isWakingUp", true);
        }

        Vector2 contactPoint = other.GetContact(0).point;

        // particles.Play();
        // particles.transform.position = contactPoint;

        forceDirection = contactPoint - (Vector2)transform.position;

        anim.SetTrigger("gotDamaged");

        activeState = State.Hit;
    }

    /// <summary>
    /// Sets a random or next waypoint from the waypoints-array.
    /// </summary>
    protected virtual void SetNextCheckpoint()
    {
        if (randomNextWaypoint)
            targetWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
        else if (!randomNextWaypoint && targetWaypointIndex < waypoints.Length - 1)
            targetWaypointIndex++;
        else if (!randomNextWaypoint && targetWaypointIndex >= waypoints.Length - 1)
            targetWaypointIndex = 0;

        nextPos = waypoints[targetWaypointIndex].position;
    }

    /// <summary>
    /// Sets a new position based on the active state.
    /// </summary>
    public virtual void SetupNextMovement()
    {
        startPos = transform.position;

        int movement = Random.Range(0, 2);

        if (movement == 1)
            moveInCurve = true;
        else
            moveInCurve = false;

        if (activeState == State.Attacking)
        {
            currentSpeed = attackSpeed;

            //nextPos = target.transform.position;

            anim.SetBool("isAttacking", true);

            canHit = true;
        }
        else if (activeState == State.Alerted)
        {
            currentSpeed = standardSpeed;

            float offsetX = UnityEngine.Random.Range(minMaxPlayerOffsetX.x, minMaxPlayerOffsetX.y);
            float offsetY = UnityEngine.Random.Range(minMaxPlayerOffsetY.x, minMaxPlayerOffsetY.y);
            nextPos = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, 0);

            anim.SetBool("isAttacking", false);
        }

    }

    /// <summary>
    /// Moves the enemy to a position based on his current state. If the enemy reaches the next position the state will be changed.
    /// </summary>
    protected virtual void Move()
    {
        if (transform.position == nextPos || currentCurvePos >= 1f)
        {
            if (activeState == State.Alerted)
            {
                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAtWaypointTime)
                    return;

                currentWaitTime = 0f;

                activeState = State.Attacking;

                SetupNextMovement();
                CalculateCurvePoint();
                FlipTowardsPos(target.transform.position);
            }
            else if (activeState == State.Attacking)
            {
                activeState = State.Alerted;

                SetupNextMovement();
                CalculateCurvePoint();
                FlipTowardsPos(target.transform.position);
            }

            currentCurvePos = 0f;
        }
        else
        {
            // if (currentTime <= 0.8f && activeState == State.Attacking)
            //     nextPos = target.transform.position;

            rb.velocity = Vector2.zero;

            if (!moveInCurve)
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime * 30);
            }
            else
            {
                MoveInCurve(target.transform.position);
            }
        }
    }

    /// <summary>
    /// Calculates a third curve point for the enemy to lerp towards to create a curved movement.
    /// </summary>
    protected virtual void CalculateCurvePoint()
    {
        float xOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetX.x, minMaxCurvePointOffsetX.y);
        float yOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetY.x, minMaxCurvePointOffsetY.y);

        if (CalculateDirectionToPos(target.transform.position) >= 0)
            curvePoint = new Vector3(target.transform.position.x + xOffset, nextPos.y + yOffset, 0);
        else
            curvePoint = new Vector3(target.transform.position.x - xOffset, nextPos.y + yOffset, 0);
    }

    /// <summary>
    /// Waits a specified duration after being hit and sets the last saved state before being hit as the active state.
    /// </summary>
    protected virtual void WaitAfterHit()
    {
        rb.AddForce(forceDirection.normalized * Time.deltaTime);

        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            activeState = lastState;
            rb.velocity = Vector2.zero;
            currentWaitTime = 0f;
            currentCurvePos = 0f;

            if (resources.GetCurrentHealth() <= 0)
            {
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.None;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isWakingUp", false);
                activeState = State.Dead;
            }
            else
                SetupNextMovement();
        }
    }

    #region Movement-Types

        /// <summary>
        /// Lerps towards the curve-point and then towards the end-point to create a curved movement.
        /// </summary>
        protected void MoveInCurve(Vector3 pos)
        {
            if (currentCurvePos < 1f)
            {
                currentCurvePos += Time.deltaTime * currentSpeed;

                if (activeState == State.Attacking)
                    nextPos = pos;

                Vector3 lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentCurvePos);
                Vector3 lerptoEndPoint = Vector3.Lerp(curvePoint, nextPos, currentCurvePos);
                transform.position = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentCurvePos);
            }
            else
                currentCurvePos = 0f;
        }

        protected void MoveInSine()
        {
            Vector3 pos = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime * 20);
            transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
        }

        protected void MoveStraight()
        {
            transform.position = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime);
        }

    #endregion

    #region Direction, Rotation, Flip

        protected virtual float CalculateDirectionToPos(Vector3 pos)
        {
            float direction;

            if (transform.position.x > pos.x)
                direction = 1;
            else
                direction = -1;

            return direction;
        }

        /// <summary>
        /// Rotates and flips the enemy towards its velocity.
        /// </summary>
        /// <param name="flip">Should the object also be flipped on the localScale.x?</param>
        protected virtual void RotateAndFlip(bool flip)
        {
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            if (transform.localScale.x > 0)
                transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
            else
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (flip)
            {
                if (rb.velocity.y < 0)
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, transform.localScale.y);
                else
                    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * 1, transform.localScale.y);
            }
        }

        /// <summary>
        /// Flips the enemy towards a position on the localScale.x.
        /// </summary>
        /// <param name="pos">The refernce position to flip towards</param>
        protected virtual void FlipTowardsPos(Vector3 pos)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * CalculateDirectionToPos(pos), transform.localScale.y, transform.localScale.z);
        }
    #endregion

    #region Gizmo-Display
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 1f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nextPos, 1f);
        Gizmos.DrawLine(transform.position, nextPos);
    
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(curvePoint, 1f);
    }
#endregion
}