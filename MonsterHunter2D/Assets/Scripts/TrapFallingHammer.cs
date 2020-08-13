using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapFallingHammer : MonoBehaviour
{

    [SerializeField] Transform anchorPos;
    [SerializeField] float allowedFallingTime;
    [SerializeField] float rechargeSpeed;
    [SerializeField] ParticleSystem rechargePs1;
    [SerializeField] ParticleSystem rechargePs2;

    float timer;
    bool backToAnchor = true;

    Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (backToAnchor)
        {
            transform.position = Vector3.MoveTowards(transform.position, anchorPos.position, rechargeSpeed * Time.deltaTime);
            if (rechargePs1 != null && !rechargePs1.isEmitting)
                rechargePs1.Play();
            if (rechargePs2 != null && !rechargePs2.isEmitting)
                rechargePs2.Play();
        }
        if(transform.position == anchorPos.position)
        {
            backToAnchor = false;
            rb.isKinematic = false;
            timer = 0f;
        }
        if (!backToAnchor)
        {
            if (timer < allowedFallingTime)
                timer += Time.deltaTime;
            else
            {
                backToAnchor = true;
                rb.isKinematic = true;
                rb.velocity = Vector2.zero;
                timer = 0f;
            }
        }
    }
}
