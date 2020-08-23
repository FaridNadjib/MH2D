using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used in the story manager to display the correct character sprite.
/// </summary>
public enum CharacterIcon { Women, Hero, FlyDino, Ankylosaurus, Bat }

/// <summary>
/// This class manages our story dialogs. Farid.
/// </summary>
public class StoryManager : MonoBehaviour
{
    #region Fields
    [SerializeField] Sprite[] characterIcons;
    [SerializeField] Dialog[] dialogs;
    [SerializeField] PlayerController player;

    bool inDialog;
    int dialogIndex = -1;
    int lineCounter = 0;
    bool continueDialog = true;
    #endregion

    // Update is called once per frame
    void Update()
    {
        // Go through the line of the triggerd dialog index from the dialog array.
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
                    player.SetKinematic(false);
                }
            }

            // Continue the dialog.
            if (Input.GetKeyUp(KeyCode.E))
                continueDialog = true;
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
            player.SetKinematic(true);
        }
        else
        {
            dialogIndex = -1;
            UIManager.instance.EnableDialogPanel(false);
            player.BlockInput(false);
            player.SetKinematic(false);
        }
    }
}
