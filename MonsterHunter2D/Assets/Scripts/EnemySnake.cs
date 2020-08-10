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

    protected override void Alerted(Collider2D other)
    {
        anim.SetBool("isWakingUp", true);
        // play sound
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
            lastState = State.Alerted;
        }
        else if (currentState != State.Hit)
            lastState = currentState;

        // particles.Play();
        // particles.transform.position = contactPoint;

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
            anim.SetTrigger("isMeleeAttacking");
        }
        else if (currentWaitTime >= rangedAttackInterval && (distance > meleeAttackRange && distance < rangedAttackRange))
        {
            currentWaitTime = 0f;
            anim.SetTrigger("isRangedAttacking");
        }
        else if (distance > rangedAttackRange)
        {
            currentState = State.Unalerted;
            anim.SetBool("isWakingUp", false);
        }
    }

    protected override void HitBehaviour()
    {
        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            //shoot venom wave?

            float distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > rangedAttackRange)
            {
                currentState = State.Unalerted;
                anim.SetBool("isWakingUp", false);
            }
            else if (distance < rangedAttackRange)
                currentState = State.Attacking;
        }
    }

    public void ShootVenom()
    {
        GameObject tempVenom = ObjectPoolsController.instance.GetFromPool("snakeVenomPool");
        tempVenom.transform.position = projectilePos.transform.position;
        Vector2 direction = target.transform.position - transform.position;
        tempVenom.SetActive(true);
        tempVenom.GetComponent<Projectile>().ShootProjectile(direction.normalized);
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