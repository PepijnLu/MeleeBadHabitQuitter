using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    [SerializeField] GUIHandler guiHandler;
    public bool inHitStun;

    public void RegisterHitStun(byte[] _memory)
    {
        bool activate = false;
        Debug.Log("First byte: " + _memory[0]);
        if(_memory[0] == 128) activate = true;
        else if(_memory[0] == 0) activate = false;

        if(inHitStun != activate) 
        {
            inHitStun = activate;
            guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("hitstunImg"), activate);
        }
    }
}
