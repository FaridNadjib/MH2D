using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnake : Enemy
{
    [Header("Snake")]
    [SerializeField] private float meleeAttackRange;
    [SerializeField] private float meleeAttackInterval;
    [SerializeField] private float rangedAttackRange;
    [SerializeField] private float rangedAttackInterval;
    [SerializeField] private GameObject projectilePos;
    private enum AttackType { Melee, SingleRanged, MultipleRanged }
    private AttackType nextAttackType;
    [SerializeField] private int multiAttackProjectileCount;
    private string projectilePool = "snakeVenomPool";

    protected override void Alerted(Collider2D other)
    {
        anim.SetBool("isWakingUp", true);
        characterSounds.PlaySound(CharacterSounds.Sound.Alerted, 0, false, false);
        target = other.GetComponent<PlayerController>();
        currentState = State.Attacking;
        currentWaitTime = Mathf.Infinity;
    }

    protected override void Hit(Collision2D other)
    {
        alertedOnce = true;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        if (playerCol == null)
        {
            playerCol = target.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(this.GetComponent<Collider2D>(), playerCol);
        }

        if (currentState == State.Unalerted)
        {
            anim.SetBool("isWakingUp", true);
            characterSounds.PlaySound(CharacterSounds.Sound.Alerted, 0, false, false);
            lastState = State.Alerted;
        }
        else if (currentState != State.Hit)
            lastState = currentState;

        if (currentState != State.Unalerted)
            characterSounds.PlaySound(CharacterSounds.Sound.Hit, 0, false, false);

        // particles.transform.position = contactPoint;
        // particles.Play();

        anim.SetTrigger("gotDamaged");

        currentWaitTime = 0f;

        if (GetComponent<CharacterResources>().GetCurrentHealth() > 0)
            currentState = State.Hit;
        else 
            currentState = State.Dead;
    }

    protected override void AttackingBehaviour()
    {
        FlipTowardsPos(target.transform.position);

        float distance = Vector2.Distance(transform.position, target.transform.position);

        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= meleeAttackInterval && distance < meleeAttackRange)
        {
            CanHit = true;
            currentWaitTime = 0f;
            anim.SetTrigger("isAttacking");
            nextAttackType = AttackType.Melee;
        }
        else if (currentWaitTime >= rangedAttackInterval && (distance > meleeAttackRange && distance < rangedAttackRange))
        {
            currentWaitTime = 0f;
            anim.SetTrigger("isAttacking");
            nextAttackType = AttackType.SingleRanged;
        }
        else if (distance > rangedAttackRange)
        {
            currentState = State.Unalerted;
            anim.SetBool("isWakingUp", false);
            alertedOnce = false;
        }
    }

    protected override void HitBehaviour()
    {
        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            nextAttackType = AttackType.MultipleRanged;

            anim.SetTrigger("isAttacking");

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > rangedAttackRange)
            {
                currentState = State.Unalerted;
                anim.SetBool("isWakingUp", false);
                alertedOnce = false;
            }
            else if (distance < rangedAttackRange)
                currentState = State.Attacking;
        }
    }

    public void ShootVenom()
    {
        float distance = Vector2.Distance(transform.position, target.transform.position);

        switch (nextAttackType)
        {
            case AttackType.Melee:
                break;

            case AttackType.SingleRanged:
                GameObject tempVenom = ObjectPoolsController.instance.GetFromPool(projectilePool);
                tempVenom.transform.position = projectilePos.transform.position;
                Vector2 direction = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y);
                tempVenom.SetActive(true);
                tempVenom.GetComponent<Projectile>().ShootProjectile(direction.normalized);
            break;

            case AttackType.MultipleRanged:
                for (int i = 0; i < multiAttackProjectileCount; i++)
                {
                    GameObject tempVenom1 = ObjectPoolsController.instance.GetFromPool(projectilePool);
                    tempVenom1.transform.position = projectilePos.transform.position;
                    Vector2 dir = new Vector2(target.transform.position.x - transform.position.x, target.transform.position.y - transform.position.y + i + 0.5f);
                    tempVenom1.SetActive(true);
                    tempVenom1.GetComponent<Projectile>().ShootProjectile(dir.normalized);
                }
            break;
        }
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, meleeAttackRange);

        Gizmos.color = new Color(1, 1, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, rangedAttackRange);
    }
}