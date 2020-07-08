using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeredPlatform : MovingPlatform
{
    private bool triggered = false;

    protected override void Update()
    {
        if (triggered)
            Extend(); 
        else
            Reset();
        
    }

    public void Trigger()
    {
        triggered = true;
    }

    public void UnTrigger()
    {
        triggered = false;
    }
}
