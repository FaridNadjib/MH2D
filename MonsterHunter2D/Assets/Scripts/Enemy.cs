using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private Vector2 minMaxPlayerOffsetX;
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
    [SerializeField] private float currentTime;
    [SerializeField] private Vector3 knockBack;

    private Vector2 direction;

    private float currentSpeed;

    private CharacterResources resources;
    private Rigidbody2D rb;

    private void Awake() 
    {
        activeState = State.Unalerted;
        anim = GetComponent<Animator>();
        resources = GetComponent<CharacterResources>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;      
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
            currentSpeed = movementSpeed;

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
                currentSpeed = attackSpeed;
                startPos = transform.position;

                SetAttackPosition();
                CalculateThirdCurvePoint();
            }
            else if (activeState != State.Hit)
                lastState = activeState;

            //int damage = other.gameObject.GetComponent<Projectile>().damageValue;
            float damage = 5f;
            resources.ReduceHealth(damage);

            if (anim.GetBool("isWakingUp") == false)
            {
                anim.SetBool("isWakingUp", true);
            }

            direction = other.GetContact(0).point - (Vector2)transform.position; 

            anim.SetTrigger("gotDamaged");

            activeState = State.Hit;
        }
    }

    /// <summary>
    /// Sets a next random waypoint from the waypoints-array.
    /// </summary>
    private void SetNextWaypoint()
    {
        targetWaypointIndex = UnityEngine.Random.Range(0, waypoints.Length);
    }

    /// <summary>
    /// Sets a position around the player based on the minimum and maximum offset values for the X- and Y-axis.
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
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        attackPos = new Vector3(target.transform.position.x + UnityEngine.Random.Range(-attackOffsetX, attackOffsetX), target.transform.position.y, 0);
    }

    /// <summary>
    /// Moves the enemy to a position based on his current state. If the enemy reaches the position, it waits for a specified duration and starts to attack the player from there.
    /// </summary>
    protected virtual void MoveToPosition()
    {
        if (transform.position == nextPosAroundPlayer || transform.position == waypoints[targetWaypointIndex].position)
        {

            print("reached posAroundPlayer");

            startPos = transform.position;
            currentWaitTime += Time.deltaTime;

            if (currentWaitTime < waitAtWaypointTime)
                return;

            currentWaitTime = 0f;

            activeState = State.Attacking;
            currentSpeed = attackSpeed;
            SetAttackPosition();
            CalculateThirdCurvePoint();

            Flip();

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
                MoveInCurve();
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

            print("reached attackPos");
            startPos = transform.position;
            anim.SetBool("isAttacking", false);
            activeState = State.Alerted;

            currentSpeed = movementSpeed;


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
                MoveInCurve();
            }
        }
    }

    private void CalculateThirdCurvePoint()
    {
        float xOffset = UnityEngine.Random.Range(minMaxCurvePointOffsetX.x, minMaxCurvePointOffsetX.y);

        if (CalculateDirectionToPlayerPos() >= 0)
        {
            curvePoint = new Vector3(target.transform.position.x + xOffset, nextPosAroundPlayer.y, 0);
        }
        else
        {
            curvePoint = new Vector3(target.transform.position.x - xOffset, nextPosAroundPlayer.y, 0);
        }
    }

    private void MoveInCurve()
    {
        if (currentTime < 1.0f)
        {
            currentTime += Time.deltaTime * currentSpeed;

            Vector3 lerpToCurvePoint = new Vector3();
            Vector3 lerptoEndPoint = new Vector3();

            if (activeState == State.Attacking)
            {
                lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentTime);
                lerptoEndPoint = Vector3.Lerp(curvePoint, attackPos, currentTime);

                transform.position = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentTime);
            }
            else if (activeState == State.Alerted)
            {
                if (!moveBetweenCheckpoints)
                {
                    lerpToCurvePoint = Vector3.Lerp(startPos, curvePoint, currentTime);
                    lerptoEndPoint = Vector3.Lerp(curvePoint, nextPosAroundPlayer, currentTime);

                    transform.position = Vector3.Lerp(lerpToCurvePoint, lerptoEndPoint, currentTime);
                }
            }
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

        rb.AddForce(direction.normalized * Time.deltaTime);

        if (currentWaitTime >= waitAfterHitTime)
        {

            rb.velocity = Vector2.zero; 
            activeState = lastState;
            currentWaitTime = 0f;
            startPos = transform.position;

            if (activeState == State.Attacking)
            {
                anim.SetBool("isAttacking", true);
            }
        }
    }

    private float CalculateDirectionToPlayerPos()
    {
        float direction;

        if (transform.position.x > target.transform.position.x)
            direction = 1;
        else
            direction = -1;

        return direction;
    }

    private void Flip()
    {           
        transform.localScale = new Vector3(CalculateDirectionToPlayerPos(), transform.localScale.y, transform.localScale.z);
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
