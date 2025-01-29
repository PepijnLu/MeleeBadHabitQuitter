using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [SerializeField] GUIHandler guiHandler;
    [SerializeField] CustomButtonBehaviour customButtonBehaviour;
    List<string> inputsPreviousFrame = new();
    Dictionary<string, bool> buttonPresses = new()
    {
        ["aPress"] = false,
        ["bPress"] = false,
        ["xPress"] = false,
        ["yPress"] = false,
        ["startPress"] = false,

        ["lPress"] = false,
        ["rPress"] = false,
        ["zPress"] = false,

        ["dpadDown"] = false,
        ["dpadUp"] = false,
        ["dpadLeft"] = false,
        ["dpadRight"] = false,
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called every frame
    public void CheckInputs(List<string> _inputs)
    {
        //OnButtonPress
        foreach (string _input in _inputs)
        {
            if(!inputsPreviousFrame.Contains(_input))
            {
                SetBool(_input, true);
            }
        }

        //OnButtonRelease
        foreach (string _input in inputsPreviousFrame)
        {
            if(!_inputs.Contains(_input))
            {
                SetBool(_input, false);
            }
        }

        inputsPreviousFrame = _inputs;
    }

    public void SetBool(string key, bool value)
    {
        // Check if the key exists
        if (buttonPresses.ContainsKey(key))
        {
            bool currentValue = buttonPresses[key];
            if (currentValue != value) // Detect the flip
            {
                buttonPresses[key] = value;

                // Call a method when the bool flips
                OnBoolFlipped(key, value);
            }
        }
        else
        {
            // If the key doesn't exist, add it
            Debug.Log($"Key '{key}' added with value {value}");
            buttonPresses[key] = value;
        }
    }

    public bool GetBool(string key)
    {
        if (buttonPresses.ContainsKey(key))
        {
            return buttonPresses[key];
        }
        Debug.LogWarning($"Key '{key}' not found. Returning false as default.");
        return false;
    }

    private void OnBoolFlipped(string key, bool newValue)
    {
        guiHandler.UpdateButtonPress(key, newValue);
        customButtonBehaviour.RegisterButtonPress(key, newValue);
    }
}
