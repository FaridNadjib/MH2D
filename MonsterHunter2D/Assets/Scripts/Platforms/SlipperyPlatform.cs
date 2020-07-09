using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlipperyPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            //other.gameObject.GetComponent<PlayerController>().blockInput = true;
        }
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            //other.gameObject.GetComponent<PlayerController>().blockInput = false;
        }
    }
}
