using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player Stats related:")]
    [SerializeField] TextMeshProUGUI crystals;
    [SerializeField] Image healthBarFill;
    [SerializeField] RectTransform hBBG;
    [SerializeField] RectTransform hBHL;
    [SerializeField] TextMeshProUGUI numberOfLives;
    [SerializeField] Image staminaBarFill;
    [SerializeField] RectTransform sBBG;
    [SerializeField] RectTransform sBHL;
    [SerializeField] Image weapon1Icon;
    [SerializeField] Image weapon2Icon;
    [SerializeField] Image selectionMarkerWeapon1;
    [SerializeField] Image selectionMarkerWeapon2;
    [SerializeField] TextMeshProUGUI munitionAmount1;
    [SerializeField] TextMeshProUGUI munitionAmount2;
    [SerializeField] CharacterResources characterResources;
    [SerializeField] TextMeshProUGUI popUpMessage;
    [SerializeField] float popUpShowTime;
    [SerializeField] PlayerStats playerStats;

    [Header("Dialog related:")]
    [SerializeField] GameObject dialogPanel;
    [SerializeField] Image characterIcon1;
    [SerializeField] Image characterIcon2;
    [SerializeField] TextMeshProUGUI dialogText;

    [Header("Escape Menu related:")]
    [SerializeField] GameObject retryButton;
    [SerializeField] GameObject backButton;
    [SerializeField] GameObject saveExitButton;

    float timer;
    bool showPopUpMessage = false;

    public static UIManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        // Set some defaults:
        selectionMarkerWeapon1.color = Color.green;
        munitionAmount1.text = playerStats.MaxArrows.ToString();
        munitionAmount2.text = playerStats.MaxSpears.ToString();
        numberOfLives.text = playerStats.CurrentNumberOfHearts.ToString();
        crystals.text = $"{playerStats.CurrentCrystals.ToString()} x";
        

        if(characterResources != null)
        {
            characterResources.OnHpChanged += (float fillAmount) => { healthBarFill.fillAmount = fillAmount; };
            characterResources.OnStaminaChanged += (float fillAmount) => { staminaBarFill.fillAmount = fillAmount; };
        }

        PlayerWeaponChanger.instance.OnWeaponChanged += (ActiveWeaponType activeWeapon, ActiveWeaponHand activeHand, Sprite weaponIcon) => {
            if (activeHand == ActiveWeaponHand.Left)
            {
                weapon1Icon.sprite = weaponIcon;
                selectionMarkerWeapon1.color = Color.green;
                selectionMarkerWeapon2.color = Color.yellow;
            }
            else
            {
                weapon2Icon.sprite = weaponIcon;
                selectionMarkerWeapon2.color = Color.green;
                selectionMarkerWeapon1.color = Color.yellow;
            }
        };

        playerStats.OnStatsChanged += () => {
            crystals.text = $"{playerStats.CurrentCrystals.ToString()} x";
            numberOfLives.text = playerStats.NumberOfHearts.ToString();

            hBBG.sizeDelta = new Vector2(playerStats.MaxHealth * 5, hBBG.rect.height);
            hBHL.sizeDelta = new Vector2(playerStats.MaxHealth * 5, hBHL.rect.height);
            healthBarFill.rectTransform.sizeDelta = new Vector2(playerStats.MaxHealth * 5, healthBarFill.rectTransform.rect.height);

            sBBG.sizeDelta = new Vector2(playerStats.MaxStamina * 5, sBBG.rect.height);
            sBHL.sizeDelta = new Vector2(playerStats.MaxStamina * 5, sBHL.rect.height);
            staminaBarFill.rectTransform.sizeDelta = new Vector2(playerStats.MaxStamina * 5, staminaBarFill.rectTransform.rect.height);

        };
    }

    public void UpdateUI()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Count how long a popup message should be shown on the screen.
        if (showPopUpMessage)
            if (timer < popUpShowTime)
                timer += Time.deltaTime;
            else
            {
                timer = 0;
                showPopUpMessage = false;
                popUpMessage.enabled = false;
            }
    }

    /// <summary>
    /// This method shows a popup text;
    /// </summary>
    /// <param name="message">A text displayed to let the palyer know what happened.</param>
    public void ShowPopUpMessage(string message)
    {
        timer = 0f;
        popUpMessage.text = message;
        popUpMessage.enabled = true;
        showPopUpMessage = true;
    }

    public void AmmoChanged(string newAmount, ActiveWeaponHand hand)
    {
        if (hand == ActiveWeaponHand.Left)
            munitionAmount1.text = newAmount;
        else
            munitionAmount2.text = newAmount;
    }

    public void ShowDialog(Sprite charIcon, string message, CharacterIcon speaker)
    {
        if (speaker == CharacterIcon.Women || speaker == CharacterIcon.Hero)
        {
            characterIcon1.sprite = charIcon;
            characterIcon1.enabled = true;
            characterIcon2.enabled = false;
        }
        else
        {
            characterIcon2.sprite = charIcon;
            characterIcon2.enabled = true;
            characterIcon1.enabled = false;
        }

        dialogText.text = message;
    }

    public void EnableDialogPanel(bool status)
    {
        dialogPanel.SetActive(status);
    }


    public void ShowRetryMenu()
    {
        retryButton.SetActive(!retryButton.activeSelf);
        backButton.SetActive(!backButton.activeSelf);
    }

    public void ShowHubMenu()
    {
        saveExitButton.SetActive(!saveExitButton.activeSelf);
    }
}
