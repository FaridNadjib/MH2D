using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformsChildPlayer : MonoBehaviour
{
    ParticleSystem ps;

    // Start is called before the first frame update
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // On bottom of the player there is a circleCollider2D Component which is a trigger, it is looking for if the player steps on a moveable platform and makes it his parent.
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.tag == "Platform" || other.tag == "MovablePlatform")
    //    {
    //        //Instantiate(dustCloud, groundCheck.position, dustCloud.transform.rotation);
    //        //Instantiate(platformDrop, other.transform.position, platformDrop.transform.rotation);
    //    }

    //    if (other.tag == "MovablePlatform")
    //        transform.parent = other.transform;
    //}
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag == "MovablePlatform")
    //    {
    //        transform.parent = null;
    //        // Make sure it has no rotation, when leaving a moveable platform.
    //        transform.rotation = Quaternion.identity;
    //    }


    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //play particles
            if(ps != null && !ps.isEmitting)
                ps.Play();
            Debug.Log("PLayer entered");
            collision.transform.parent = transform.parent;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
            collision.transform.rotation = Quaternion.identity;
            Debug.Log("PLayer gone");
        }
    }
}
