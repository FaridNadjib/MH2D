using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class handles our mainmenu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    #region Fields
    [Header("The different panels:")]
    [SerializeField] GameObject menuButtons;
    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject storyPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] AudioClip buttonActivation;
    [SerializeField] AudioClip buttonDeactivation;
    [SerializeField] Button continueButton;

    AudioSource source;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        // Checks if there are saves.
        if(continueButton != null)
        {
            if (!PlayerPrefs.HasKey("HasSaveData") || (PlayerPrefs.HasKey("HasSaveData") && PlayerPrefs.GetInt("HasSaveData") == 0))
                continueButton.enabled = false;
            else
                continueButton.enabled = true;
        }
    }

    /// <summary>
    /// Calls the method from the gameManager instance. Since its not destroyed on load, the reference got lost. Therefore these small methods.
    /// </summary>
    public void StartNewGame()
    {
        GameManager.instance.StartNewGame();
    }

    /// <summary>
    /// Calls the method from the gameManager instance. Since its not destroyed on load, the reference got lost. Therefore these small methods.
    /// </summary>
    public void ContinueGame()
    {
        GameManager.instance.ContinueGame();
    }

    /// <summary>
    /// Calls the method from the gameManager instance. Since its not destroyed on load, the reference got lost. Therefore these small methods.
    /// </summary>
    public void ExitGame()
    {
        GameManager.instance.EndGame();
    }


    /// <summary>
    /// Shows the menubuttons.
    /// </summary>
    public void ShowMenuButtons()
    {
        DeavtivatePanels();
        menuButtons.SetActive(true);
    }

    /// <summary>
    /// Shows the control panel.
    /// </summary>
    public void ShowControlPanel()
    {
        DeavtivatePanels();
        controlPanel.SetActive(true);
    }

    /// <summary>
    /// Shows the storypanel.
    /// </summary>
    public void ShowStoryPanel()
    {
        DeavtivatePanels();
        storyPanel.SetActive(true);
    }

    /// <summary>
    /// Shows the creditpanel.
    /// </summary>
    public void ShowCreditsPanel()
    {
        DeavtivatePanels();
        creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Deactivats all panels from the mainmenu.
    /// </summary>
    void DeavtivatePanels()
    {
        menuButtons.SetActive(false);
        controlPanel.SetActive(false);
        storyPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Plays a buttonactivation sound.
    /// </summary>
    public void PlayButtonsActivationSound()
    {
        if (source != null)
            source.PlayOneShot(buttonActivation);
    }

    /// <summary>
    /// Plays a button deactivation sound.
    /// </summary>
    public void PlayButtonsDeactivationSound()
    {
        if (source != null)
            source.PlayOneShot(buttonDeactivation);
    }
}
