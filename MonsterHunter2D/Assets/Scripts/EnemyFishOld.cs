using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFishOld : EnemyOld
{
    private float currentJumpIntervalTime;
    private const float fixedWaitAfterJumpTime = 0.5f;
    private float currentWaitAfterJumpTime;
    private bool jump = true;

    private void Awake() 
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
        resources = GetComponent<CharacterResources>();

        if (moveBetweenWaypoints)
        {
            nextPos = waypoints[targetWaypointIndex].position;
            currentSpeed = standardSpeed;
            FlipTowardsPos(nextPos);
        }

        if (moveInCurve)
        {
            rb.isKinematic = true;
        }
    }

    protected override void Update() 
    {
        if (!moveBetweenWaypoints && !moveInCurve)
        {
            if (rb.gravityScale == 0f)
                Jump();
            else if (transform.position.y <= startPos.y)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
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
        else if (moveBetweenWaypoints && !moveInCurve)
        {
            Patrol();
        }

        if (moveBetweenWaypoints && moveInCurve)
        {
            // UNALERTED = move between the 2 checkpoints around the platform (normal speed + sinus)
            if (activeState == State.Unalerted)
            {
                currentSpeed = standardSpeed;
                Patrol();
            }
            else if (activeState == State.Alerted)
            {
                currentSpeed = attackSpeed;
                Patrol();
            }
            else if (activeState == State.Attacking)
            {
                currentSpeed = attackSpeed;
                MoveInCurve(nextPos);
            }

            // ONTRIGGERENTER: 
            // ALERTED = move between the 2 checkpoints around the platform (attacking speed + straight)

            // ATTACKING = jump between the 2 checkpoints and the curve point over the platform (attacking speed + curve)
        }
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
        if (Vector2.Distance(transform.position, nextPos) < 0.01f)
        {
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime > waitAtWaypointTime)
            {
                if (activeState == State.Alerted)
                {
                    CalculateCurvePoint();
                    SetNextCheckpoint();

                    activeState = State.Attacking;

                }
                else
                {
                    SetNextCheckpoint();
                    FlipTowardsPos(nextPos);
                }

                currentWaitTime = 0f;
            }
        }
        // if the enemy hasnt reached the next position yet
        else
        {
            if (moveInSine)
                MoveInSine();
            else
                MoveStraight();
        }
    }

    protected override void CalculateCurvePoint()
    {
        curvePoint = new Vector3((waypoints[0].position.x + waypoints[1].position.x) * 0.5f, waypoints[targetWaypointIndex].position.y + 10f);
    }
}
