using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private TriggeredPlatform platform;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            platform.Trigger();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            platform.UnTrigger();
        }
    }
}
