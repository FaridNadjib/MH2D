using UnityEngine;

public class EnemyAnkylo : Enemy
{
    [Header("Ankylo")]
    [SerializeField] private Vector2 meleeAttackRange;
    [SerializeField] private float meleeAttackPositionOffset;
    [SerializeField] private float rangedAttackRange;
    [SerializeField][Range(0, 100)] private int chanceToShootExplodingTail;
    [SerializeField] private Vector2 minMaxWaitAfterAttackTime;
    private float waitAfterAttackTime;
    [SerializeField] private GameObject projectilePos;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private int collisionDamage;
    [SerializeField] private int collisionStrength;

    private const string explodingTailPool = "ankyloExplodingTailPool";
    private const string normalTailPool = "ankyloTailPool";


    private bool playedSoundOnce = false;


    protected override void Alerted(Collider2D other)
    {
        target.CanHide = false;
        currentState = State.Alerted;
        SetupNextBehaviour();
    }

    protected override void Start()
    {
        SubscribeToEvents();
        SetupNextBehaviour();

        waitAfterAttackTime = GetNextWaitAfterAttackTime();
    }

    private float GetNextWaitAfterAttackTime()
    {
        return UnityEngine.Random.Range(minMaxWaitAfterAttackTime.x, minMaxWaitAfterAttackTime.y);
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

    protected override void CollisionWithPlayer(Collision2D collision)
    {
        Vector3 direction = collision.transform.position - transform.position;
        Vector2 pos = collision.GetContact(0).point;
        collision.gameObject.GetComponent<CharacterResources>().ReduceHealth(collisionDamage);
        collision.gameObject.GetComponent<PlayerController>().ApplyRecoil(direction, collisionStrength, pos, true);
    }

    public override void HasHitPlayer(Collider2D other) 
    { 
        nextPos = transform.position;
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
            anim.SetBool("isWalking", false);
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
            waitAfterAttackTime = GetNextWaitAfterAttackTime();

            anim.SetBool("isWalking", false);

            if (currentState == State.Unalerted)
            {
                if (!playedSoundOnce)
                {
                    if (!characterSounds.IsPlaying(CharacterSounds.Sound.Idle))
                    {
                        characterSounds.PlaySound(CharacterSounds.Sound.Idle, 0, true, false);
                        playedSoundOnce = true;
                    }
                }

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

                // if ankylo is at outer waypoint and the player is out of range, make him only range-attack the player 
                if ((Vector2)transform.position == GetOuterWaypoint(false) && target.transform.position.x < GetOuterWaypoint(false).x 
                    || (Vector2)transform.position == GetOuterWaypoint(true) && target.transform.position.x > GetOuterWaypoint(true).x )
                {
                    currentWaitTime = 0f;
                    anim.SetBool("isAttackingRanged", true);
                    chanceToShootExplodingTail = 100;
                    return;
                }
                else
                {
                    currentWaitTime = 0f;
                    SetupNextBehaviour();
                }
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
        playedSoundOnce = false;

        // patrol to next waypoint
        if (currentState == State.Unalerted)
        {
            targetWaypointIndex = GetNextWaypointIndex();
            nextPos = waypoints[targetWaypointIndex].position;
            anim.SetBool("isWalking", true);
            currentSpeed = standardSpeed;

            if (!characterSounds.IsPlaying(CharacterSounds.Sound.Moving))
                characterSounds.PlaySound(CharacterSounds.Sound.Moving, 0, false, true);
        }
        // charge towards target position 
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
                target.CanHide = true;
            }
            // if player is in melee range charge towards player position + offset
            else if (Mathf.Abs(distance.x) < meleeAttackRange.x && Mathf.Abs(distance.y) < meleeAttackRange.y)
            {
                if (target.transform.position.x < transform.position.x)
                {
                    nextPos = new Vector2(target.transform.position.x - meleeAttackPositionOffset, transform.position.y);

                    if (nextPos.x < GetOuterWaypoint(false).x)
                        nextPos = GetOuterWaypoint(false);
                }
                else
                {
                    nextPos = new Vector2(target.transform.position.x + meleeAttackPositionOffset, transform.position.y);

                    if (nextPos.x > GetOuterWaypoint(true).x)
                    {
                        nextPos = GetOuterWaypoint(true);
                    }
                }

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

    private Vector2 GetOuterWaypoint(bool right)
    {
        Vector2 furthest = waypoints[0].position;

        if (right)
        {
            for (int i = 1; i < waypoints.Length; i++)
            {
                if (waypoints[i].position.x > furthest.x)
                    furthest = waypoints[i].position;
            }
        }
        else
        {
            for (int i = 1; i < waypoints.Length; i++)
            {
                if (waypoints[i].position.x < furthest.x)
                    furthest = waypoints[i].position;
            }
        }
        return furthest;
    }

    public void ShootTail()
    {
        GameObject tail;
        int chance = UnityEngine.Random.Range(0, 99);

        // shoot exploding tail
        if (chance < chanceToShootExplodingTail)
            tail = ObjectPoolsController.instance.GetFromPool(explodingTailPool);
        // shoot normal tail
        else
            tail = ObjectPoolsController.instance.GetFromPool(normalTailPool);

        tail.transform.position = projectilePos.transform.position;
        int offsetX = UnityEngine.Random.Range(0, 8);

        if (transform.position.x > target.transform.position.x)
            offsetX *= -1;

        int offsetY = UnityEngine.Random.Range(5, 15);
        Vector2 direction = new Vector2(target.transform.position.x - transform.position.x + offsetX, target.transform.position.y - transform.position.y + offsetY);
        tail.SetActive(true);
        tail.GetComponent<Projectile>().ShootProjectile(direction.normalized, false);
    }

    protected override void OnDrawGizmosSelected()
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
    }
}