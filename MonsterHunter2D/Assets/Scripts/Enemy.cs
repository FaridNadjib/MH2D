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
    private Vector3 startPos;
    [MinMaxSlider(-10, 10)]
    [SerializeField] private Vector2 minMaxPlayerOffsetX;
    [MinMaxSlider(4, 12)]
    [SerializeField] private Vector2 minMaxPlayerOffsetY;
    [SerializeField] private float attackOffsetX;
    private bool alertedOnce = false;
    [Tooltip("Should this enemy move between fixed checkpoints or should it move based on the player position?")]
    [SerializeField] private bool moveBetweenCheckpoints = false;
    [SerializeField] private bool moveInRandomCurve = false;
    [SerializeField] private float arcHeight;
    [SerializeField] private Vector3 curvePoint;
    [SerializeField] private Vector2 minMaxCurvePointOffsetX;
    [SerializeField] private Vector2 minMaxCurvePointOffsetY;
    [SerializeField] private float timeToMoveToAttackPos;
    [SerializeField] private float timeToMoveToPosAroundPlayer;
    [SerializeField] private float timeToMoveToPos;
    [SerializeField] private float currentTime;




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
                timeToMoveToPos = timeToMoveToAttackPos;
                lastState = State.Attacking;
                SetAttackPosition();
                CalculateThirdCurvePoint();
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

            //rb.AddForceAtPosition(other.GetContact(0).normal, other.GetContact(0).point, ForceMode2D.Impulse);

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
            startPos = transform.position;
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime < waitAtWaypointTime)
                return;

            timeToMoveToPos = timeToMoveToAttackPos;

            activeState = State.Attacking;
            SetAttackPosition();
            currentWaitTime = 0f;
            CalculateThirdCurvePoint();

            anim.SetBool("isAttacking", true);


        }
        else
        {
            if (!moveInRandomCurve)
            {
                if (moveBetweenCheckpoints)
                    transform.position = Vector2.MoveTowards(transform.position, waypoints[targetWaypointIndex].position, movementSpeed * Time.deltaTime);
                else
                    transform.position = Vector2.MoveTowards(transform.position, nextPosAroundPlayer, movementSpeed * Time.deltaTime);
            }
            else
            {
                if (moveBetweenCheckpoints)
                    MoveInArc(waypoints[targetWaypointIndex].position, movementSpeed);
                else
                    MoveInArc(nextPosAroundPlayer, movementSpeed);
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
            startPos = transform.position;
            anim.SetBool("isAttacking", false);
            activeState = State.Alerted;

            timeToMoveToPos = timeToMoveToPosAroundPlayer;

            if (moveBetweenCheckpoints)
                SetNextWaypoint();
            else
                SetNextPositionAroundPlayer();

            CalculateThirdCurvePoint();
        }
        else
        {
            if (!moveInRandomCurve)
            {
                transform.position = Vector2.MoveTowards(transform.position, attackPos, attackSpeed * Time.deltaTime);
            }
            else
            {
                MoveInArc(attackPos, attackSpeed / 2);
            }

        }
    }

    private void CalculateThirdCurvePoint()
    {
        float direction = attackPos.x - transform.position.x;
        float xOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetX.x, minMaxCurvePointOffsetX.y);

        if (direction > 0)
        {
            curvePoint = new Vector3(target.transform.position.x + xOffset, nextPosAroundPlayer.y, 0);
        }
        else
        {
            curvePoint = new Vector3(target.transform.position.x - xOffset, nextPosAroundPlayer.y, 0);
        }



    }

    private void MoveStraight()
    {
        
    }


    private void MoveInArc(Vector3 targetPos, float speed)
    {
        // float x0 = startPos.x;
        // float x1 = targetPos.x;
        // float dist = x1 - x0;
        // float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
        // float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
        // float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
        // if (dist != 0)
        //     transform.position = new Vector3(nextX, baseY + arc, transform.position.z);


        if (currentTime < timeToMoveToPos)
        {
            currentTime += Time.deltaTime * timeToMoveToPos;


            Vector3 m1 = new Vector3();
            Vector3 m2 = new Vector3();


            if (activeState == State.Attacking)
            {
                m1 = Vector3.Lerp(startPos, curvePoint, timeToMoveToPos);
                m2 = Vector3.Lerp(curvePoint, attackPos, timeToMoveToPos);
            }
            else if (activeState == State.Alerted)
            {
                if (!moveBetweenCheckpoints)
                {
                    m1 = Vector3.Lerp(startPos, curvePoint, timeToMoveToPos);
                    m2 = Vector3.Lerp(curvePoint, nextPosAroundPlayer, timeToMoveToPos);
                }
            }

            transform.position = Vector3.Lerp(m1, m2, timeToMoveToPos);
        }
        else
            currentTime = 0f;

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

        Gizmos.color = Color.green;

        Gizmos.DrawSphere(curvePoint, 2f);
    }
}
