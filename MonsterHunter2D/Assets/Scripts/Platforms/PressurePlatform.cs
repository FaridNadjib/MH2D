﻿using UnityEngine;
using Cinemachine;

public class PressurePlatform : MovingPlatform
{
    [Header("Moving Object")]
    [SerializeField] protected GameObject objectToMove;
    [SerializeField] protected Transform objectEndPos;
    [SerializeField] private Transform objectStartPos;
    [SerializeField] protected float objectSpeed;
    [SerializeField] protected CinemachineVirtualCamera objectCam;

    [SerializeField] private GameObject objectToHide;

    private PlayerController player;

    public bool pushed = false;
    protected bool pushedOnce = false;
    private bool cameraEventTriggered = false;
    protected bool cameraEventActive = false;
    [SerializeField] private float cameraEventTime = 4f;

    protected override void Update()
    {
        if (pushed && transform.position.y == endPos.position.y || !pushed && transform.position.y == startPos.position.y)
        {
            if (audioSource != null)
                audioSource.Stop();
            print("endPos reached");
            return;
        }

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

        print("cameraEventTime: " + cameraEventTime);

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
        float pushedValue = transform.localPosition.y / endPos.localPosition.y;
        return pushedValue;
    }

    protected override void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && other.enabled)
        {                
            pushed = true;
            pushedOnce = true;

            if (objectToHide != null)
                objectToHide.SetActive(false);

            if (setAsChild)
                other.gameObject.transform.SetParent(this.gameObject.transform);

            player = other.gameObject.GetComponent<PlayerController>();

            if (!cameraEventTriggered && objectCam != null)
            {
                if (hold || setAsChild)
                {
                    player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
                    player.blockInput = true;
                }

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

            if (setAsChild)
                other.gameObject.transform.SetParent(null);
        }
    }
}
