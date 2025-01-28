// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEditor;
// using UnityEngine;
// using System.Diagnostics;


// public class MemoryReader : MonoBehaviour
// {
//     [SerializeField] List<string> pointers;
//     List<IntPtr> intPointers;
//     int processId;

//     // Start is called before the first frame update
//     void Start()
//     {
//         // foreach(string _str in pointers)
//         // {
//         //     IntPtr address = (IntPtr)Convert.ToInt64(_str, 16);
//         //     intPointers.Add(address);
//         // }

//         //memAccess = new();
//         //GetAllProcesses();

//         Process[] processes = Process.GetProcessesByName("Slippi Dolphin");
//         if (processes.Length > 0)
//         {
//             processId = processes[0].Id;
//             UnityEngine.Debug.Log($"Process ID: {processId}");
//         }
//         else
//         {
//             UnityEngine.Debug.Log("Target process not found.");
//         }
//     }

//     void GetAllProcesses()
//     {
//         Process[] processes = Process.GetProcesses();

//         foreach (Process process in processes)
//         {
//             try
//             {
//                 UnityEngine.Debug.Log($"Process Name: {process.ProcessName}, ID: {process.Id}");
//             }
//             catch
//             {
//                 UnityEngine.Debug.Log("Could not access process information.");
//             }
//         }
//     }

//     // Update is called once per frame
//     unsafe void Update()
//     {
//         if(Input.GetKeyDown(KeyCode.Space))
//         {
//             // int testValue = 12345; // Example value to test
//             // int* testPointer = &testValue; // Get the pointer to testValue
//             // ReadMemoryTest((IntPtr)testPointer);

//             // foreach(IntPtr _pointer in intPointers)
//             // {
//             //     ReadMemoryTest(_pointer);
//             // }

//             //IntPtr testAdress = (IntPtr)(0x80453080 + 0xB0 + 0x60 + 0x0620);
//             // IntPtr testAdress = (IntPtr)0x80000000;
//             // ReadMemoryTest(testAdress);

//             //MemoryAccess.AccessMemory(processId, (IntPtr)0x33FFF00000, 16);
//         }

//         //MemoryAccess.AccessMemory(processId, (IntPtr)0x80479D60, 16);
//     }

//     // unsafe void ReadMemoryTest(IntPtr address)
//     // {
//     //     //IntPtr address = (IntPtr)0x80453080;

//     //     try
//     //     {
//     //         byte value = ReadByte(address);
//     //         Debug.Log($"Value at address {address:X}: {value}");
//     //     }
//     //     catch (Exception ex)
//     //     {
//     //         Debug.Log($"Failed to read memory: {ex.Message}");
//     //     }
//     // }

//     unsafe void ReadMemoryTest(IntPtr address)
//     {
//         try
//         {
//             int value = ReadInt32(address);
//             UnityEngine.Debug.Log($"Value at address {address:X}: {value}");
//         }
//         catch (Exception ex)
//         {
//             UnityEngine.Debug.Log($"Failed to read memory at {address}: {ex.Message}");
//         }
//     }

//     unsafe byte ReadByte(IntPtr address)
//     {
//         if (address == IntPtr.Zero)
//         {
//             throw new ArgumentException("Address cannot be null.");
//         }

//         byte* ptr = (byte*)address;
//         return *ptr; // Attempt to dereference
//     }

//     unsafe int ReadInt32(IntPtr address)
//     {
//         if (address == IntPtr.Zero)
//         {
//             throw new ArgumentException("Address cannot be null.");
//         }

//         int* ptr = (int*)address;
//         return *ptr; // Read 4 bytes as an int
//     }

//     unsafe float ReadFloat(IntPtr address)
//     {
//         if (address == IntPtr.Zero)
//         {
//             throw new ArgumentException("Address cannot be null.");
//         }

//         float* ptr = (float*)address;
//         return *ptr; // Read 4 bytes as an int
//     }


// }
