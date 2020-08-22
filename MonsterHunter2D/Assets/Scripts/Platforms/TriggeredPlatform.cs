
/// <summary>
/// A platform which position changes based on a trigger.
/// </summary>
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
