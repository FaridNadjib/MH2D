using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockEventTrigger : MonoBehaviour
{
    private FallingRockEvent rockEvent;
    private bool triggered = false;

    private void Awake() 
    {
        rockEvent = GetComponentInParent<FallingRockEvent>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.gameObject.tag == "Player" && !triggered )
        {
            triggered = true;
            rockEvent.started = true;
        }    
    }
}
