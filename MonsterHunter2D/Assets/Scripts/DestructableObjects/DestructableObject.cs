using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    [Header("The gameobject with the destructable parts:")]
    [SerializeField] GameObject destroyedParts;
    [SerializeField] SpriteRenderer[] partImages;
    [SerializeField] float deactivationTime;
    [SerializeField] float dissappearTime;
    float timer = 0f;
    float dissappearTimer;
    Color oldColor;
    Color newColor;

    bool startDeactivation = false;
    Collider2D col;
    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        oldColor = spriteRenderer.color;
        newColor = new Color(oldColor.r,oldColor.g, oldColor.b,0);
    }

    // Update is called once per frame
    void Update()
    {
        if (startDeactivation)
        {
            if (timer < deactivationTime - dissappearTime)
                timer += Time.deltaTime;
            else if(timer < deactivationTime)
            {
                dissappearTimer += Time.deltaTime;
                for (int i = 0; i < partImages.Length; i++)
                {
                    partImages[i].color = Color.Lerp(oldColor, newColor, dissappearTimer / dissappearTime);
                }
                timer += Time.deltaTime;
            }
            else
            {
                // Disable or destroy the destroyed gam object.
                destroyedParts.SetActive(false);
            }
        }
    }

    public void ActivateDestruction()
    {
        if (col != null)
            col.enabled = false;
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;
        destroyedParts.SetActive(true);

        startDeactivation = true;
    }

    
}
