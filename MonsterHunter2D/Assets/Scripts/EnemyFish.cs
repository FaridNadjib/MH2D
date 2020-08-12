using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : Enemy
{
    [Header("Fish")]
    [SerializeField] private FishType type;
    public enum FishType { Passive, VerticalJump, CurveJump }
    [SerializeField] protected Vector2 jumpForce;
    [SerializeField] private float curveJumpHeight;
    [SerializeField] protected float waitAfterJumpTime;

    [Header("Sine")]
    [SerializeField] protected float frequency;
    [SerializeField] protected float magnitude;

    private float currentJumpIntervalTime;
    private const float fixedWaitAfterJumpTime = 0.5f;
    private bool jump = true;
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
            return;

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
            currentJumpIntervalTime = 0f;
            jump = true;
            currentWaitTime = 0f;
        }

        if (rb.velocity != Vector2.zero)
        {
            RotateAndFlip(true);
        }
    }

    private void PatrolAndJump()
    {
        // if the enemy has reached the next position
        if (Vector2.Distance(transform.position, nextPos) < 0.05f)
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

                if (UnityEngine.Random.Range(0, 9) <= 6)
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

    private void Jump()
    {
        currentJumpIntervalTime += Time.deltaTime;

        if (currentJumpIntervalTime < waitAfterJumpTime)
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

    public override void HasHitPlayer(Collider2D other)
    {
        characterSounds.PlaySound(CharacterSounds.Sound.MeleeAttacking, 0, false, false);
        currentState = State.Attacking;
        transform.SetParent(other.transform);
        transform.right = other.transform.position - transform.position;
        rb.isKinematic = true;
        rb.simulated = false;
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 5f);
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
        Vector3 pos = pos = new Vector3((transform.position.x + nextPos.x) * 0.5f, nextPos.y + curveJumpHeight, 0);
        return pos;
    }
}