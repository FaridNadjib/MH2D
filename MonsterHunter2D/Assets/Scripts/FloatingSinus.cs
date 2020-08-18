using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class will move the gameobject either in horizontal or vertical direction using sinus and time. Farid.
/// </summary>
public class FloatingSinus : MonoBehaviour
{
    #region Fields
    [Header("How it floats:")]
    [Tooltip("The floating intervall.")]
    [SerializeField] float speed;
    [Tooltip("The amount to move on the Y-axis.")]
    [SerializeField] float height;
    [Tooltip("The amount to move on the X-axis.")]
    [SerializeField] float width;
    float fluctuation;
    Vector3 newPos = new Vector3();
    #endregion

    // Update is called once per frame
    void Update()
    {
        // Calculates the new position of the gameobject.
        newPos = transform.position;
        fluctuation = Mathf.Sin(Time.time * speed);
        newPos.x += fluctuation * width * Time.deltaTime;
        newPos.y += fluctuation * height * Time.deltaTime;
        transform.position = newPos;
    }
}
