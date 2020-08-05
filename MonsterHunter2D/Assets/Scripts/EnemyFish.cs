using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : Enemy
{
    private float currentJumpIntervalTime;
    private const float fixedWaitAfterJumpTime = 0.5f;
    private float currentWaitAfterJumpTime;
    private bool jump = true;
    private Quaternion startRotation;
    private Vector3 startLocalScale;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        resources = GetComponent<CharacterResources>();

        startRotation = transform.rotation;
        startLocalScale = transform.localScale;

        if (moveBetweenWaypoints || currentState == State.Alerted)
        {
            nextPos = waypoints[targetWaypointIndex].position;
            currentSpeed = standardSpeed;
            FlipTowardsPos(nextPos);
        }
    }

    protected override void UnalertedBehaviour()
    {
        if (!moveBetweenWaypoints)
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
                currentWaitAfterJumpTime = 0f;
            }

            if (rb.velocity != Vector2.zero)
            {
                RotateAndFlip(true);
            }
        }
        else if (moveBetweenWaypoints)
        {
            Patrol();
        }
    }

    protected override void AlertedBehaviour()
    {
        PatrolAndJump();
    }

    private void PatrolAndJump()
    {
        // if the enemy has reached the next position
        if (Vector2.Distance(transform.position, nextPos) < 0.05f)
        {
            transform.rotation = startRotation;
            currentWaitTime += Time.deltaTime;
            currentCurvePos = 0f;

            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * 1, Mathf.Abs(transform.localScale.x) * 1);

            if (CalculateDirectionToPos(nextPos) == -1)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -1, Mathf.Abs(transform.localScale.x) * 1);

            if (currentWaitTime > waitAtWaypointTime)
            {
                targetWaypointIndex = GetNextWaypointIndex();
                nextPos = waypoints[targetWaypointIndex].position;
                currentWaitTime = 0f;
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

        currentWaitAfterJumpTime += Time.deltaTime;

        if (currentWaitAfterJumpTime < fixedWaitAfterJumpTime)
            return;

        rb.gravityScale = 1f;
    }

    private void Patrol()
    {
        // if the enemy has reached the next position
        if (Vector2.Distance(transform.position, nextPos) < 0.05f)
        {
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime > waitAtWaypointTime)
            {
                targetWaypointIndex = GetNextWaypointIndex();
                nextPos = waypoints[targetWaypointIndex].position;
                currentWaitTime = 0f;
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

    /// <summary>
    /// Calculates a third curve point for the enemy to lerp towards to create a curved movement (Bézier-curve).
    /// </summary>
    protected override Vector3 CalculateCurvePoint()
    {
        Vector3 pos = pos = new Vector3((transform.position.x + nextPos.x) * 0.5f, nextPos.y + 10, 0);
        return pos;
    }
}