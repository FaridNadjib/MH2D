using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

/// <summary>
/// An array of symbols that can be changed and light up. Everytime a symbol is changed, the sequence of symbols will be check by the referenced Door.
/// </summary>
public class ChangingSymbol : MonoBehaviour
{
    [SerializeField] private SymbolPuzzlePlatform door;

    [Header("Sprite")]
    [SerializeField] private GameObject[] symbols;
    [SerializeField] private int activeSymbolIndex;
    [SerializeField] private int neededSymbolIndex;
    public bool neededSymbolActive = false;

    [Header("Light")]
    [SerializeField] private float standardIntensity;
    [SerializeField] private float triggeredIntensity;
    [SerializeField] private float maxChangingIntensity;
    [SerializeField] private float transitionTime;
    private Light2D spriteLight;
    private float currentTime = 0f;

    private bool changing = false;
    private bool inTrigger = false;
    private bool transitionBack = false;


    private void Awake() 
    {
        symbols[activeSymbolIndex].SetActive(true);
        spriteLight = GetComponentInChildren<Light2D>();
        spriteLight.intensity = standardIntensity;

        if (activeSymbolIndex == neededSymbolIndex)
            neededSymbolActive = true;
    }

    private void Update()
    {
        if (inTrigger && Input.GetKeyDown(KeyCode.E) && !changing)
            ChangeSymbol();

        if (!changing)
            return;

        if (!transitionBack)
            Transition();
        else
            TransitionBack();

 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inTrigger = true;

            if (!changing)
                spriteLight.intensity = triggeredIntensity;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.GetComponent<PlayerController>() != null)
        {
            inTrigger = false;

            if (!changing)
                spriteLight.intensity = standardIntensity;

        }
    }

    /// <summary>
    /// Changes the sprite-light to the next symbol. Then calls the door function to let it check if the sequence of symbols are correct.
    /// </summary>
    private void ChangeSymbol()
    {
        changing = true;

        symbols[activeSymbolIndex].SetActive(false);

        if (activeSymbolIndex == symbols.Length - 1)
            activeSymbolIndex = 0;
        else    
            activeSymbolIndex++;

        symbols[activeSymbolIndex].SetActive(true);
        spriteLight = symbols[activeSymbolIndex].GetComponent<Light2D>();

        if (activeSymbolIndex == neededSymbolIndex)
            neededSymbolActive = true;
        else
            neededSymbolActive = false;

        door.CheckSymbol();
    }

    /// <summary>
    /// Transitions the light to maxIntensity over time by using a lerp.
    /// </summary>
    private void Transition()
    {
        currentTime += Time.deltaTime;

        float t = currentTime / transitionTime;

        if (t < 1)
            spriteLight.intensity = Mathf.Lerp(triggeredIntensity, maxChangingIntensity, t);
        else if (t >= 1)
        {
            currentTime = 0f;
            transitionBack = true;
        }
    }

    /// <summary>
    /// Transitions the light to standardIntensity over time by using a lerp.
    /// </summary>
    private void TransitionBack()
    {
        currentTime += Time.deltaTime;

        float t = currentTime / transitionTime;

        if (t < 1)
            spriteLight.intensity = Mathf.Lerp(maxChangingIntensity, standardIntensity, t);
        else if (t >= 1)
        {
            changing = false;
            transitionBack = false;
            currentTime = 0f;

        }
    }
}
