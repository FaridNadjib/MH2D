using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(CapsuleCollider2D), typeof(CircleCollider2D), typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] private bool randomNextWaypoint = false;
    [SerializeField] protected int targetWaypointIndex = 0;
    [SerializeField] protected float standardSpeed;
    [SerializeField] protected float attackSpeed;
    [SerializeField] private float waitAtWaypointTime;
    [SerializeField] private float waitAfterHitTime;
    protected float currentWaitTime = 0f;
    protected enum State { Unalerted, Alerted, Attacking, Hit, Dead }
    [SerializeField] protected State activeState;
    private State lastState;
    protected Animator anim;
    protected PlayerController target;
    [SerializeField] protected Vector3 nextPos;
    private Vector3 startPos;
    [SerializeField] private Vector2 minMaxPlayerOffsetX;
    [SerializeField] private Vector2 minMaxPlayerOffsetY;
    [SerializeField] private float attackOffsetX;
    protected bool alertedOnce = false;
    [Tooltip("Should this enemy move between fixed checkpoints or should it move based on the player position?")]
    [SerializeField] private bool moveBetweenCheckpoints = false;
    [SerializeField] private bool moveInRandomCurve = false;

    [SerializeField] protected Vector3 curvePoint;
    [SerializeField] private Vector2 minMaxCurvePointOffsetX;
    [SerializeField] private Vector2 minMaxCurvePointOffsetY;
    [SerializeField] [Range(0, 1)] private float currentTime;

    [SerializeField] private ParticleSystem particles;

    private bool canHit = true;
    public bool CanHit { get => canHit; set => canHit = value; }

    private Vector2 forceDirection;
    protected float currentSpeed;
    private CharacterResources resources;
    protected Rigidbody2D rb;

    private void Awake()
    {
        activeState = State.Unalerted;
        anim = GetComponent<Animator>();
        resources = GetComponent<CharacterResources>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
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

            if (!moveBetweenCheckpoints)
                SetupNextMovement();

            anim.SetBool("isWakingUp", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Projectile>() != null)
        {
            if (activeState == State.Dead)
                return;

            if (!alertedOnce)
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
    }

    /// <summary>
    /// Sets a random or a next waypoint from the waypoints-array.
    /// </summary>
    protected virtual void SetNextWaypoint()
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
    /// Sets a new attack-position with an offset around the player.
    /// </summary>
    protected virtual void SetupNextMovement()
    {
        startPos = transform.position;

        int movement = Random.Range(0, 2);

        if (movement == 1)
            moveInRandomCurve = true;
        else
            moveInRandomCurve = false;

        if (activeState == State.Attacking)
        {
            currentSpeed = attackSpeed;

            nextPos = new Vector3(target.transform.position.x + UnityEngine.Random.Range(-attackOffsetX, attackOffsetX), target.transform.position.y, 0);

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
    /// Moves the enemy to a position based on his current state. If the enemy reaches the position, it waits for a specified duration and starts to attack the player from there.
    /// </summary>
    protected virtual void Move()
    {
        if (transform.position == nextPos || currentTime >= 1f)
        {
            print("reached nextPos");

            if (activeState == State.Alerted)
            {
                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAtWaypointTime)
                    return;

                currentWaitTime = 0f;

                activeState = State.Attacking;

                SetupNextMovement();
                CalculateCurvePoint();
                Flip();
            }
            else if (activeState == State.Attacking)
            {
                activeState = State.Alerted;

                SetupNextMovement();
                CalculateCurvePoint();
                Flip();
            }

            currentTime = 0f;
        }
        else
        {
            rb.velocity = Vector2.zero;

            if (!moveInRandomCurve)
            {
                transform.position = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime * 20);
            }
            else
            {
                MoveInCurve();
            }
        }
    }

    /// <summary>
    /// Calculates a third curve point that the enemy lerps towards to create a curved movement.
    /// </summary>
    private void CalculateCurvePoint()
    {
        float xOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetX.x, minMaxCurvePointOffsetX.y);

        if (CalculateDirectionToPos(target.transform.position) >= 0)
        {
            curvePoint = new Vector3(target.transform.position.x + xOffset, nextPos.y, 0);
        }
        else
        {
            curvePoint = new Vector3(target.transform.position.x - xOffset, nextPos.y, 0);
        }
    }

    /// <summary>
    /// Lerps towards the curve-point and then towards the end-point to create a curved movement.
    /// </summary>
    private void MoveInCurve()
    {
        if (currentTime < 1f)
        {
            currentTime += Time.deltaTime * currentSpeed;

            if (activeState == State.Attacking)
                nextPos = target.transform.position;

            Vector3 lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentTime);
            Vector3 lerptoEndPoint = Vector3.Lerp(curvePoint, nextPos, currentTime);
            transform.position = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentTime);
        }
        else
            currentTime = 0f;
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
            currentTime = 0f;

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

    protected virtual float CalculateDirectionToPos(Vector3 pos)
    {
        float direction;

        if (transform.position.x > pos.x)
            direction = 1;
        else
            direction = -1;

        return direction;
    }

    protected virtual void Flip()
    {
        transform.localScale = new Vector3(CalculateDirectionToPos(target.transform.position), transform.localScale.y, transform.localScale.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        if (moveBetweenCheckpoints)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i].position, 1f);
            }

            Gizmos.DrawLine(transform.position, waypoints[targetWaypointIndex].position);
        }
        else
        {
            Gizmos.DrawSphere(nextPos, 1f);
            Gizmos.DrawLine(transform.position, nextPos);
        }

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(curvePoint, 2f);
    }
}