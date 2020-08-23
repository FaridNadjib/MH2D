using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class activated a point effector to simulate a bomb explosion. Farid
/// </summary>
public class BombActivation : MonoBehaviour
{
    #region Fields
    float explosionTime = 0.2f;
    float timer = 0f;
    bool exploded = false;
    Collider2D col;
    PointEffector2D point;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Get the components.
        col = GetComponent<Collider2D>();
        point = GetComponent<PointEffector2D>();
    }

    /// <summary>
    /// This is part of a projectile used by the objectpool, therefore onEnable resets some stats.
    /// </summary>
    private void OnEnable()
    {
        // Restore the defaults, since its used by the object pool.
        point.enabled = true;
        timer = 0;
        exploded = false;
    }

    private void Update()
    {
        // Count down the time how long the point effector will be active.
        if (timer <= explosionTime)
            timer += Time.deltaTime;
        else
            exploded = true;
        if (exploded)
            point.enabled = false;
    }

    /// <summary>
    /// Depending on what enters the trigger collider, the bomb will apply damage or calls another action.
    /// </summary>
    /// <param name="collision">Destructable object? Player? Enemy?</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exploded)
        {
            if (collision.GetComponent<DestructableObject>() != null)
                collision.GetComponent<DestructableObject>().ActivateDestruction();

            if (collision.GetComponent<PlayerController>())
                collision.GetComponent<CharacterResources>().ReduceHealth(500f);

            if (collision.GetComponent<Enemy>())
            {
                if (collision.GetComponent<CharacterResources>() != null)
                    collision.GetComponent<CharacterResources>().ReduceHealth(15);
            }
        }
    }
}
