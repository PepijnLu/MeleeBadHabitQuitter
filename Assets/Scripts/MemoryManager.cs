using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
public class MemoryManager : MonoBehaviour
{
    [SerializeField] ControllerBindings controllerBindings;
    [SerializeField] StateOffsets stateOffsets;
    [SerializeField] InputHandler inputHandler;
    [SerializeField] StateHandler stateHandler;
    ulong logicalCharacterDataAddress;
    //[SerializeField] ulong logicalAddressHex;
    // Start is called before the first frame update
    void Start()
    {
        //Run this every new match
        logicalCharacterDataAddress = GetPhysicalCharacterDataLocation();
    }

    // Update is called once per frame
    void Update()
    {
        ulong logicalInputAddress = GetPhysicalAddress(0x804C1FAC);

        ulong physicalTestAddress = stateOffsets.AddOffsetToAddress("InHitstun", logicalCharacterDataAddress);

        ulong logicalNetplayAddress = 0x80005614;
        ulong physicalNetplayAddress = GetPhysicalAddress(logicalNetplayAddress);

        // ulong physicalLocalPlayerIndexAddress = stateOffsets.AddOffsetToAddress("Local Player Ready", physicalNetplayAddress);
        // ulong physicalTestAddress = physicalLocalPlayerIndexAddress;

        for(int i = 0; i < GameData.port; i++)
        {
            logicalInputAddress += (ulong)i * 0x44;
        }

        //Logs Digital Inputs
        byte[] inputsByteArray = GetByteArrayAtAddress(logicalInputAddress, 4, "inputs");
        RegisterDigitalInputs(inputsByteArray);

        //Test
        GetByteArrayAtAddress(physicalTestAddress, 4, "ReadHitstun");


    }

    ulong GetPhysicalCharacterDataLocation()
    {
        ulong logicalPlayerEntityPointerAddress = stateOffsets.AddOffsetToAddress("PlayerEntityPointer", stateOffsets.playerStateAddress);

        ulong phyiscalPlayerEntityPointerAddress = GetPhysicalAddress(logicalPlayerEntityPointerAddress); 

        byte[] playerEntityPointer = GetByteArrayAtAddress(phyiscalPlayerEntityPointerAddress, 4, "offsetPlayerEntityPointer");

        ulong logicalPlayerEntityAddress = stateOffsets.GetAddressFromPointer(playerEntityPointer);

        ulong physicalPlayerEntityAdress = GetPhysicalAddress(logicalPlayerEntityAddress);

        ulong physicalCharacterDataPointerAddress = stateOffsets.AddOffsetToAddress("CharacterDataPointer", physicalPlayerEntityAdress);

        byte[] characterDataPointer = GetByteArrayAtAddress(physicalCharacterDataPointerAddress, 4, "offsetCharacterDataPointer");

        ulong logicalCharacterDataAddress = stateOffsets.GetAddressFromPointer(characterDataPointer);
 
        ulong physicalCharacterDataAddress = GetPhysicalAddress(logicalCharacterDataAddress);

        return physicalCharacterDataAddress;
    }

    byte[] GetByteArrayAtAddress(ulong _address, uint _size, string log)
    {
        Process targetProcess = Process.GetProcessesByName("Slippi Dolphin")[0];
        byte[] memoryData = MemoryAccess.ReadMemory(targetProcess.Id, (IntPtr)_address, _size);

        if(log != "inputs") LogData(memoryData, log);
        //else stateHandler.RegisterHitStun(memoryData);

        return memoryData;

    }

    ulong GetPhysicalAddress(ulong _logicalAddressHex)
    {
        UnityEngine.Debug.Log("Logical Address Hex: " + _logicalAddressHex);
        uint LogicalBase = 0x80000000;

        // Example offset calculation base (e.g., physical starts at 0xC0000000 for uncached)
        ulong PhysicalBase = 0x33FFF0000;
        //UnityEngine.Debug.Log($"Logical Address: {_logicalAddressHex}");

        ulong offset = _logicalAddressHex - LogicalBase;
        //UnityEngine.Debug.Log($"Offset: {offset}");
        ulong physicalAddress = PhysicalBase + offset;

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

        stateHandler.RegisterHitStun(_memoryData);
    }

    public static string GetHexadecimal(ulong value)
    {
        // Convert ulong to hexadecimal
        return value.ToString("X");
    }
}
