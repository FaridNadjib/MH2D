using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SymbolPuzzlePlatform : PressurePlatform
{
    [SerializeField] private ChangingSymbol[] neededSymbols;

    private bool activated = false;

    protected override void Update()
    {
        if (activated && transform.position == endPos.position || !activated && transform.position == startPos.position)
            return;

        if (activated)
            Extend();
        else
            Reset();

        CameraEvent();
    }

    public void CheckSymbol()
    {
        int count = 0;

        for (int i = 0; i < neededSymbols.Length; i++)
        {
            if (neededSymbols[i].neededSymbolActive)
                count++;
        }

        if (count == neededSymbols.Length)
        {
            activated = true;
            cameraEventActive = true;
            objectCam.enabled = true;
        }
        else
        {
            activated = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other) {}
    protected override void OnCollisionExit2D(Collision2D other) {}
}
