using UnityEngine;

/// <summary>
/// lets a boulder roll towards the player after he activated the trigger.!-- Joachim
/// </summary>
public class CaveEndTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boulder;
    [SerializeField] private GameObject door;

    private bool triggeredOnce = false;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null && !triggeredOnce)
        {
            door.SetActive(false);
            boulder.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        }   
    }
}
