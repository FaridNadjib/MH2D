using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : Enemy
{
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;
    [SerializeField] private bool patrolling;
    private bool jump = false;
    [SerializeField] private Vector2 jumpForce;
    [SerializeField] private float jumpTime;
    private float currentJumpTime;
    [SerializeField] private float waitAfterJumpTime;
    private float currentWaitAfterJumpTime;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void Update() 
    {
        if (patrolling && rb.gravityScale == 0f)
            Jump();
        else if (transform.position.y <= startPos.y)
        {
            transform.rotation = new Quaternion(0,0,0,0);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            currentWaitAfterJumpTime = 0f;
            currentJumpTime = 0f;
        }

        if (rb.velocity != Vector2.zero)
            RotateTowards();
    }

    private void Jump()
    {
        currentWaitAfterJumpTime += Time.deltaTime;

        if (currentWaitAfterJumpTime < waitAfterJumpTime)
            return;

        currentJumpTime += Time.deltaTime;

        jump = true;

        ApplyForce();

        if (currentJumpTime < jumpTime)
            return;

        jump = false;
        rb.gravityScale = 1f;
    }

    private void ApplyForce()
    {
        if (jump)
        {
            rb.AddForce(jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
    }

    // private void Start() 
    // {
    //     nextPos = waypoints[targetWaypointIndex].position;
    //     currentSpeed = standardSpeed;
    // }

    // protected override void Update()
    // {
    //     if (activeState == State.Dead)
    //         return;
    //     else if (activeState == State.Hit)
    //         WaitAfterHit();
    //     else
    //         Move();
    // }

    // protected override void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.tag == "Player" && !alertedOnce)
    //     {
    //         alertedOnce = true;
    //         target = other.GetComponent<PlayerController>();

    //         nextPos = CalculateAttackStartPoint(target.transform.position);
    //         Flip();

    //         currentSpeed = attackSpeed;

    //         activeState = State.Alerted;
    //     }
    // }

    // private Vector2 CalculateAttackStartPoint(Vector2 targetPos)
    // {
    //     Vector2 pos;
    //     float offsetX = UnityEngine.Random.Range(minMaxPlayerOffsetX.x, minMaxPlayerOffsetX.y);

    //     if (CalculateDirectionToPos(targetPos) >= 0)
    //         pos = new Vector2(targetPos.x + offsetX, targetPos.y - 3);
    //     else
    //         pos = new Vector2(targetPos.x - offsetX, targetPos.y - 3);

    //     return pos;
    // }

    // private void OnCollisionEnter2D(Collision2D other) 
    // {
        
    // }

    // protected override void Move()
    // {
    //     if (Vector2.Distance(transform.position, nextPos) < 0.1f)
    //     {
    //         if (activeState == State.Unalerted)
    //         {
    //             SetNextWaypoint();
    //             Flip();
    //         }
    //         else if (activeState == State.Alerted)
    //         {
    //             CalculateCurvePoint();
    //             if (transform.position.x < target.transform.position.x)
    //                 nextPos = new Vector2(target.transform.position.x + (Vector2.Distance(target.transform.position, transform.position)), transform.position.y);
    //             else
    //                 nextPos = new Vector2(target.transform.position.x - (Vector2.Distance(target.transform.position, transform.position)), transform.position.y);

    //             startPos = transform.position;

    //             currentSpeed = attackSpeed;

    //             activeState = State.Attacking;
    //         }
    //     }
    //     else
    //     {
    //         if (activeState == State.Unalerted)
    //             MoveInSinus();
    //         else if (activeState == State.Alerted)
    //             MoveStraight();
    //         else if (activeState == State.Attacking)
    //             MoveInCurve();
    //     }

    //     //RotateTowards();

    // }

    // private void MoveInSinus()
    // {
    //     Vector3 pos = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime * 20);
    //     transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude;
    // }

    // private void MoveStraight()
    // {
    //     transform.position = Vector2.MoveTowards(transform.position, nextPos, currentSpeed * Time.deltaTime);
    // }

    private void RotateTowards()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 180, Vector3.forward);
    }

    // protected override void Flip()
    // {
    //     transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * CalculateDirectionToPos(nextPos), transform.localScale.y);
    // }
}
