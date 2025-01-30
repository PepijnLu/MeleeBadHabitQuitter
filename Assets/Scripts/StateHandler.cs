using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHandler : MonoBehaviour
{
    [SerializeField] GUIHandler guiHandler;
    public bool inHitStun, missedTech;

    public void RegisterHitStun(byte[] _memory)
    {
        bool activate = false;
        if(_memory[0] == 128) activate = true;
        else if(_memory[0] == 0) activate = false;

        if(inHitStun != activate) 
        {
            inHitStun = activate;
            if(GameData.jumpDuringHitstunDebugging) guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("hitstunImg"), activate);
        }
    }

    public void RegisterMissedTech(byte[] _memory)
    {
        bool activate;
        Debug.Log("Hitstun Byte: " + _memory[3]);
        if(_memory[3] == 191 || _memory[3] == 183) activate = true;
        else activate = false;

        if(missedTech != activate) 
        {
            missedTech = activate;
            if(GameData.attackAfterMissedTechDebugging) guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("knockdownImg"), activate);
        }
    }
}
