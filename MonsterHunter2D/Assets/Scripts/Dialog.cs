using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// This class sets the base datatype we use for our storymanager. Every dialogline has a speaker and a text, an arry from that makes the dialog and the story manager has an arry of every dialog. Farid.
/// </summary>
[System.Serializable]
public class Dialog
{
    [Serializable]
    public struct DialogLines
    {
        public CharacterIcon speaker;
        public string text;
    }
    public DialogLines[] dialogs;
}
