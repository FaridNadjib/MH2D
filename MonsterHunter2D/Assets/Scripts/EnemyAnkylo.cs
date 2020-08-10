using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnkylo : Enemy
{
    [Header("Ankylo")]
    [SerializeField] private Vector2 meleeAttackRange;
    [SerializeField] private float meleeAttackPositionOffset;
    [SerializeField] private float rangedAttackRange;
    [SerializeField] private float maxRange;
    [SerializeField] private float waitAfterAttackTime;

    protected override void Alerted(Collider2D other)
    {
        currentState = State.Alerted;
        SetupNextBehaviour();
    }

    protected override void Start()
    {
        SubscribeToEvents();
        SetupNextBehaviour();
    }

    protected override void UnalertedBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void AlertedBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void AttackingBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void CheckDistanceToNextPos()
    {
        if (Vector3.Distance(transform.position, nextPos) < 0.5f)
        {
            anim.SetBool("isWalking", false);

            if (currentState == State.Unalerted)
            {
                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAtWaypointTime)
                    return;

                SetupNextBehaviour();
                currentWaitTime = 0f;
            }
            else if (currentState == State.Attacking)
            {
                anim.SetBool("isAttacking", false);

                FlipTowardsPos(target.transform.position);

                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAfterAttackTime)
                    return;

                SetupNextBehaviour();
                currentWaitTime = 0f;
            }
        }
        else
        {
            FlipTowardsPos(nextPos);
            rb.velocity = Vector2.zero;
            Move();
        }
    }

    protected override void SetupNextBehaviour()
    {
        startPos = transform.position;

        if (currentState == State.Unalerted)
        {
            targetWaypointIndex = GetNextWaypointIndex();
            nextPos = waypoints[targetWaypointIndex].position;
            anim.SetBool("isWalking", true);
            currentSpeed = standardSpeed;
        }
        else if (currentState == State.Alerted)
        {
            if (target.transform.position.x < transform.position.x)
                nextPos = new Vector2(target.transform.position.x - meleeAttackPositionOffset, transform.position.y);
            else
                nextPos = new Vector2(target.transform.position.x + meleeAttackPositionOffset, transform.position.y);

            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", true);
            currentSpeed = attackSpeed;
            currentState = State.Attacking;
        }
        else if (currentState == State.Attacking)
        {
            Vector3 distance = transform.position - target.transform.position;

            // if player is in melee range charge towards player position + offset
            if (Mathf.Abs(distance.x) < meleeAttackRange.x && Mathf.Abs(distance.y) < meleeAttackRange.y)
            {
                if (target.transform.position.x < transform.position.x)
                    nextPos = new Vector2(target.transform.position.x - meleeAttackPositionOffset, transform.position.y);
                else
                    nextPos = new Vector2(target.transform.position.x + meleeAttackPositionOffset, transform.position.y);

                anim.SetBool("isAttacking", true);
            }
            // if player too far on x or y 
            else if (Mathf.Abs(distance.x) > meleeAttackRange.x || Mathf.Abs(distance.y) > meleeAttackRange.y)
            {
                // Shoot spikes 
                anim.SetTrigger("isAttackingRanged");         
            }
            // if player out of range 
            else if (Vector3.Distance(transform.position, target.transform.position) > maxRange)
            {
                currentState = State.Unalerted;
            }
        }
    }
}