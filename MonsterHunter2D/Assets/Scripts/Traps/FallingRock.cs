using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : MonoBehaviour
{
    [SerializeField] public Sprite[] sprites;
    [SerializeField] public float minScale;
    [SerializeField] public float maxScale;
    private Rigidbody2D rb;

    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>();   
    }

    private void Update() 
    {
        if (rb.velocity == Vector2.zero)
        {
            Disable();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.layer == 8)
        {
            // Play collision sound
            // Play particle effect
        }
    }

    public void Setup(Vector2 pos)
    {
        gameObject.transform.position = pos;
        GetComponent<SpriteRenderer>().sprite = sprites[UnityEngine.Random.Range(0, sprites.Length)];
        float scale = UnityEngine.Random.Range(minScale, maxScale);
        GetComponent<Transform>().localScale = new Vector3(scale, scale, 0);
        rb.mass *= 5;
    }

    public void Enable()
    {
        gameObject.SetActive(true);
        rb.constraints = RigidbodyConstraints2D.None;
    }

    public void Disable()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.SetActive(false);
    }
}
