using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MovingPlatform : MonoBehaviour
{
    [Header("Button/Platform")]
    [SerializeField] protected Transform endPos;
    [SerializeField] protected Transform startPos;
    [SerializeField] private float stayAtEndTime;
    [SerializeField] private float stayAtStartTime;
    [SerializeField] private float speedToEnd;
    [SerializeField] private float speedToStart;
    [Tooltip("Should the button be pushed once or held down to activate?")]
    [SerializeField] protected bool hold = false;
    [Tooltip("Should the player become a child on collision?")]
    [SerializeField] protected bool setAsChild = false;

    private float currentTime = 0;
    private bool extending = true;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (extending)
            Extend();
        else
            Reset();
    }

    protected virtual void Reset()
    {
        currentTime += Time.deltaTime;

        if (currentTime < stayAtEndTime)
            return;

        float step = speedToStart * Time.deltaTime;
        
        transform.position = Vector2.MoveTowards(transform.position, startPos.position, step);

        if (transform.position == startPos.position)
        {
            currentTime = 0;
            extending = true;
        }
    }

    protected virtual void Extend()
    {
        currentTime += Time.deltaTime;

        if (currentTime < stayAtStartTime)
            return;

        float step = speedToEnd * Time.deltaTime;

        transform.position = Vector2.MoveTowards(transform.position, endPos.position, step);

        if (transform.position == endPos.position)
        {
            currentTime = 0;
            extending = false;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && other.enabled)
        {
            if (setAsChild)
                other.gameObject.transform.SetParent(this.gameObject.transform);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player" && other.enabled)
        {
            if (setAsChild)
                other.gameObject.transform.SetParent(null);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(endPos.position, startPos.position);
        Gizmos.DrawSphere(endPos.position, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos.position, 0.5f);
    }
}
