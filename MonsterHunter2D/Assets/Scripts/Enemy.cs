using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD.MinMaxSlider;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected Transform[] waypoints;
    [SerializeField] private int targetWaypointIndex = 0;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float waitAtWaypointTime;
    [SerializeField] private float waitAfterHitTime;
    private float currentWaitTime = 0f;
    private enum State { Unalerted, Alerted, Attacking, Hit, Dead }
    [SerializeField] private State activeState;
    private State lastState;
    private Animator anim;
    private PlayerController target;
    [SerializeField] private Vector3 nextPosAroundPlayer;
    [SerializeField] private Vector3 attackPos;
    [MinMaxSlider(-10, 10)]
    [SerializeField] private Vector2 minMaxPlayerOffsetX;
    [MinMaxSlider(4, 12)]
    [SerializeField] private Vector2 minMaxPlayerOffsetY;
    [SerializeField] private float attackOffsetX;
    private bool alertedOnce = false;
    [Tooltip("Should this enemy move between fixed checkpoints or should it move based on the player position?")]
    [SerializeField] private bool moveBetweenCheckpoints = false;
    [SerializeField] private bool moveInRandomCurve = false;


    private CharacterResources resources;
    private Rigidbody2D rb;

    private void Awake() 
    {
        activeState = State.Unalerted;
        anim = GetComponent<Animator>();
        resources = GetComponent<CharacterResources>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (activeState == State.Alerted)
        {
            MoveToPosition();
        }
        else if (activeState == State.Attacking)
        {
            Attack();
        }
        else if (activeState == State.Hit)
        {
            WaitAfterHit();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !alertedOnce)
        {
            alertedOnce = true;
            target = other.GetComponent<PlayerController>();

            if (!moveBetweenCheckpoints)
                SetNextPositionAroundPlayer();

            anim.SetBool("isWakingUp", true);
            activeState = State.Alerted;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
       if (other.gameObject.GetComponent<Projectile>() != null)
       {
            if (!alertedOnce)
                alertedOnce = true;

            if (activeState == State.Unalerted)
            {
                lastState = State.Attacking;
                SetAttackPosition();
            }
            else if (activeState != State.Hit)
                lastState = activeState;

            activeState = State.Hit;

            //int damage = other.gameObject.GetComponent<Projectile>().damageValue;
            int damage = 5;
            resources.ReduceHealth(damage);

            if (anim.GetBool("isWakingUp") == false)
            {
                anim.SetBool("isWakingUp", true);
            }

            rb.AddForceAtPosition(other.GetContact(0).normal, other.GetContact(0).point, ForceMode2D.Impulse);

            anim.SetTrigger("gotDamaged");
        }
    }

    /// <summary>
    /// Sets a next waypoint from the waypoints-array.
    /// </summary>
    private void SetNextWaypoint()
    {
        targetWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
    }

    /// <summary>
    /// Sets a random position around the player.
    /// </summary>
    private void SetNextPositionAroundPlayer()
    {
        float offsetX = UnityEngine.Random.Range(minMaxPlayerOffsetX.x, minMaxPlayerOffsetX.y);
        float offsetY = UnityEngine.Random.Range(minMaxPlayerOffsetY.x, minMaxPlayerOffsetY.y);
        nextPosAroundPlayer = new Vector3(target.transform.position.x + offsetX, target.transform.position.y + offsetY, 0);
    }

    /// <summary>
    /// Sets a new attack-position with an offset around the player.
    /// </summary>
    private void SetAttackPosition()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        attackPos = new Vector3(target.transform.position.x + UnityEngine.Random.Range(-attackOffsetX, attackOffsetX), target.transform.position.y, 0);
    }


    /// <summary>
    /// Moves the enemy to a position. If the enemy reaches the position, it waits for a specified duration and starts to attack the player from there.
    /// </summary>
    protected virtual void MoveToPosition()
    {
        if (transform.position == nextPosAroundPlayer || transform.position == waypoints[targetWaypointIndex].position)
        {
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime < waitAtWaypointTime)
                return;

            activeState = State.Attacking;
            SetAttackPosition();
            currentWaitTime = 0f;
            anim.SetBool("isAttacking", true);
        }
        else
        {
            if (!moveInRandomCurve)
            {
                float step = movementSpeed * Time.deltaTime;
                if (moveBetweenCheckpoints)
                    transform.position = Vector2.MoveTowards(transform.position, waypoints[targetWaypointIndex].position, step);
                else
                    transform.position = Vector2.MoveTowards(transform.position, nextPosAroundPlayer, step);
            }
            else
            {

            }

        }
    }

    /// <summary>
    /// Moves the enemy towards the attack-position. If the enemy reaches the attack-position, a new position is calculated and the state is changed.
    /// </summary>
    protected virtual void Attack()
    {
        if (transform.position == attackPos)
        {
            anim.SetBool("isAttacking", false);
            activeState = State.Alerted;

            if (moveBetweenCheckpoints)
                SetNextWaypoint();
            else
                SetNextPositionAroundPlayer();
        }
        else
        {
            if (!moveInRandomCurve)
            {
                float step = attackSpeed * Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position, attackPos, step);
            }
            else
            {

            }

        }
    }

    /// <summary>
    /// Waits a specified duration after being hit and sets the last saved state before being hit as the active state.
    /// </summary>
    private void WaitAfterHit()
    {
        currentWaitTime += Time.deltaTime;

        if (currentWaitTime >= waitAfterHitTime)
        {
            activeState = lastState;
            currentWaitTime = 0f;

            if (activeState == State.Attacking)
            {
                anim.SetBool("isAttacking", true);
            }
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;

        if (moveBetweenCheckpoints)
        {
            for (int i = 0; i < waypoints.Length; i++)
            {
                Gizmos.DrawSphere(waypoints[i].position, 1f);
            }

            Gizmos.DrawLine(transform.position, waypoints[targetWaypointIndex].position);
        }
        else
        {
            Gizmos.DrawSphere(nextPosAroundPlayer, 1f);
            Gizmos.DrawLine(transform.position, nextPosAroundPlayer);
        }
    }
}
