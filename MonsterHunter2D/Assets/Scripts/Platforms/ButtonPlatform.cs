using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPlatform : PressurePlatform
{
    protected override void Update()
    {
        if ((pushed || pushedOnce) && transform.position == endPos.position && objectToMove.transform.position != objectEndPos.position)
        {
            MoveObject();
            CameraEvent();
        }

        if (hold)
        {
            if (pushed)
                Extend();
            else
                return;    
        }
        else
        {
            if (pushedOnce && transform.position != endPos.position)
                Extend();
        }
    }

    protected override void MoveObject()
    {
        float step = objectSpeed * Time.deltaTime;
        objectToMove.transform.position = Vector2.MoveTowards(objectToMove.transform.position, objectEndPos.position, step);
    }
}
