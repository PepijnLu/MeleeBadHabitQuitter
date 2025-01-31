using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;
public class MemoryManager : MonoBehaviour
{
    [SerializeField] ControllerBindings controllerBindings;
    [SerializeField] StateOffsets stateOffsets;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] StateHandler stateHandler;
    [SerializeField] GUIHandler guiHandler;
    ulong physicalCharacterDataAddress;
    ulong physicalMatchInfoStructAddress;
    ulong phyicalCurrentMenuAddress;
    ulong basePhyiscalInputAddress;
    ulong physicalNetplayAddress, physicalNetplayPointerAddress;
    ulong matchPlayingAddress;
    ulong phyiscalInputAddress;
    ulong physicalIsConnectedAddress;
    ulong hitstunAddress;
    ulong actionStateAddress;
    int portLastFrame, localPlayerIndexNetplay;
    bool netplayMatchStarted;
    Process targetProcess;

    //[SerializeField] ulong logicalAddressHex;
    // Start is called before the first frame update
    void Start()
    {
        phyicalCurrentMenuAddress = GetPhysicalAddress(0x8065CC14);
        physicalMatchInfoStructAddress = GetPhysicalAddress(0x8046B6A0, "matchInfo");
        matchPlayingAddress = stateOffsets.AddOffsetToAddress("Match Playing", physicalMatchInfoStructAddress);
        physicalNetplayAddress = GetPhysicalAddress(0x810E5F60);

        /*
        //Index 0: 0 default, switches between 0 and 179 in unranked menu, 26 after going VS mode, switches back after online menu
        //Index 1: 0 default, 32 after going VS mode
        //Index 2: 0 default, 28 after going VS mode, 1 during VS screen
        //Index 3: 0 default, 32 after going VS mode

        During P1 Game:
        Index 0 = 4
        Index 1 = 1
        Index 2 = 1
        Index 3 = 0

        During P2 Game:
        Index 0 = 4
        Index 1 = 1
        Index 2 = 1
        Index 3 = 1
        */
    }   

    void StartGame()
    {
        basePhyiscalInputAddress = GetPhysicalAddress(0x804C1FAC, "playerInput");
        GameData.gameRunning = true;

        UnityEngine.Debug.Log("Game Started");
        guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("waitingForGameImg"), false);
    }

    void EndGame()
    {
        GameData.gameRunning = false;
        UnityEngine.Debug.Log("Game Stopped");
        guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("waitingForGameImg"), true);
    }


    // Update is called once per frame
    void Update()
    {     
        if(targetProcess == null)
        {   
            UnityEngine.Debug.Log("FindingProgram");
            Process[] slippiDolphin = Process.GetProcessesByName("Slippi Dolphin");
            if(slippiDolphin.Length > 0) 
            {
                targetProcess = slippiDolphin[0];
                targetProcess.EnableRaisingEvents = true;
                targetProcess.Exited += (sender, e) =>
                {
                    UnityEngine.Debug.Log($"Process {targetProcess} has exited.");
                    targetProcess = null;
                };
            }
            return;
        }

        GetByteArrayAtAddress(phyicalCurrentMenuAddress, 1, "CheckIfGameRunning");

        if(GameData.gameRunning)
        {
            if(GameData.port != portLastFrame)
            {
                UpdatePort();
            }

            if(GameData.inMatch)
            {
                GetByteArrayAtAddress(hitstunAddress, 1, "ReadHitstun");
                GetByteArrayAtAddress(actionStateAddress, 4, "ReadKnockdown");
            }

            GetByteArrayAtAddress(phyiscalInputAddress, 4, "ReadInputs");
            GetByteArrayAtAddress(physicalIsConnectedAddress, 1, "CheckIfConnected");
            GetByteArrayAtAddress(matchPlayingAddress, 4, "CheckIfInMatch");
            GetByteArrayAtAddress(physicalNetplayAddress, 4, "CheckNetplay");
            //13 = 1, 14 = 0 if not ready
            //13 = 2, 14 = 1 if ready

            portLastFrame = GameData.port;
        }
    }

    void UpdatePort()
    {
        //Offset for port
        phyiscalInputAddress = basePhyiscalInputAddress;
        for(int i = 0; i < GameData.port - 1; i++)
        {
            phyiscalInputAddress += 0x44;
        }
        physicalIsConnectedAddress = stateOffsets.AddOffsetToAddress("Is Connected", phyiscalInputAddress);
        guiHandler.portText.text = GameData.port.ToString();
        //Untested
        StartCoroutine(TryGetCharacterDataLocation());
    }


    byte[] GetByteArrayAtAddress(ulong _address, uint _size, string log)
    {
        byte[] memoryData = MemoryAccess.ReadMemory(targetProcess.Id, (IntPtr)_address, _size);
        LogData(memoryData, log);
        return memoryData;
    }

    ulong GetPhysicalAddress(ulong _logicalAddressHex, string desiredLocation = "")
    {
        UnityEngine.Debug.Log("Logical Address Hex: " + _logicalAddressHex);
        uint LogicalBase = 0x80000000;

        // Example offset calculation base (e.g., physical starts at 0xC0000000 for uncached)
        ulong PhysicalBase = 0x33FFF0000;
        //UnityEngine.Debug.Log($"Logical Address: {_logicalAddressHex}");

        ulong offset = _logicalAddressHex - LogicalBase;
        //UnityEngine.Debug.Log($"Offset: {offset}");
        ulong physicalAddress = PhysicalBase + offset;

        UnityEngine.Debug.Log(desiredLocation + ": Physical Address Found: " + physicalAddress);

        return physicalAddress; // Replace with the target memory address
    }

    void RegisterDigitalInputs(byte[] _memoryData)
    {
        List<string> inputs = new();

        if(_memoryData != null) 
        {
            inputs = controllerBindings.GetInputsOfFrame(_memoryData);
        }

        inputHandler.CheckInputs(inputs);
    }

    void LogData(byte[] _memoryData, string log)
    {

        for(int i = 0; i < _memoryData.Length; i++)
        {
            UnityEngine.Debug.Log($"memData: {log}: Index: {i}, Data; {_memoryData[i]}");
        }

        if(log == "ReadHitstun") stateHandler.RegisterHitStun(_memoryData);
        if(log == "ReadKnockdown") stateHandler.RegisterMissedTech(_memoryData);
        if(log == "ReadInputs") RegisterDigitalInputs(_memoryData);
        if(log == "CheckIfConnected") inputHandler.CheckIfConnected(_memoryData);
        if(log == "CheckIfInMatch") CheckRegularMatchState(_memoryData);
        if(log == "CheckIfGameRunning") CheckIfGameRunning(_memoryData);
        if(log == "CheckNetplay") CheckNetplay(_memoryData);
        
    }

    void CheckNetplay(byte[] _memoryData)
    {
        //In netplay match
        if(_memoryData[0] == 4)
        {
            if(!GameData.inNetplay)
            {
                GameData.localPort = GameData.port;
                GameData.inNetplay = true;
                guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("netplayImg"), true);
                // if(!netplayMatchStarted)
                // {
                //     netplayMatchStarted = true;
                //     StartMatch();
                // }
            }
        }
        else if (_memoryData[0] != 179)
        {
            if(GameData.inNetplay)
            {
                GameData.inNetplay = false;
                GameData.port = GameData.localPort;
                UnityEngine.Debug.Log("Local Port: " + GameData.port);
                UpdatePort();
                guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("netplayImg"), false);
                //if(netplayMatchStarted) netplayMatchStarted = false;
            }
        }

        if(!GameData.inNetplay) return;

        //PlayerIndex 1
        if(_memoryData[3] == 0)
        {
            localPlayerIndexNetplay = 0;
        }
        //PlayerIndex 2
        else if (_memoryData[3] == 1)
        {
            localPlayerIndexNetplay = 1;
        }
        else throw new Exception("Netplay Player Index Data Unexpected");
    }
    void CheckRegularMatchState(byte[] _memoryData)
    {

        if(_memoryData[3] == 0)
        {
            if(GameData.inMatch == false) UpdateMatchState(true);
        }
        else
        {
            if(GameData.inMatch == true) UpdateMatchState(false);
        };
    }

    void CheckIfGameRunning(byte[] _memoryData)
    {
        if(_memoryData[0] != 0)
        {
            {
                if(!GameData.gameRunning) 
                {
                    StartGame();
                }
            }
        }
        else
        {
            if(GameData.gameRunning) 
            {
                EndGame();
            }
        }
    }

    void StartMatch()
    {
        UnityEngine.Debug.Log("Start Match");
        StartCoroutine(TryGetCharacterDataLocation());
    }

    void UpdateMatchState(bool _inMatch)
    {
        GameData.inMatch = _inMatch;

        if(_inMatch)
        {
            StartMatch();
        }
        else
        {
            EndMatch();
        }
    }

    void EndMatch()
    {
        stateHandler.inHitStun = false;
        stateHandler.missedTech = false;

        guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("hitstunImg"), false);
        guiHandler.ToggleUIImage(guiHandler.GetImageFromDicrionary("knockdownImg"), false);
    }

    public static string GetHexadecimal(ulong value)
    {
        // Convert ulong to hexadecimal
        return value.ToString("X");
    }

    IEnumerator TryGetCharacterDataLocation()
    {
        physicalCharacterDataAddress = GetPhysicalCharacterDataLocation();

        while(physicalCharacterDataAddress == 11811094528)
        {
            yield return new WaitForSeconds(0.1f);
            physicalCharacterDataAddress = GetPhysicalCharacterDataLocation();
        }

        hitstunAddress = stateOffsets.AddOffsetToAddress("InHitstun", physicalCharacterDataAddress);
        actionStateAddress = stateOffsets.AddOffsetToAddress("Action State", physicalCharacterDataAddress);
    }
    ulong GetPhysicalCharacterDataLocation()
    {
        ulong logicalStartStaticPlayerBlock = stateOffsets.playerStateAddress;

        //Offset for port
        if(!GameData.inNetplay)
        {
            for(int i = 0; i < GameData.port - 1; i++)
            {
                logicalStartStaticPlayerBlock += 0xE90;
            }
        }
        else
        {
            logicalStartStaticPlayerBlock += ((ulong)localPlayerIndexNetplay * 0xE90);
            UnityEngine.Debug.Log("Local Port: " + GameData.localPort);
            GameData.port = localPlayerIndexNetplay + 1;
            UpdatePort();
        }

        ulong logicalPlayerEntityPointerAddress = stateOffsets.AddOffsetToAddress("PlayerEntityPointer", logicalStartStaticPlayerBlock);

        ulong phyiscalPlayerEntityPointerAddress = GetPhysicalAddress(logicalPlayerEntityPointerAddress); 

        byte[] playerEntityPointer = GetByteArrayAtAddress(phyiscalPlayerEntityPointerAddress, 4, "offsetPlayerEntityPointer");

        ulong logicalPlayerEntityAddress = stateOffsets.GetAddressFromPointer(playerEntityPointer);

        ulong physicalPlayerEntityAdress = GetPhysicalAddress(logicalPlayerEntityAddress);

        ulong physicalCharacterDataPointerAddress = stateOffsets.AddOffsetToAddress("CharacterDataPointer", physicalPlayerEntityAdress);

        byte[] characterDataPointer = GetByteArrayAtAddress(physicalCharacterDataPointerAddress, 4, "offsetCharacterDataPointer");

        ulong logicalCharacterDataAddress = stateOffsets.GetAddressFromPointer(characterDataPointer);
 
        ulong physicalCharacterDataAddress = GetPhysicalAddress(logicalCharacterDataAddress);

        UnityEngine.Debug.Log("Found Physical Address: " + physicalCharacterDataAddress);
        //11811094528 means not found

        return physicalCharacterDataAddress;
    }
}
