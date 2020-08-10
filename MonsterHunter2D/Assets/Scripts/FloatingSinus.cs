using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSinus : MonoBehaviour
{
    [Header("How it floats:")]
    [SerializeField] float speed;
    [SerializeField] float height;
    [SerializeField] float width;
    float fluctuation;
    Vector3 newPos = new Vector3();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        newPos = transform.position;
        fluctuation = Mathf.Sin(Time.time * speed);
        newPos.x += fluctuation * width * Time.deltaTime;
        newPos.y += fluctuation * height * Time.deltaTime;
        transform.position = newPos;
    }
}
