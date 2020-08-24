using UnityEngine;

public class EnemyFish : Enemy
{
    [Header("Fish")]
    [SerializeField] private FishType type;
    public enum FishType { Passive, VerticalJump, CurveJump }
    [SerializeField] protected Vector2 jumpForce;
    [SerializeField] private Vector2 minMaxCurveJumpHeight;
    [SerializeField] protected float waitAfterJumpTime;
    [SerializeField] private float bitingTime;
    [SerializeField] [Range(1, 100)] private int chanceToJump;

    [Header("Sine")]
    [SerializeField] protected float frequency;
    [SerializeField] protected float magnitude;

    private float currentTime;
    private const float fixedWaitAfterJumpTime = 0.5f;
    private bool jump = true;
    private bool biting = false;
    private Quaternion startRotation;
    private Vector3 startLocalScale;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem bubbles;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        resources = GetComponent<CharacterResources>();
        characterSounds = GetComponent<CharacterSounds>();

        startRotation = transform.rotation;
        startLocalScale = transform.localScale;

        if (moveBetweenWaypoints || currentState == State.Alerted)
        {
            nextPos = waypoints[targetWaypointIndex].position;
            currentSpeed = standardSpeed;
            FlipTowardsPos(nextPos);
        }
    }

    protected override void Update() 
    {   
        if (currentState == State.Attacking)
        {
            Bite();
            return;
        }

        switch (type)
        {
            case FishType.Passive:
                Patrol();
                break;
            case FishType.VerticalJump:
                JumpVertical();
                break;
            case FishType.CurveJump:
                PatrolAndJump();
                break;
        }
    }

    /// <summary>
    /// checks if the position on the y is smaller or equal to the startPos. then waits for the given amount of time
    /// </summary>
    private void JumpVertical()
    {
        if (rb.gravityScale == 0f)
            Jump();
        else if (transform.position.y <= startPos.y)
        {
            transform.rotation = startRotation;
            transform.localScale = startLocalScale;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            currentTime = 0f;
            jump = true;
            currentWaitTime = 0f;
        }

        if (rb.velocity != Vector2.zero)
        {
            RotateAndFlip(true);
        }
    }

    /// <summary>
    /// checks if the nextPos has been reached and then decides whether to jump or to swim towards the next waypoint.
    /// </summary>
    private void PatrolAndJump()
    {
        // if the enemy has reached the next position
        if (Mathf.Approximately(Vector3.Distance(transform.position, nextPos), 0))
        {
            transform.rotation = startRotation;
            base.currentWaitTime += Time.deltaTime;
            currentCurvePos = 0f;

            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * 1, Mathf.Abs(transform.localScale.x) * 1);

            if (CalculateDirectionToPos(nextPos) == -1)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, Mathf.Abs(transform.localScale.x) * 1);

            if (base.currentWaitTime > waitAtWaypointTime)
            {
                targetWaypointIndex = GetNextWaypointIndex();
                nextPos = waypoints[targetWaypointIndex].position;
                base.currentWaitTime = 0f;
                startPos = transform.position;

                if (UnityEngine.Random.Range(0, 99) < chanceToJump)
                {
                    currentMovementType = MovementType.Curve;
                    curvePoint = CalculateCurvePoint();
                    currentSpeed = attackSpeed;
                }
                else
                    currentMovementType = MovementType.Straight;
            }
        }
        // if the enemy hasnt reached the next position yet
        else
        {
            FlipTowardsPos(nextPos);
            Move();
        }
    }

    protected override void Move()
    {
        switch (currentMovementType)
        {
            case MovementType.Straight:
                MoveStraight();
                break;
            case MovementType.Curve:
                MoveInCurve();
                break;
            case MovementType.Sine:
                MoveInSineOnY();
                break;
        }
    }

    /// <summary>
    /// moves in a beziér-curve.
    /// </summary>
    protected override void MoveInCurve()
    {
        if (currentCurvePos < 1f)
        {
            currentCurvePos += Time.deltaTime * currentSpeed;

            Vector3 lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentCurvePos);
            Vector3 lerptoEndPoint = Vector3.Lerp(curvePoint, nextPos, currentCurvePos);

            Vector3 combinedLerp = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentCurvePos);

            Vector3 direction = combinedLerp - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (CalculateDirectionToPos(nextPos) == 1)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, Mathf.Abs(transform.localScale.y) * -1);
            else
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, Mathf.Abs(transform.localScale.y) * 1);

            transform.position = combinedLerp;
        }
        else
            currentCurvePos = 0f;
    }

    /// <summary>
    /// jumps into the air vertically by applying a force
    /// </summary>
    private void Jump()
    {
        currentTime += Time.deltaTime;

        if (currentTime < waitAfterJumpTime)
            return;

        if (jump)
        {
            jump = false;
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
        }

        currentWaitTime += Time.deltaTime;

        if (currentWaitTime < fixedWaitAfterJumpTime)
            return;

        rb.gravityScale = 1f;
    }

    /// <summary>
    /// checks if the waypoint has been reached and waits until the timer has been reached.!-- then 
    /// </summary>
    private void Patrol()
    {
        // if the enemy has reached the next position
        if (Vector2.Distance(transform.position, nextPos) < 0.05f)
        {
            base.currentWaitTime += Time.deltaTime;

            if (base.currentWaitTime > waitAtWaypointTime)
            {
                targetWaypointIndex = GetNextWaypointIndex();
                nextPos = waypoints[targetWaypointIndex].position;
                base.currentWaitTime = 0f;
                FlipTowardsPos(nextPos);

                if (possibleMovements.Length > 1)
                    currentMovementType = GetNextMovementType();
            }
        }
        // if the enemy hasnt reached the next position yet
        else
        {
            Move();
        }
    }

    private void MoveInSineOnY()
    {
        Vector3 pos = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime * 20);
        transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
    }

    /// <summary>
    /// if the player has been hit make it stick to it
    /// </summary>
    /// <param name="other"></param>
    public override void HasHitPlayer(Collider2D other)
    {
        characterSounds.PlaySound(CharacterSounds.Sound.MeleeAttacking, 0, false, false);
        transform.SetParent(other.transform);

        // rotate towards player-center
        Vector3 target = other.transform.position; 
        Vector3 objectPos = transform.position;
        target.x = target.x - objectPos.x;
        target.y = target.y - objectPos.y;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        currentState = State.Attacking;
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        currentTime = 0f;
    }

    /// <summary>
    /// reduces the bite timer and if that is reached, make the fish fall down and destroy it after
    /// </summary>
    private void Bite()
    {
        currentTime += Time.deltaTime;

        if (currentTime < bitingTime)
            return;
        
        rb.isKinematic = false;
        rb.simulated = true;
        rb.gravityScale = 1f;

        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        if (biting)
        {
            biting = false;
            Destroy(gameObject, 5f);
        }
    }

    protected override void CollisionWithWater()
    {
        if (type != FishType.Passive)
        {
            characterSounds.PlaySound(CharacterSounds.Sound.Moving, 0, true, false);
        }
    }


    /// <summary>
    /// Calculates a third curve point for the enemy to lerp towards to create a curved movement (Bézier-curve).
    /// </summary>
    protected override Vector3 CalculateCurvePoint()
    {
        float nextJumpHeight = UnityEngine.Random.Range(minMaxCurveJumpHeight.x, minMaxCurveJumpHeight.y);
        Vector3 pos = pos = new Vector3((transform.position.x + nextPos.x) * 0.5f, nextPos.y + nextJumpHeight, 0);
        return pos;
    }
}