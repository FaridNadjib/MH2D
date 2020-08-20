using UnityEngine;

public class BecomeChildOnCollision : MonoBehaviour 
{
    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
            other.gameObject.transform.SetParent(this.gameObject.transform);
    }

    private void OnCollisionExit2D(Collision2D other) 
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
            other.gameObject.transform.SetParent(null);
    }
}