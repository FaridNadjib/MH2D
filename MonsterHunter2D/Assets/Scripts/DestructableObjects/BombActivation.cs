using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombActivation : MonoBehaviour
{
    bool exploded = false;
    float timer = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (exploded)
        {
            if (timer > 0f)
                timer -= Time.deltaTime;
            else
            {
            Collider2D col = GetComponent<Collider2D>();
            col.usedByEffector = false;

            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (!exploded)
        {
            Debug.Log("hitted");
            if (collision.GetComponent<DestructableObject>() != null)
            {
                collision.GetComponent<DestructableObject>().ActivateDestruction();
            //exploded = true;

            }
            Collider2D col = GetComponent<Collider2D>();
            col.usedByEffector = true;
            //col.usedByEffector = false;
            //gameObject.SetActive(false);
        }




    }
}
