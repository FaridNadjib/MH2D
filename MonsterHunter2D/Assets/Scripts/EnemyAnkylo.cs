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
    [SerializeField][Range(0, 100)] private int chanceToShootExplodingTail;
    [SerializeField] private float waitAfterAttackTime;
    [SerializeField] private GameObject projectilePos;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private int collisionDamage;
    [Header("Sounds")]
    [SerializeField] private Vector2 soundIntervalRange;
    private float currentInterval;
    private float currentIntervalTime = 0f;


    protected override void Alerted(Collider2D other)
    {
        currentState = State.Alerted;
        SetupNextBehaviour();
    }

    protected override void Start()
    {
        SubscribeToEvents();
        SetupNextBehaviour();

        currentInterval = UnityEngine.Random.Range(soundIntervalRange.x, soundIntervalRange.y);
    }

    protected override void UnalertedBehaviour()
    {
        CheckDistanceToNextPos();

        currentIntervalTime += Time.deltaTime;

        if (currentIntervalTime >= currentInterval)
        {
            currentIntervalTime = 0f;
            currentInterval = UnityEngine.Random.Range(soundIntervalRange.x, soundIntervalRange.y);
            characterSounds.PlaySound(CharacterSounds.Sound.Idle, 0, true, false);
        }
    }

    protected override void AlertedBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void AttackingBehaviour()
    {
        CheckDistanceToNextPos();
    }

    protected override void CollisionWithPlayer(Collision2D other)
    {
        other.gameObject.GetComponent<CharacterResources>().ReduceHealth(collisionDamage);
    }

    protected override void Hit(Collision2D other)
    {
        alertedOnce = true;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (currentState == State.Attacking && anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAnkylosaurusMeleeAttack"))
            return;
        else 
        {
            currentState = State.Hit;
            anim.SetTrigger("gotDamaged");
            characterSounds.PlaySound(CharacterSounds.Sound.Hit, 0, false, false);
            currentWaitTime = 0f;
        }
    }

    protected override void HitBehaviour()
    {
        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            currentWaitTime = 0f;
            currentState = State.Attacking;
            SetupNextBehaviour();
        }
    }

    protected override void CheckDistanceToNextPos()
    {
        if (Mathf.Approximately(Vector3.Distance(transform.position, nextPos), 0))
        {
            anim.SetBool("isWalking", false);

            if (currentState == State.Unalerted)
            {
                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAtWaypointTime)
                    return;

                currentWaitTime = 0f;
                SetupNextBehaviour();
            }
            else if (currentState == State.Attacking)
            {
                dustParticles.Stop();
                CanHit = false;
                anim.SetBool("isAttacking", false);

                FlipTowardsPos(target.transform.position);

                currentWaitTime += Time.deltaTime;

                if (currentWaitTime < waitAfterAttackTime)
                    return;


                currentWaitTime = 0f;
                SetupNextBehaviour();
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
            CanHit = true;
            currentSpeed = attackSpeed;
            currentState = State.Attacking;
            characterSounds.PlaySound(CharacterSounds.Sound.MeleeAttacking, 0, false, false);
            dustParticles.Play();
        }
        else if (currentState == State.Attacking)
        {
            Vector3 distance = transform.position - target.transform.position;

            // if player is dead or out of range
            if (!target.IsAlive || Mathf.Abs(Vector3.Distance(transform.position, target.transform.position)) > rangedAttackRange)
            {
                currentState = State.Unalerted;
                alertedOnce = false;
            }
            // if player is in melee range charge towards player position + offset
            else if (Mathf.Abs(distance.x) < meleeAttackRange.x && Mathf.Abs(distance.y) < meleeAttackRange.y)
            {
                if (target.transform.position.x < transform.position.x)
                    nextPos = new Vector2(target.transform.position.x - meleeAttackPositionOffset, transform.position.y);
                else
                    nextPos = new Vector2(target.transform.position.x + meleeAttackPositionOffset, transform.position.y);

                currentSpeed = attackSpeed;
                anim.SetBool("isAttacking", true);
                CanHit = true;
                characterSounds.PlaySound(CharacterSounds.Sound.MeleeAttacking, 0, false, false);
                dustParticles.Play();
                dustParticles.transform.localScale = gameObject.transform.localScale;
            }
            // if player too far on x or y 
            else if (Mathf.Abs(distance.x) > meleeAttackRange.x || Mathf.Abs(distance.y) > meleeAttackRange.y)
            {
                nextPos = transform.position;
                FlipTowardsPos(target.transform.position);
                anim.SetTrigger("isAttackingRanged");         
            }
        }
    }

    public void ShootTail()
    {
        GameObject tail;
        int chance = UnityEngine.Random.Range(0, 99);

        // shoot exploding tail
        if (chance < chanceToShootExplodingTail)
            tail = ObjectPoolsController.instance.GetFromPool("ankyloExplodingTailPool");
        // shoot normal tail
        else
            tail = ObjectPoolsController.instance.GetFromPool("ankyloTailPool");

        tail.transform.position = projectilePos.transform.position;
        int offsetX = UnityEngine.Random.Range(0, 8);

        if (transform.position.x > target.transform.position.x)
            offsetX *= -1;

        int offsetY = UnityEngine.Random.Range(5, 15);
        Vector2 direction = new Vector2(target.transform.position.x - transform.position.x + offsetX, target.transform.position.y - transform.position.y + offsetY);
        tail.SetActive(true);
        tail.GetComponent<Projectile>().ShootProjectile(direction.normalized);
    }
}