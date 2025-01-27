using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using Unity.VisualScripting;

public class InputReader : MonoBehaviour
{
    private string filePath;
    private long lastFileLength = 0;
    private Dictionary<int, string> inputs;
    private Dictionary<string, int> monitoredInputs;
    private Dictionary<string, bool> monitoredInputBools;
    public int jumpPresses, aPresses, bPresses, increment, updateInt;
    bool holdJumpDown, holdADown, holdBDown;
    public AudioSource dontJump;
    public TextMeshProUGUI updateTxt, jumpTxt;
    public ButtonHandler buttonHandler;
    

    void Start()
    {
        filePath = "C:\\Users\\pepijn\\AppData\\Roaming\\LOVE\\m-overlay\\console.log"; // Path to the JSON file

        inputs = new()
        {
            [100000] = "cU",
            [200000] = "cD",
            [400000] = "cdL",
            [800000] = "cdR",

            [10000] = "jU",
            [20000] = "jD",
            [40000] = "jdL",
            [80000] = "jdR",

            [100] = "A",
            [200] = "B",
            [400] = "X",
            [800] = "Y",

            [40] = "L",
            [20] = "R",
            [10] = "Z"

        };

        monitoredInputs = new()
        {
            ["A"] = 0,
            ["B"] = 0,
            ["Jump"] = 0,
        };

        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found!");
            return;
        }

        // Initialize file length
        lastFileLength = new FileInfo(filePath).Length;

        // Start monitoring the file
        InvokeRepeating(nameof(CheckForUpdates), 0.01f, 0.01f); // Check every second
    }

    void CheckForUpdates()
    {
        FileInfo fileInfo = new FileInfo(filePath);
        long currentLength = fileInfo.Length;

        if (currentLength > lastFileLength)
        {
            // Read only the new data
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                stream.Seek(lastFileLength, SeekOrigin.Begin); // Start reading from the last position
                StreamReader reader = new StreamReader(stream);
                string newContent = reader.ReadToEnd();

                // Process the new content

                //Debug.Log("New JSON data added: " + newContent);

                string parsedString = ParseStringBeforeI(newContent);
                string newParsedString = ParseStringBeforeFirstBracket(parsedString);

                Debug.Log("NEW Parsed string = " + newParsedString);

                int pressedButton = ParseToInt(newParsedString);

                //Debug.Log("pressed button: " + pressedButton);

                if(pressedButton != 0)
                {
                    List<int> buttonInputs = FindPressedButtons(pressedButton);
                    List<string> pressedButtons = new();

                    foreach(int _inp in buttonInputs)
                    {
                        pressedButtons.Add(inputs[_inp]);
                    }

                    MonitorInput(pressedButtons, true);
                }
                else
                {
                    MonitorInput(null, false);
                } 

                LogInput();     
            }

            // Update the tracked file length
            updateInt++;
            if(updateInt >= 1000) updateInt = 0;
            // updateTxt.text = "Update: " + updateInt.ToString();
            // jumpTxt.text = "Jump: " + jumpPresses.ToString();
            lastFileLength = currentLength;
        }
    }

    void LogInput()
    {
        //if(jumpPresses >= 20 && !dontJump.isPlaying) dontJump.Play();

        if(jumpPresses >= 25 && GameData.jumpDebugging)
        {
            buttonHandler.SpamWarning("Jump");
        }  

        if(aPresses >= 25 && GameData.aDebugging)
        {
            buttonHandler.SpamWarning("A");
        }
    }

    void MonitorInput(List<string> _pressedButtons, bool pressed)
    {
        if(pressed)
        {
            if(_pressedButtons.Contains("X") || _pressedButtons.Contains("Y"))
            {
                Debug.Log("Player: Jump");
                if(!holdJumpDown) jumpPresses += increment;
                holdJumpDown = true;
            }
            else
            {
                holdJumpDown = false;
            }

            if(_pressedButtons.Contains("A"))
            {
                Debug.Log("Player: A");
                if(!holdADown) aPresses += increment;
                holdADown = true;
            }
            else
            {
                holdADown = false;
            }

            if(_pressedButtons.Contains("B"))
            {
                Debug.Log("Player: B");
                if(!holdBDown) bPresses += increment;
                holdBDown = true;
            }
            else
            {
                holdBDown = false;
            }
        }
        else
        {
            holdJumpDown = false;
            holdADown = false;
            holdBDown = false;

            if(jumpPresses > 0) jumpPresses--;
            if (aPresses > 0) aPresses--;
            if (bPresses > 0) bPresses--;
        }
    }

    string ParseStringBeforeI(string input)
    {
        int index = input.IndexOf('I'); // Find the first occurrence of 'I'

        if (index != -1) // Check if 'I' is found
        {
            string result = input.Substring(index); // Remove everything before 'I'
            string singleLine = result.Replace("\n", "").Replace("\r", "");
            //Debug.Log("Result: " + result); // Output: "IToRemove"
            return result;
        }
        else
        {
            return "No press";
        }
    }

    string ParseStringBeforeFirstBracket(string input)
{
    int index = input.IndexOf('['); // Find the first occurrence of '.'

    if (index != -1) // Check if '.' is found
    {
        string result = input.Substring(0, index); // Get everything before the first '.'
        string singleLine = result.Replace("\n", "").Replace("\r", ""); // Remove newlines
        return singleLine;
    }
    else
    {
        return input; // Return the original string if no '.' is found
    }
}

    int ParseToInt(string input)
    {
        // Remove non-numeric characters (except for the minus sign)
        string numericString = "";
        foreach (char c in input)
        {
            if (char.IsDigit(c) || c == '-') // Keep digits and minus for negative numbers
            {
                numericString += c;
            }
        }

        // Try to parse the numeric part to an integer
        int parsedInt = 0;
        if (int.TryParse(numericString, out parsedInt))
        {
            return parsedInt;
        }
        else
        {
            return 0; // Return 0 or some default value if parsing fails
        }
    }

    List<int> FindPressedButtons(int target)
    {
        List<int> pressedButtons = new();

        if(inputs.ContainsKey(target))
        {
            pressedButtons.Add(target);
            return pressedButtons;
        }

        List<int> availableInts = new();
        foreach(var kvp in inputs)
        {
            availableInts.Add(kvp.Key);
        }

        List<int> result = GetCombination(availableInts, target);

        if (result.Count > 0)
        {
            Debug.Log("The number " + target + " is made up of:");
            foreach (int num in result)
            {
                Debug.Log(num);
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
