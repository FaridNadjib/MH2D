using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFish : Enemy
{
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;
    [SerializeField] private float waitAfterAttackTime;

    private void Start() 
    {
        nextPos = waypoints[targetWaypointIndex].position;
        currentSpeed = standardSpeed;
    }

    protected override void Update()
    {
        if (activeState == State.Dead)
            return;
        else if (activeState == State.Hit)
            WaitAfterHit();
        else if (activeState == State.Unalerted)
            Move();
        else if (activeState == State.Attacking)
        {
            WaitAfterAttack();
        }

    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !alertedOnce)
        {
            alertedOnce = true;
            target = other.GetComponent<PlayerController>();

            anim.SetBool("attacking", true);
            //rb.AddForce((target.transform.position + new Vector3(0, 4, 0) - transform.position), ForceMode2D.Impulse);

            if (CalculateDirectionToPos(target.transform.position) >= 0)
                curvePoint = new Vector3(target.transform.position.x + 3, target.transform.position.y + 4 );
            else
                curvePoint = new Vector3(target.transform.position.x - 3, target.transform.position.y + 4);

            activeState = State.Attacking;

        }
    }

    protected override void Move()
    {
        if (Vector2.Distance(transform.position, nextPos) < 0.1f)
        {
            print(this + "reached checkpoint");
            if (activeState == State.Unalerted)
            {
                SetNextWaypoint();
                Flip();
            }
        }
        else
        {
            Vector3 pos = Vector2.MoveTowards(transform.position, nextPos, standardSpeed * Time.deltaTime * 20);
            transform.position = pos + transform.up * Mathf.Sin(Time.time * frequency) * magnitude; 
        }
    }

    private void WaitAfterAttack()
    {
        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterAttackTime)
        {
            rb.gravityScale = 1f;
            rb.velocity = Vector2.zero;
            activeState = State.Alerted;
            currentWaitTime = 0f;
        }
        Rotate();
    }

    private void Rotate()
    {
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    protected override void Flip()
    {
        transform.localScale = new Vector3(CalculateDirectionToPos(nextPos) * transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
