using UnityEngine;

public class EnemyBat : Enemy
{
    protected override void Alerted(Collider2D other)
    {
        if (playerCol == null)
        {
            playerCol = target.GetComponent<Collider2D>();
            Physics2D.IgnoreCollision(this.GetComponent<CapsuleCollider2D>(), playerCol);
        }

        currentState = State.Attacking;
        anim.SetBool("isWakingUp", true);
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
        //bat is sleeping or patrolling
    }
}