using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterResources), typeof(Animator), typeof(CharacterSounds))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] protected bool moveBetweenWaypoints = false;
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] private bool randomNextWaypoint = false;
    [SerializeField] protected float waitAtWaypointTime;
    protected int targetWaypointIndex = 0;
    [Space(3)]

    [Header("Movement")]
    [SerializeField] protected MovementType[] possibleMovements;
    [SerializeField] protected MovementType currentMovementType;
    public enum MovementType { Straight, Curve, Sine }
    [SerializeField] protected float standardSpeed;
    [SerializeField] protected float attackSpeed;
    protected float currentSpeed;
    [Tooltip("The offset-range from the player position where the enemy will move to after the attack")]
    [SerializeField] private Vector2 minMaxPlayerOffsetX, minMaxPlayerOffsetY;
    [SerializeField] private float minDistanceToAlwaysHitPlayer;

    [Header("Curve")]
    [Tooltip("The offset-range from the player position where the curve point will be created")]
    [SerializeField] private Vector2 minMaxCurvePointOffsetX;
    [SerializeField] private Vector2 minMaxCurvePointOffsetY;
    protected Vector2 curvePoint;
    [Range(0, 1)] protected float currentCurvePos = 0f;
    protected Vector3 nextPos;
    protected Vector3 startPos;
    [SerializeField] protected float waitAfterHitTime;
    protected float currentWaitTime = 0f;
    [Space(3)]

    [Header("States")]
    [SerializeField] protected State currentState;
    public enum State { Unalerted, Alerted, Attacking, Hit, Dead }
    protected State lastState;
    [Space(3)]

    private bool willHitTarget = false;
    protected bool alertedOnce = false;
    private bool canHit = true;
    public bool CanHit { get => canHit; set => canHit = value; }

    private bool hasHit = false;
    public bool HasHit { get => hasHit; set => hasHit = value; }
    protected bool hit;

    protected Animator anim;
    protected PlayerController target;
    protected Collider2D playerCol;
    protected Vector2 forceDirection;
    protected CharacterResources resources;
    protected Rigidbody2D rb;
    protected RagdollController ragdoll;
    protected CharacterSounds characterSounds;

    private void Awake()
    {
        currentState = State.Unalerted;
        anim = GetComponent<Animator>();
        resources = GetComponent<CharacterResources>();
        rb = GetComponent<Rigidbody2D>();
        ragdoll = GetComponent<RagdollController>();
        startPos = transform.position;
        characterSounds = GetComponent<CharacterSounds>();
    }

    protected virtual void Start()
    {
        SubscribeToEvents();
    }

    protected void SubscribeToEvents()
    {
        resources.OnUnitDied += () =>
        {
            // Activate the ragdoll and disable movement.
            //blockInput = true;
            if (target == null)
                target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

            target.CanHide = true;
            characterSounds.PlaySound(CharacterSounds.Sound.Dead, 0, false, false);
            currentState = State.Dead;
            anim.enabled = false;
            gameObject.GetComponent<Collider2D>().enabled = false;
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
            ragdoll.EnableRagdoll();
            currentState = State.Dead;
            canHit = false;
            //IsAlive = false;
            // ToDo: show message on screen to press l to reload last save or esc to open menu.
        };
    }

    protected virtual void Update()
    {        
        switch (currentState)
        {
            case State.Unalerted:
                UnalertedBehaviour();
                break;
            case State.Alerted:
                AlertedBehaviour();
                break;
            case State.Attacking:
                AttackingBehaviour();
                break;
            case State.Hit:
                break;
            case State.Dead:
                DeadBehaviour();
                break;
        }
    }

    private void FixedUpdate() 
    {
        if (currentState == State.Hit)
            HitBehaviour();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // if (other.GetComponent<PlayerController>() != null && !alertedOnce && currentState != State.Dead)
        // {
        //     if (other.GetComponent<PlayerController>().Invisible)
        //         return;

        //     alertedOnce = true;

        //     if (target == null)
        //         target = other.GetComponent<PlayerController>();

        //     Alerted(other);
        // }
        // else 
        if (other.GetComponent<BuoyancyEffector2D>() != null && currentState != State.Dead)
            CollisionWithWater();
    }

    private void OnTriggerStay2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && currentState != State.Dead)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player.Invisible && currentState == State.Unalerted && this is EnemyAnkylo || this is EnemySnake)
            {
                Physics2D.IgnoreCollision(other, GetComponent<CapsuleCollider2D>(), true);
                return;
            }
            else if (this is EnemyAnkylo || this is EnemySnake)
            {
                player.CanHide = false;
                Physics2D.IgnoreCollision(other, GetComponent<CapsuleCollider2D>(), false);
            }

            //alertedOnce = true;

            if (target == null)
                target = player;

            //Alerted(other);
        }
        
    }

    public void PlayerInRange(Collider2D other)
    {
        if (!alertedOnce && currentState != State.Dead)
        {
            alertedOnce = true;

            PlayerController player = other.GetComponent<PlayerController>();
            if (player.Invisible)
            {
                Physics2D.IgnoreCollision(other, GetComponent<CapsuleCollider2D>(), true);
                return;
            }

            alertedOnce = true;

            if (target == null)
                target = player;

            Alerted(other);
        }

    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<BuoyancyEffector2D>() != null && currentState != State.Dead)
            CollisionWithWater();
    }

    protected virtual void CollisionWithWater(){}

    protected virtual void Alerted(Collider2D other){}

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Projectile>() != null && other.gameObject.GetComponent<EnemyProjectile>() == null && currentState != State.Dead)
            Hit(other);
        else if (other.gameObject.GetComponent<PlayerController>() != null && currentState != State.Dead)   
            CollisionWithPlayer(other);
    }

    protected virtual void UnalertedBehaviour(){}

    protected virtual void AlertedBehaviour(){}

    protected virtual void AttackingBehaviour(){}

    /// <summary>
    /// Waits a specified duration after being hit and sets the last saved state before being hit as the active state.
    /// </summary>
    protected virtual void HitBehaviour()
    {

        rb.AddForce(forceDirection.normalized * Time.fixedDeltaTime);

        print("Time: " + currentWaitTime + "\n velocity: " + rb.velocity);

        currentWaitTime += Time.fixedDeltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            currentState = lastState;
            rb.velocity = Vector2.zero;
            currentWaitTime = 0f;
            currentCurvePos = 0f;
            startPos = transform.position;

            if (resources.GetCurrentHealth() <= 0)
            {
                rb.gravityScale = 1f;
                rb.constraints = RigidbodyConstraints2D.None;
                anim.SetBool("isAttacking", false);
                anim.SetBool("isWakingUp", false);
                currentState = State.Dead;
                target.CanHide = true;
            }
            else
                SetupNextBehaviour();
        }
    }

    protected virtual void DeadBehaviour(){}

    protected virtual void Hit(Collision2D other){}

    protected virtual void CollisionWithPlayer(Collision2D collision){}

    public virtual void HasHitPlayer(Collider2D other){}

    /// <summary>
    /// Returns a random or next waypoint-index from the waypoints-array
    /// </summary>
    protected int GetNextWaypointIndex()
    {
        int index = 0;

        if (randomNextWaypoint)
            index = UnityEngine.Random.Range(0, waypoints.Length);
        else if (targetWaypointIndex < waypoints.Length - 1)
            index = targetWaypointIndex + 1;
        else if (targetWaypointIndex >= waypoints.Length - 1)
            index = 0;

        return index;
    }

    protected virtual void CheckDistanceToNextPos()
    {
        if (Mathf.Approximately(Vector3.Distance(transform.position, nextPos), 0))
        {
            print("reached nextPos");

            if (currentState == State.Alerted)
            {
                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAtWaypointTime)
                    return;

                currentWaitTime = 0f;
            }

            SetupNextBehaviour(); 
        }
        else
        {
            FlipTowardsPos(nextPos);
            rb.velocity = Vector2.zero;
            Move();
        }
    }

    protected virtual void SetupNextBehaviour()
    {
        if (possibleMovements.Length >= 1)
            currentMovementType = GetNextMovementType();

        print(currentMovementType);

        if (currentMovementType == MovementType.Curve)
            curvePoint = CalculateCurvePoint();

        startPos = transform.position;
        currentCurvePos = 0f;

        if (currentState == State.Attacking)
        {
            float offsetX = UnityEngine.Random.Range(minMaxPlayerOffsetX.x, minMaxPlayerOffsetX.y);
            float offsetY = UnityEngine.Random.Range(minMaxPlayerOffsetY.x, minMaxPlayerOffsetY.y);
            nextPos = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, 0);
            willHitTarget = false;
            currentSpeed = standardSpeed;
            characterSounds.PlaySound(CharacterSounds.Sound.Moving, 0, true, true);
            anim.SetBool("isAttacking", false);
            canHit = false;
            currentState = State.Alerted;
        }
        else if (currentState == State.Alerted)
        {
            nextPos = target.transform.position;

            if (Vector3.Distance(startPos, nextPos) >= minDistanceToAlwaysHitPlayer)
                willHitTarget = true;

            currentSpeed = attackSpeed;
            characterSounds.PlaySound(CharacterSounds.Sound.MeleeAttacking, 0, true, false);
            anim.SetBool("isAttacking", true);
            canHit = true;
            currentState = State.Attacking;
        }
    }

    protected virtual void Move()
    {
        if (willHitTarget)
            nextPos = target.transform.position;

        switch (currentMovementType)
        {
            case MovementType.Straight:
                MoveStraight();
                break;
            case MovementType.Curve:
                MoveInCurve();
                break;
        }
    }

    protected MovementType GetNextMovementType()
    {
        MovementType moveType = possibleMovements[Random.Range(0, possibleMovements.Length)];
        return moveType;
    }

    /// <summary>
    /// Calculates a third curve point for the enemy to lerp towards to create a curved movement (Bézier-curve).
    /// </summary>
    protected virtual Vector3 CalculateCurvePoint()
    {
        Vector3 pos = new Vector3();

        float xOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetX.x, minMaxCurvePointOffsetX.y);
        float yOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetY.x, minMaxCurvePointOffsetY.y);

        if (CalculateDirectionToPos(target.transform.position) >= 0)
            pos = new Vector3(target.transform.position.x + xOffset, nextPos.y + yOffset, 0);
        else
            pos = new Vector3(target.transform.position.x - xOffset, nextPos.y + yOffset, 0);

        return pos;
    }

