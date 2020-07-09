using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class PressurePlatform : MovingPlatform
{
    [Header("Moving Object")]
    [SerializeField] protected GameObject objectToMove;
    [SerializeField] protected Transform objectEndPos;
    [SerializeField] private Transform objectStartPos;
    [SerializeField] protected float objectSpeed;
    [SerializeField] protected CinemachineVirtualCamera objectCam;

    private PlayerController player;

    protected bool pushed = false;
    protected bool pushedOnce = false;
    private bool cameraEventTriggered = false;
    protected bool cameraEventActive = false;
    [SerializeField] private float cameraEventTime = 4f;

    protected override void Update()
    {
        if (pushed && transform.position == endPos.position || !pushed && transform.position == startPos.position)
            return;

        if (pushed)
            Extend();
        else
            Reset();

        MoveObject();

        CameraEvent();
    }

    protected void CameraEvent()
    {
        if (!cameraEventActive)
            return;

        cameraEventTime -= Time.deltaTime;

        if (cameraEventTime > 0)
            return;

        objectCam.enabled = false;
        cameraEventActive = false;

        if (player != null)
        {
            player.blockInput = false;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    protected virtual void MoveObject()
    {
        objectToMove.transform.position = Vector2.Lerp(objectStartPos.position, objectEndPos.position, GetCurrentPushedValue());
    }

    private float GetCurrentPushedValue()
    {
        float pushedValue = this.transform.localPosition.x / endPos.localPosition.x;
        return pushedValue;
    }

    protected override void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player" && other.enabled)
        {                
            pushed = true;
            pushedOnce = true;
            other.gameObject.transform.SetParent(this.gameObject.transform);
            player = other.gameObject.GetComponent<PlayerController>();

            if (!cameraEventTriggered && objectCam != null)
            {
                player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                player.blockInput = true;
                cameraEventTriggered = true;
                cameraEventActive = true;
                objectCam.enabled = true;
            }
        }
    }

    protected override void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.tag == "Player" && other.enabled)
        {
            pushed = false;
            other.gameObject.transform.SetParent(null);
        }
    }
}
