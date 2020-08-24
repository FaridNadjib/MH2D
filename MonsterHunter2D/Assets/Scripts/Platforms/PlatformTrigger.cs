using UnityEngine;

/// <summary>
/// A trigger to activate an object that will then move. Joachim
/// </summary>
public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private TriggeredPlatform objectToMove;
    [Tooltip("Should the moving object stay at the end position?")]
    [SerializeField] private bool stay = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            objectToMove.Trigger();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            if (!stay)
                objectToMove.UnTrigger();
        }
    }
}
