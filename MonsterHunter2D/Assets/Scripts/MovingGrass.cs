using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingGrass : MonoBehaviour
{
    float enterOffset;
    bool isBending;
    bool isRebounding;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //enterOffset = collision.transform.position.x - transform.position.x;
            anim.SetTrigger("Move");
            Debug.Log("playermoved grass");

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            float offset = collision.transform.position.x - transform.position.x;

            if( isBending || Mathf.Sign(enterOffset) != Mathf.Sign(offset))
            {
                isRebounding = false;
                isBending = true;

                
            }
        }
    }
}
