using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private bool activatedOnce = false;
    public bool ActivatedOnce { get => activatedOnce; }
    [SerializeField] private bool triggerOnce = false;
    [SerializeField] private bool zoom = false;
    private bool triggered;
    private bool zooming = false;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float startSize;
    [SerializeField] private float targetSize;
    private float waitAfterReach = 3f;


    private void Awake() 
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void Update() 
    {
        if (!zooming)
            return;

        if (virtualCamera.m_Lens.OrthographicSize > targetSize)
        {
            virtualCamera.m_Lens.OrthographicSize -= zoomSpeed * Time.deltaTime;
            return;
        }

        waitAfterReach -= Time.deltaTime;

        if (waitAfterReach <= 0)
        {
            virtualCamera.enabled = false;
            zooming = false;
        }
 


    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (triggerOnce && triggered)
                return;

            virtualCamera.enabled = true;

            if (zoom == true)
            {
                virtualCamera.m_Lens.OrthographicSize = startSize;
                zooming = true;
            }

            triggered = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
            virtualCamera.enabled = false;
    }

    public void On()
    {
        activatedOnce = true;
        virtualCamera.enabled = true;
    }

    public void Off()
    {
        virtualCamera.enabled = false;
    }
}
