using UnityEngine;
using Cinemachine;

public class PressureOneTapPlatform : MovingPlatform
{
    [Header("Moving Object")]
    [SerializeField] protected GameObject objectToMove;
    [SerializeField] protected Transform objectEndPos;
    [SerializeField] private Transform objectStartPos;
    [SerializeField] protected float objectSpeed;
    [SerializeField] protected CinemachineVirtualCamera objectCam;

    [SerializeField] private GameObject objectToHide;

    private PlayerController player;

    protected bool pushed = false;
    protected bool pushedOnce = false;
    private bool cameraEventTriggered = false;
    protected bool cameraEventActive = false;
    [SerializeField] private float cameraEventTime = 4f;
    [SerializeField] private bool blockPlayer = false;


    protected override void Update()
    {
        CameraEvent();

        if (!hold)
        {
            if (pushed && transform.position == endPos.position || !pushed && transform.position == startPos.position)
                return;
        }
        else
        {
            if (pushed && transform.position == endPos.position || !pushed && transform.position == startPos.position)
            {
                pushed = false;
            }
        }


        if (pushed)
            Extend();
        else
            Reset();

        MoveObject();

    }

    protected void CameraEvent()
    {
        if (!cameraEventActive)
            return;

        if (player == null && blockPlayer)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            FreezePlayer();
        }

        cameraEventTime -= Time.deltaTime;

        if (cameraEventTime > 0)
            return;

        objectCam.enabled = false;
        cameraEventActive = false;

        if (blockPlayer)
            UnfreezePlayer();
    }

    protected virtual void MoveObject()
    {
        objectToMove.transform.position = Vector2.Lerp(objectStartPos.position, objectEndPos.position, GetCurrentPushedValue());
    }

    private float GetCurrentPushedValue()
    {
        float pushedValue = this.transform.localPosition.y / endPos.localPosition.y;
        return pushedValue;
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null && other.enabled || other.gameObject.GetComponent<Projectile>().Type == ActiveWeaponType.BombNormal)
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
                if ((hold || setAsChild) && player != null && blockPlayer)
                    FreezePlayer();

                cameraEventTriggered = true;
                cameraEventActive = true;
                objectCam.enabled = true;
            }
        }
    }

    private void FreezePlayer()
    {
        print("player freezed");
        player.gameObject.layer = 14;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        player.BlockInput(true);
    }

    private void UnfreezePlayer()
    {
        player.gameObject.layer = 8;
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        player.BlockInput(false);
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