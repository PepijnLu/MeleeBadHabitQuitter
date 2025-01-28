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
    [SerializeField] InputHandler inputHandler;
    //[SerializeField] ulong logicalAddressHex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ulong inputAddress = 0x804C1FAC;
        ulong netplayAddress = 0x80005614 + 0x01;

        for(int i = 0; i < GameData.port; i++)
        {
            inputAddress += (ulong)i * 0x44;
        }

        //LogInputs(0x804C1FAC, 4);
        Log(inputAddress, 4, "inputs");
        Log(netplayAddress, 4, "netplay");
    }

    void Log(ulong _logicalAddressHex, uint _size, string log)
    {
        uint LogicalBase = 0x80000000;

        // Example offset calculation base (e.g., physical starts at 0xC0000000 for uncached)
        ulong PhysicalBase = 0x33FFF0000;
        //UnityEngine.Debug.Log($"Logical Address: {_logicalAddressHex}");

        ulong offset = _logicalAddressHex - LogicalBase;
        //UnityEngine.Debug.Log($"Offset: {offset}");
        ulong physicalAddress = PhysicalBase + offset;

        Process targetProcess = Process.GetProcessesByName("Slippi Dolphin")[0];
        IntPtr address = (IntPtr)physicalAddress; // Replace with the target memory address
        //uint size = 4; // Number of bytes to read

        byte[] memoryData = MemoryAccess.ReadMemory(targetProcess.Id, address, _size);
        //UnityEngine.Debug.Log($"Memory Text: {Encoding.ASCII.GetString(memoryData)}");

        if(log == "inputs") LogInputs(memoryData);
        if(log == "netplay") LogNetplay(memoryData);

    }

    void LogInputs(byte[] _memoryData)
    {
        List<string> inputs = new();

        if(_memoryData != null) 
        {
            inputs = controllerBindings.GetInputsOfFrame(_memoryData);
        }

        inputHandler.CheckInputs(inputs);
    }

    void LogNetplay(byte[] _memoryData)
    {
        string memDataString = BitConverter.ToString(_memoryData);
        UnityEngine.Debug.Log("memDataString:  " +  memDataString);
    }
}
