using UnityEngine;

public class EnemyFlyingDino : Enemy
{
    protected override void Start()
    {
        SubscribeToEvents();
        SetupNextBehaviour();
    }

    protected override void Alerted(Collider2D other)
    {
        if (playerCol == null)
        {
            playerCol = target.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), playerCol);
        }

        target.CanHide = false;

        currentState = State.Attacking;
        nextPos = transform.position;
    }

    protected override void Hit(Collision2D other)
    {
        alertedOnce = true;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (playerCol == null)
        {
            playerCol = target.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), playerCol);
        }

        if (currentState == State.Unalerted)
        {
            anim.SetBool("isWakingUp", true);
            lastState = State.Alerted;
        }
        else if (currentState != State.Hit)
            lastState = currentState;

        Vector2 contactPoint = other.GetContact(0).point;

        // particles.Play();
        // particles.transform.position = contactPoint;

        forceDirection = contactPoint - (Vector2)transform.position;

        characterSounds.PlaySound(CharacterSounds.Sound.Hit, 0, true, false);

        anim.SetTrigger("gotDamaged");

        currentState = State.Hit;
    }

    protected override void AlertedBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void AttackingBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void DeadBehaviour()
    {
        //fall to ground and activate ragdoll
    }

    protected override void UnalertedBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void CheckDistanceToNextPos()
    {
        if (Mathf.Approximately(Vector3.Distance(transform.position, nextPos), 0))
        {
            //anim.SetBool("isWalking", false);

            if (currentState == State.Unalerted)
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

    protected override void SetupNextBehaviour()
    {
        if (possibleMovements.Length >= 1)
            currentMovementType = GetNextMovementType();

        if (currentMovementType == MovementType.Curve)
            curvePoint = CalculateCurvePoint();

        startPos = transform.position;
        currentCurvePos = 0f;

        // patrol to next waypoint
        if (currentState == State.Unalerted)
        {
            targetWaypointIndex = GetNextWaypointIndex();
            nextPos = waypoints[targetWaypointIndex].position;
            anim.SetBool("isAttacking", false);
            currentSpeed = standardSpeed;

            // if (!characterSounds.IsPlaying(CharacterSounds.Sound.Moving))
            //     characterSounds.PlaySound(CharacterSounds.Sound.Moving, 0, false, true);
        }
        else if (currentState == State.Attacking)
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



}