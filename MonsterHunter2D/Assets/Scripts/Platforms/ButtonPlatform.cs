using UnityEngine;

/// <summary>
/// A button that will move another object to its End-Position and back.
/// </summary>
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
