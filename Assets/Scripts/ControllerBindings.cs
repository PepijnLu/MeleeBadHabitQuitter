using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControllerBindings : MonoBehaviour
{
    public List<int> GenerateList(int _size)
    {
        List<int> generatedList = new();
        int intToAdd = 1;

        for (int i = 0; i < _size; i++)
        {
            generatedList.Add(intToAdd);
            intToAdd *= 2;
        }

        return generatedList;
    }

    public List<string> GetInputsOfFrame(byte[] _memoryData)
    {
        //string memDataString = BitConverter.ToString(_memoryData);
        //Debug.Log("Memory Data String: " + memDataString);
        
        List<string> inputs = new();

        for(int i = 0; i < _memoryData.Length; i++)
        {
            int inputData = _memoryData[i];
            if(inputData != 0) Debug.Log($"Input Data: {inputData} at {i}");

            switch(i)
            {
                case 1:
                    List<string> stickInputs = GetStickDirections(inputData);
                    foreach(string _inp in stickInputs)
                    {
                        if(!_inp.Contains("None"))
                        {
                            inputs.Add(_inp);
                        };
                    }
                    break;
                case 2:
                    List<string> faceButtonInputs = GetFaceButtonInputs(inputData);
                    foreach(string _inp in faceButtonInputs)
                    {
                        if(!_inp.Contains("None"))
                        {
                            inputs.Add(_inp);
                        };
                    }
                    break;
                case 3:
                    List<string> dpadTriggerInputs = GetDpadTriggerInputs(inputData);
                    foreach(string _inp in dpadTriggerInputs)
                    {
                        if(!_inp.Contains("None"))
                        {
                            inputs.Add(_inp);
                        };
                    }
                    break;
                default:
                    break;
            }
        }

        return inputs;
    }

    public List<string> GetStickDirections(int _inputData)
    {
        List<int> stickDirections = FindCombination(GenerateList(8), _inputData);
        List<string> directions = new();
        
        foreach(int _dir in stickDirections)
        {
            switch(_dir)
            {
                case 1:
                    directions.Add("joyStickUp");
                    break;
                case 2:
                    directions.Add("joyStickDown");
                    break;
                case 4:
                    directions.Add("joyStickLeft");
                    break;
                case 8:
                    directions.Add("joyStickRight");
                    break;
                case 16:    
                    directions.Add("cStickUp");
                    break;
                case 32:
                    directions.Add("cStickDown");
                    break;
                case 64:
                    directions.Add("cStickLeft");
                    break;
                case 128:
                    directions.Add("cStickRight");
                    break;
            }   
        }

        return directions;
    }

    public List<string> GetFaceButtonInputs(int _inputData)
    {
        List<int> faceButtonInputs = FindCombination(GenerateList(5), _inputData);
        List<string> faceButtonPresses = new();
        
        foreach(int _prs in faceButtonInputs)
        {
            switch(_prs)
            {
                case 1:
                    faceButtonPresses.Add("aPress");
                    break;
                case 2:
                    faceButtonPresses.Add("bPress");
                    break;
                case 4:
                    faceButtonPresses.Add("xPress");
                    break;
                case 8:
                    faceButtonPresses.Add("yPress");
                    break;
                case 16:    
                    faceButtonPresses.Add("startPress");
                    break;
            }   
        }

        return faceButtonPresses;
    }

    public List<string> GetDpadTriggerInputs(int _inputData)
    {
        List<int> dpadTriggerInputs = FindCombination(GenerateList(7), _inputData);
        List<string> dPadTriggerPresses = new();
        
        foreach(int _prs in dpadTriggerInputs)
        {
            switch(_prs)
            {
                case 1:
                    dPadTriggerPresses.Add("dpadLeft");
                    break;
                case 2:
                    dPadTriggerPresses.Add("dpadRight");
                    break;
                case 4:
                    dPadTriggerPresses.Add("dpadDown");
                    break;
                case 8:
                    dPadTriggerPresses.Add("dpadUp");
                    break;
                case 16:    
                    dPadTriggerPresses.Add("Z");
                    break;
                case 32:    
                    dPadTriggerPresses.Add("R");
                    break;
                case 64:    
                    dPadTriggerPresses.Add("L");
                    break;
            }   
        }

        return dPadTriggerPresses;
    }

    List<int> FindCombination(List<int> _availableInts, int target)
    {
        List<int> pressedButtons = new();

        List<int> result = GetCombination(_availableInts, target);

        if (result.Count > 0)
        {
            Debug.Log("The number " + target + " is made up of:");
            foreach (int num in result)
            {
                pressedButtons.Add(num);
            }
        }
        else
        {
            Debug.Log("No combination found for the target.");
        }

        return pressedButtons;
    }

    List<int> GetCombination(List<int> availableInts, int target)
    {
        List<int> result = new List<int>();
        bool found = FindCombination(availableInts, target, 0, new List<int>(), result);

        return found ? result : new List<int>(); // Return the combination if found, else return empty list
    }

    bool FindCombination(List<int> availableInts, int target, int startIndex, List<int> currentCombination, List<int> result)
    {
        if (target == 0)
        {
            result.Clear();
            result.AddRange(currentCombination); // If the target is 0, we've found a valid combination
            return true;
        }

        for (int i = startIndex; i < availableInts.Count; i++)
        {
            if (availableInts[i] <= target) // Only consider the current number if it's less than or equal to the target
            {
                currentCombination.Add(availableInts[i]); // Add number to current combination
                if (FindCombination(availableInts, target - availableInts[i], i + 1, currentCombination, result))
                {
                    return true; // If a valid combination is found, return true
                }
                currentCombination.RemoveAt(currentCombination.Count - 1); // Backtrack if not valid
            }
        }

        return false; // Return false if no combination is found
    }
}
