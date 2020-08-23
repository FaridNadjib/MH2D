using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// This class handles our ingame ui. It shows the player the character stats and lets him open the esc menu.
/// </summary>
public class UIManager : MonoBehaviour
{
    #region Fields
    [Header("Player Stats related:")]
    [SerializeField] TextMeshProUGUI crystals;
    [SerializeField] Image healthBarFill;
    [SerializeField] RectTransform hBBG;
    [SerializeField] RectTransform hBHL;
    [SerializeField] TextMeshProUGUI numberOfLives;
    [SerializeField] Image staminaBarFill;
    [SerializeField] RectTransform sBBG;
    [SerializeField] RectTransform sBHL;
    [SerializeField] Image invisibleBar;
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

    [Header("GameOverScreen:")]
    [SerializeField] GameObject gameOverScreen;

    AudioSource source;
    PlayerController player;

    float timer;
    bool showPopUpMessage = false;
    #endregion

    #region Singleton
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // Set some defaults and events to subscribe to:
        selectionMarkerWeapon1.color = Color.green;
        munitionAmount1.text = playerStats.MaxArrows.ToString();
        munitionAmount2.text = playerStats.MaxSpears.ToString();
        numberOfLives.text = playerStats.NumberOfHearts.ToString();
        crystals.text = $"{playerStats.CurrentCrystals.ToString()} x";
        invisibleBar.fillAmount = 1f;
        source = GetComponent<AudioSource>();


        if (characterResources != null)
        {
            characterResources.OnHpChanged += (float fillAmount) => { healthBarFill.fillAmount = fillAmount; };
            characterResources.OnStaminaChanged += (float fillAmount) => { staminaBarFill.fillAmount = fillAmount; };
        }

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player.OnInvisibleChanged += (float invisibleTime) => { invisibleBar.fillAmount = invisibleTime; };

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

        GetPlayer();
    }

    /// <summary>
    /// This method is called once every time a new scene has loaded. Its called from the GameManager and sets some defaults and links the needed references.
    /// </summary>
    public void UpdateUI()
    {
        selectionMarkerWeapon1.color = Color.green;
        munitionAmount1.text = playerStats.MaxArrows.ToString();
        munitionAmount2.text = playerStats.MaxSpears.ToString();
        numberOfLives.text = playerStats.NumberOfHearts.ToString();
        crystals.text = $"{playerStats.CurrentCrystals.ToString()} x";

        hBBG.sizeDelta = new Vector2(playerStats.MaxHealth * 5, hBBG.rect.height);
        hBHL.sizeDelta = new Vector2(playerStats.MaxHealth * 5, hBHL.rect.height);
        healthBarFill.rectTransform.sizeDelta = new Vector2(playerStats.MaxHealth * 5, healthBarFill.rectTransform.rect.height);
        healthBarFill.fillAmount = 1f;

        sBBG.sizeDelta = new Vector2(playerStats.MaxStamina * 5, sBBG.rect.height);
        sBHL.sizeDelta = new Vector2(playerStats.MaxStamina * 5, sBHL.rect.height);
        staminaBarFill.rectTransform.sizeDelta = new Vector2(playerStats.MaxStamina * 5, staminaBarFill.rectTransform.rect.height);
        staminaBarFill.fillAmount = 1f;

        invisibleBar.fillAmount = 1f;
    }

    /// <summary>
    /// Plays a buttonSound and calls the corresponding method from the GameManager.
    /// </summary>
    public void RetryLevel()
    {
        // Todo: set crystals to 0.
        source.Play();
        GameManager.instance.RetryLevel();
    }
    /// <summary>
    /// Plays a buttonSound and calls the corresponding method from the GameManager.
    /// </summary>
    public void BackToHub()
    {
        source.Play();
        GameManager.instance.BackToHub();
    }
    /// <summary>
    /// Plays a buttonSound and calls the corresponding method from the GameManager.
    /// </summary>
    public void SaveAndExit()
    {
        source.Play();
        GameManager.instance.SaveAndExit();
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

    /// <summary>
    /// Update the ammo display.
    /// </summary>
    /// <param name="newAmount">The new amount.</param>
    /// <param name="hand">The and the weapon is in.</param>
    public void AmmoChanged(string newAmount, ActiveWeaponHand hand)
    {
        if (hand == ActiveWeaponHand.Left)
            munitionAmount1.text = newAmount;
        else
            munitionAmount2.text = newAmount;
    }

    /// <summary>
    /// Shows messages in the dialog panel. Gets activated by the story manager.
    /// </summary>
    /// <param name="charIcon">The character speaking.</param>
    /// <param name="message">The text.</param>
    /// <param name="speaker">The character speaking.</param>
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

    /// <summary>
    /// Activates the dialog panel.
    /// </summary>
    /// <param name="status">Activate or deactivate.</param>
    public void EnableDialogPanel(bool status)
    {
        dialogPanel.SetActive(status);
    }

    /// <summary>
    /// Show the retry menu.
    /// </summary>
    public void ShowRetryMenu()
    {
        retryButton.SetActive(!retryButton.activeSelf);
        backButton.SetActive(!backButton.activeSelf);
    }

    /// <summary>
    /// Shows the back to hub button.
    /// </summary>
    public void ShowHubMenu()
    {
        saveExitButton.SetActive(!saveExitButton.activeSelf);
    }

    /// <summary>
    /// Hides the escape menu.
    /// </summary>
    public void HideEscapeMenu()
    {
        retryButton.SetActive(false);
        backButton.SetActive(false);
        saveExitButton.SetActive(false);
    }

    /// <summary>
    /// Since its dontdestroy on load, get all the references you need and subscribe to all events you need.
    /// </summary>
    public void GetPlayer()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            characterResources = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterResources>();
            characterResources.OnHpChanged += (float fillAmount) => { healthBarFill.fillAmount = fillAmount; };
            characterResources.OnStaminaChanged += (float fillAmount) => { staminaBarFill.fillAmount = fillAmount; };

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

            player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            player.OnInvisibleChanged += (float invisibleTime) => { invisibleBar.fillAmount = invisibleTime;  };
        }
    }

    /// <summary>
    /// Shows the game over screen.
    /// </summary>
    public void ShowGameOverScreen(bool show)
    {
        gameOverScreen.SetActive(show);
    }
}
