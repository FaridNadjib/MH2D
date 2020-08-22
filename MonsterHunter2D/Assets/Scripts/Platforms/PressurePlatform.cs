using UnityEngine;
using Cinemachine;

/// <summary>
/// A Pressure-Pad that reacts when the player has stepped on it. 
/// The referenced object will then move between its start- and end-Point based on the pushed-value of the Pressure-Pad. 
/// (Value between 0-1)
/// </summary>
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

            return;
        }

        if (pushed)
            Extend();
        else
            Reset();

        MoveObject();

        CameraEvent();
    }

    /// <summary>
    /// Enables the cinemachine override camera for the specified amount of time.
    /// </summary>
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

    /// <summary>
    /// Moves the referenced object between its start- and end-position based on the current pushedvalue of the Pressure-Pad.
    /// </summary>
    protected virtual void MoveObject()
    {
        objectToMove.transform.position = Vector2.Lerp(objectStartPos.position, objectEndPos.position, GetCurrentPushedValue());
    }

    /// <summary>
    /// Gets the current pushed-value by dividing the currentposition by the endposition. 
    /// </summary>
    /// <returns>A float Pushed-Value between 0 and 1</returns>
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
        if (other.gameObject.GetComponent<PlayerController>() != null && other.enabled)
        {
            pushed = false;

            if (setAsChild)
                other.gameObject.transform.SetParent(null);
        }
    }
}
