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
