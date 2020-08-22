using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] GameObject cam;
    [SerializeField] Vector2 parallaxEffectMultilpier;
    private float textureUnitSizeX;
    private float textureUnitSizeY;

    public float parallaxEffectX;
    public float parallaxEffectY;
    public bool useParaX, useParaY;


    float dist;
    float heightDist;
    Vector3 newPos = new Vector3();


    Vector3 oldCamPos;

    // Start is called before the first frame update
    void Start()
    {
        oldCamPos = cam.transform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void LateUpdate()
    {

        //float temp = (cam.transform.position.x * (1 - parallaxEffectX));

        //dist = (cam.transform.position.x * parallaxEffectX);
        //heightDist = (cam.transform.position.y * parallaxEffectY);

        //newPos.Set(startposX + dist, transform.position.y, transform.position.z);
        //if (useParaX)
        //    transform.position = newPos;

        //newPos.Set(transform.position.x, startposY + heightDist, transform.position.z);
        //if (useParaY)
        //    transform.position = newPos;

        //Debug.Log(temp);
        //Debug.Log(lengthX);
        //if (temp > startposX + lengthX)
        //    startposX += lengthX;
        //else if (temp < startposX - lengthX)
        //    startposX -= lengthX;

        Vector3 deltaMovement = cam.transform.position - oldCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultilpier.x, deltaMovement.y * parallaxEffectMultilpier.y, 0);

        oldCamPos = cam.transform.position;

        if(Mathf.Abs(cam.transform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cam.transform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cam.transform.position.x + offsetPositionX, transform.position.y);
        }

        //if (Mathf.Abs(cam.transform.position.y - transform.position.y) >= textureUnitSizeY)
        //{
        //    float offsetPositionY = (cam.transform.position.y - transform.position.y) % textureUnitSizeY;
        //    transform.position = new Vector3(transform.position.x, cam.transform.position.y + offsetPositionY);
        //}
    }
}
