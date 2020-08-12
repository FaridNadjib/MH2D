using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [Header("Moveable Platform Variables")]
    public bool bShouldMove;
    public Transform[] positions;
    public float fSpeed;
    // LoopingPrder: true, loop the positions[]. False: go back and forward through positions[].
    public bool bLoopingOrder;

    private int posIndex;
    private Vector3 nextPos;
    // currentPos is just for gizmodraw.
    private Vector3 currentPos;

    [Header("Roatating Platform Variables. Only Circular Shaped Ones.")]
    public bool bShouldRotate;
    public float fRotationSpeed;

    [Header("Flipping Platform Variables.")]
    public bool bShouldFilp;
    public float fFlipOccurance;
    public float fFlipDuration;
    public float fPushback;
    private float fFlipTimer;
    private bool bFlipped;


    // Start is called before the first frame update
    void Start()
    {
        if (bShouldMove)
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

        fFlipTimer = fFlipOccurance;
    }

    // Update is called once per frame
    void Update()
    {
        if (bShouldMove)
        {
            // Check for the next platformposition and then move the platform.
            if (transform.position == nextPos)
            {
                // Loop positions[], at the end start from ppositions[0] again.
                if (bLoopingOrder)
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
                else if (!bLoopingOrder)
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
            transform.position = Vector3.MoveTowards(transform.position, nextPos, fSpeed * Time.deltaTime);
        }

        // Glitch: When standing on a rect shape rotating platform, the player will behave weired. Only circular shaped work fine.
        // Check if the platform should rotate and then rotate the platform.
        if (bShouldRotate)
        {
            transform.Rotate(Vector3.forward * fRotationSpeed * Time.deltaTime);

            if (transform.Find("Player"))
            {
                transform.Find("Player").rotation = Quaternion.Euler(transform.rotation.x * -1, transform.rotation.y * -1, transform.rotation.z * -1);
            }
        }

        // Filp the platform if enabled. It will flip after fFlipOccurance seconds for fFilpDuration seconds.
        if (bShouldFilp)
        {
            if (!bFlipped)
            {
                if (fFlipTimer > 0)
                {
                    fFlipTimer -= Time.deltaTime;
                }
                else
                {
                    // If the player is standing on the platform, push him back and break up parent, so he wont be affected by the scale operation.
                    if (transform.Find("Player"))
                    {
                        transform.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector2.up * fPushback, ForceMode2D.Impulse);
                        transform.Find("Player").transform.parent = null;
                    }
                    Vector3 scale = transform.localScale;
                    scale.y *= -1;
                    transform.localScale = scale;
                    bFlipped = true;
                    fFlipTimer = 0;
                }
            }
            else
            {
                if (fFlipTimer < fFlipDuration)
                    fFlipTimer += Time.deltaTime;
                else
                {
                    if (transform.Find("Player"))
                    {
                        transform.Find("Player").GetComponent<Rigidbody2D>().AddForce(Vector2.up * fPushback, ForceMode2D.Impulse);
                        transform.Find("Player").transform.parent = null;
                    }
                    Vector3 scale = transform.localScale;
                    scale.y *= -1;
                    transform.localScale = scale;
                    bFlipped = false;
                    fFlipTimer = fFlipOccurance;
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
