using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class moves some sprite in relation to the camera movement to achieve a parallax effect. Used this tutorial for it: https://www.youtube.com/watch?v=wBol2xzxCOU
/// Farid.
/// </summary>
public class Parallax : MonoBehaviour
{
    #region Fields
    [SerializeField] GameObject cam;
    [SerializeField] Vector2 parallaxEffectMultilpier;
    private float textureUnitSizeX;

    Vector3 oldCamPos;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Get some defaults.
        oldCamPos = cam.transform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Calculate how much the texture should be moved in ralation to how the camera moved. X axis only.
        Vector3 deltaMovement = cam.transform.position - oldCamPos;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultilpier.x, deltaMovement.y * parallaxEffectMultilpier.y, 0);

        oldCamPos = cam.transform.position;

        if(Mathf.Abs(cam.transform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (cam.transform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(cam.transform.position.x + offsetPositionX, transform.position.y);
        }
    }
}
