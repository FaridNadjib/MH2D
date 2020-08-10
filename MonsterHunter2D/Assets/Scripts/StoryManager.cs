using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CharacterIcon { Women, Hero, FlyDino, Ankylosaurus, Bat }
public class StoryManager : MonoBehaviour
{
    [SerializeField] Sprite[] characterIcons;
    [SerializeField] Dialog[] dialogs;

    [SerializeField] PlayerController player;
    bool inDialog;
    int dialogIndex = -1;
    int lineCounter = 0;
    bool continueDialog = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inDialog)
        {
            if (continueDialog)
            {
                if(lineCounter < dialogs[dialogIndex].dialogs.Length)
                {
                    UIManager.instance.ShowDialog(characterIcons[(int)dialogs[dialogIndex].dialogs[lineCounter].speaker], dialogs[dialogIndex].dialogs[lineCounter].text, dialogs[dialogIndex].dialogs[lineCounter].speaker);
                    lineCounter++;
                    continueDialog = false;
                }else if(lineCounter >= dialogs[dialogIndex].dialogs.Length)
                {
                    inDialog = false;
                    dialogIndex = -1;
                    UIManager.instance.EnableDialogPanel(false);
                    player.BlockInput(false);

                }
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                continueDialog = true;
            }
        }
    }

    /// <summary>
    /// Triggers the dialog with index from the saved dialogs.
    /// </summary>
    /// <param name="index">The dialog to trigger.</param>
    public void TriggerDialog(int index)
    {
        if(index >=0 && index < dialogs.Length)
        {
            dialogIndex = index;
            lineCounter = 0;
            inDialog = true;
            UIManager.instance.EnableDialogPanel(true);
            player.BlockInput(true);
        }
        else
        {
            dialogIndex = -1;
            UIManager.instance.EnableDialogPanel(false);
            player.BlockInput(false);
        }
    }
}
