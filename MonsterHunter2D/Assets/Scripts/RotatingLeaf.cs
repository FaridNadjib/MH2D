using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLeaf : MonoBehaviour
{
    private Rigidbody2D rb;
    private HingeJoint2D joint;
    private EdgeCollider2D col;
    private bool collided = false;
    private bool broken = false;
    [SerializeField] private float waitAfterCollision = 1f;
    [SerializeField] private float resetTime = 3f;
    [SerializeField] private float resetRotationSpeed = 10f;
    private float currentWaitTime = 0f;
    private Transform startPos;

    private void Awake() 
    {
        startPos = transform;
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        joint = GetComponent<HingeJoint2D>();
        col = GetComponent<EdgeCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            collided = true;
        }
    }

    private void Update() 
    {
        BreakLeaf();
        ResetLeaf();
    }

    private void BreakLeaf()
    {
        if (!collided)
            return;

        currentWaitTime += Time.deltaTime;

        if (!(currentWaitTime >= waitAfterCollision))
            return;

        rb.constraints = RigidbodyConstraints2D.None;

        if (!(joint.limitState >= JointLimitState2D.UpperLimit))
            return;

        col.enabled = false;
        broken = true;
        collided = false;
        currentWaitTime = 0f;
    }

    private void ResetLeaf()
    {
        if (!broken)
            return;

        currentWaitTime += Time.deltaTime;

        if (!(currentWaitTime >= resetTime))
            return;

        var step = resetRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, startPos.rotation, step);

        if (!(transform.rotation == startPos.rotation))
            return;

        col.enabled = true;
        broken = false;
        currentWaitTime = 0f;
    }
}
