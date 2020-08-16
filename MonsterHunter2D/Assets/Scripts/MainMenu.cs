using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject menuButtons;
    [SerializeField] GameObject controlPanel;
    [SerializeField] GameObject storyPanel;
    [SerializeField] GameObject creditsPanel;
    [SerializeField] AudioClip buttonActivation;
    [SerializeField] AudioClip buttonDeactivation;

    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowMenuButtons()
    {
        DeavtivatePanels();
        menuButtons.SetActive(true);
    }
    public void ShowControlPanel()
    {
        DeavtivatePanels();
        controlPanel.SetActive(true);
    }

    public void ShowStoryPanel()
    {
        DeavtivatePanels();
        storyPanel.SetActive(true);
    }

    public void ShowCreditsPanel()
    {
        DeavtivatePanels();
        creditsPanel.SetActive(true);
    }


    void DeavtivatePanels()
    {
        menuButtons.SetActive(false);
        controlPanel.SetActive(false);
        storyPanel.SetActive(false);
        creditsPanel.SetActive(false);
    }

    public void PlayButtonsActivationSound()
    {
        if (source != null)
            source.PlayOneShot(buttonActivation);
    }

    public void PlayButtonsDeactivationSound()
    {
        if (source != null)
            source.PlayOneShot(buttonDeactivation);
    }
}
