using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombActivation : MonoBehaviour
{
    float explosionTime = 0.2f;
    float timer = 0f;
    bool exploded = false;
    Collider2D col;

    PointEffector2D point;

    // Start is called before the first frame update
    void Awake()
    {
        col = GetComponent<Collider2D>();
        point = GetComponent<PointEffector2D>();
    }

    private void OnEnable()
    {
        //col.usedByEffector = true;
        point.enabled = true;
        timer = 0;
        exploded = false;
    }
    private void Update()
    {
        if (timer <= explosionTime)
            timer += Time.deltaTime;
        else
        {
            exploded = true;
        }
        if (exploded)
        {
                //col.usedByEffector = false;
            point.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exploded)
        {
            if (collision.GetComponent<DestructableObject>() != null)
            {
                Debug.Log("found destruct");
                collision.GetComponent<DestructableObject>().ActivateDestruction();
                //exploded = true;

            }
            if (collision.GetComponent<PlayerController>())
                collision.GetComponent<CharacterResources>().ReduceHealth(500f);
            else
                Debug.Log("no player?");
        }
        






    }
}
