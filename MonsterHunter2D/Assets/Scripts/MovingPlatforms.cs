using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [Header("Moveable Platform Variables:")]
    [Tooltip("Should the platform move?")]
    [SerializeField] bool shouldMove;
    [Tooltip("Put all the waypoints from waypoint object of this platform in here.")]
    [SerializeField] Transform[] positions;
    [Tooltip("How many units per second?")]
    [SerializeField] float speed;
    [Tooltip("LoopingOrder: true, loop the positions[]. False: go back and forward through positions[].")]
    [SerializeField] bool loopingOrder;

    private int posIndex;
    private Vector3 nextPos;
    // CurrentPos is just for gizmodraw.
    private Vector3 currentPos;

    [Header("Rotating Platform Variables:")]
    [Tooltip("Should the platform rotate?")]
    [SerializeField] bool shouldRotate;
    [Tooltip("How many degrees per second? pos and negative possible.")]
    [SerializeField] float rotationSpeed;

    [Header("Flipping Platform Variables:")]
    [Tooltip("Should the platform flip?")]
    [SerializeField] bool shouldFilp;
    [Tooltip("After how many seconds should it filp first?")]
    [SerializeField] float flipOccurance;
    [Tooltip("How long should that filp last?")]
    [SerializeField] float flipDuration;
    [Tooltip("Add pushback to the player, outdated though leave it at 0.")]
    [SerializeField] float pushback;
    private float flipTimer;
    private bool flipped;
    [Tooltip("The PS to be emmitted as warning for that palyer that the platform is about to flip.")]
    [SerializeField] ParticleSystem flipWarning;
    [Tooltip("Duration in seconds before the platform flips and the ps should be emmiting.")]
    [SerializeField] float warningTime;


    // Start is called before the first frame update
    void Start()
    {
        if (shouldMove)
        {
            // First position to travel to will be the nearest poistion next to the platform.
            nextPos = positions[0].position;
            for (var i = 0; i < positions.Length; i++)
            {
                if (Vector3.Distance(transform.position, positions[i].position) < Vector3.Distance(transform.position, nextPos))
                {
                    nextPos = positions[i].position;
                    posIndex = i;
                }
            }
        }

        flipTimer = flipOccurance;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            // Check for the next platformposition and then move the platform.
            if (transform.position == nextPos)
            {
                // Loop positions[], at the end start from ppositions[0] again.
                if (loopingOrder)
                {
                    if (posIndex < positions.Length - 1)
                    {
                        currentPos = nextPos;
                        nextPos = positions[posIndex + 1].position;
                        posIndex++;
                    }
                    else
                    {
                        currentPos = nextPos;
                        posIndex = 0;
                        nextPos = positions[posIndex].position;
                    }
                }
                // Loop back the array, dont start from position 0.
                else if (!loopingOrder)
                {
                    if (posIndex < positions.Length - 1)
                    {
                        currentPos = nextPos;
                        nextPos = positions[posIndex + 1].position;
                        posIndex++;
                    }
                    else
                    {
                        System.Array.Reverse(positions);
                        currentPos = nextPos;
                        posIndex = 1;
                        nextPos = positions[posIndex].position;
                    }
                }
            }
            transform.position = Vector3.MoveTowards(transform.position, nextPos, speed * Time.deltaTime);
        }

        // Glitch: When standing on a rect shape rotating platform, the player will behave weired. Only circular shaped work fine.
        // Check if the platform should rotate and then rotate the platform.
        if (shouldRotate)
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

            if (transform.Find("Player"))
            {
                transform.Find("Player").rotation = Quaternion.Euler(transform.rotation.x * -1, transform.rotation.y * -1, transform.rotation.z * -1);
            }
        }

        // Filp the platform if enabled. It will flip after fFlipOccurance seconds for fFilpDuration seconds.
        if (shouldFilp)
        {
            if (!flipped)
            {
                if (flipTimer > warningTime)
                {
                    flipTimer -= Time.deltaTime;
                }
                else if(flipTimer > 0)
                {
                    flipTimer -= Time.deltaTime;
                    if (flipWarning != null && !flipWarning.isEmitting)
                        flipWarning.Play();
                }
                else
                {
                    // If the player is standing on the platform, push him back and break up parent, so he wont be affected by the scale operation.
                    if (transform.Find("Player"))
                    {
                        transform.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector2.up * pushback, ForceMode2D.Impulse);
                        transform.Find("Player").transform.parent = null;
                    }
                    Vector3 scale = transform.localScale;
                    scale.y *= -1;
                    transform.localScale = scale;
                    flipped = true;
                    flipTimer = 0;
                }
            }
            else
            {
                if (flipTimer < flipDuration)
                    flipTimer += Time.deltaTime;
                else
                {
                    if (transform.Find("Player"))
                    {
                        transform.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector2.up * pushback, ForceMode2D.Impulse);
                        transform.Find("Player").transform.parent = null;
                    }
                    Vector3 scale = transform.localScale;
                    scale.y *= -1;
                    transform.localScale = scale;
                    flipped = false;
                    flipTimer = flipOccurance;
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If a flippable platform has a dangerzone on it, set that specific collider to isTrigger and code what should happen here.
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(currentPos, nextPos);
    }
}
