using UnityEngine;

public class MovingPlant : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Transform forcePoint;
    [SerializeField] private Vector2 force;
    private Vector2 forcePointPos;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();
        forcePointPos = new Vector2(forcePointPos.x, forcePointPos.y);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            Vector2 newForce = new Vector2();

            if (other.gameObject.transform.position.x > transform.position.x)
            {
                newForce = new Vector2(-force.x, force.y);
            }
            else
            {
                newForce = new Vector2(force.x, force.y);
            }

            rb.AddForceAtPosition(newForce, forcePointPos);
        }
    }
}
