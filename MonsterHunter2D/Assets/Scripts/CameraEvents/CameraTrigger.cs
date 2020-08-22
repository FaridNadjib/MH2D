using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTrigger : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private bool activatedOnce = false;
    public bool ActivatedOnce { get => activatedOnce; }

    private void Awake() 
    {
        virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
            virtualCamera.enabled = true;
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