#region Movement-Types
    /// <summary>
    /// Lerps towards the curve-point and then towards the end-point to create a curved movement (Bézier-curve).
    /// </summary>
    protected virtual void MoveInCurve()
    {
        if (currentCurvePos < 1f)
        {
            currentCurvePos += Time.deltaTime * currentSpeed;

            Vector3 lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentCurvePos);
            Vector3 lerptoEndPoint = Vector3.Lerp(curvePoint, nextPos, currentCurvePos);
            transform.position = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentCurvePos);
        }
        else
            currentCurvePos = 0f;
    }

    protected void MoveStraight()
    {
        transform.position = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * 20 * Time.deltaTime);
    }
#endregion

#region Direction, Rotation, Flip

    protected float CalculateDirectionToPos(Vector3 pos)
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
    protected void RotateAndFlip(bool flip)
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
    protected void FlipTowardsPos(Vector3 pos)
    {
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * CalculateDirectionToPos(pos), transform.localScale.y, transform.localScale.z);
    }
#endregion

#region Gizmo-Display
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        if (waypoints != null)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i].position, 1f);
            }
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(nextPos, 1f);
        Gizmos.DrawLine(transform.position, nextPos);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(curvePoint, 1f);
    }
#endregion

    public void PlayTriggerSound()
    {
        characterSounds.PlaySound(CharacterSounds.Sound.Trigger, 0, false, false);
    }
}