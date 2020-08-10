using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
