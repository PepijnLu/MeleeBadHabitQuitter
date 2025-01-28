using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEditor.Rendering;

public class GUIHandler : MonoBehaviour
{
    [SerializeField] Image aButtonImg, bButtonImg, xButtonImg, yButtonImg, userADebug, userBDebug, userXDebug, userYDebug;
    Dictionary<string, Image> guiImages;
    [SerializeField] TMP_InputField aThresholdInp, bThresholdInp, jumpThresholdInp;
    [SerializeField] TextMeshProUGUI portText;
    // Start is called before the first frame update
    void Start()
    {
        guiImages = new()
        {
            ["aPress"] = aButtonImg,
            ["bPress"] = bButtonImg,
            ["yPress"] = yButtonImg,
            ["xPress"] = xButtonImg
        };

        aThresholdInp.onValueChanged.AddListener((text) => OnTextChanged(aThresholdInp, text));
        bThresholdInp.onValueChanged.AddListener((text) => OnTextChanged(bThresholdInp, text));
        jumpThresholdInp.onValueChanged.AddListener((text) => OnTextChanged(jumpThresholdInp, text));

        OnTextChanged(aThresholdInp, aThresholdInp.text);
        OnTextChanged(bThresholdInp, bThresholdInp.text);
        OnTextChanged(jumpThresholdInp, jumpThresholdInp.text);

        if (int.TryParse(portText.text, out int number) && number < 5) GameData.port = number;
        else throw new System.Exception("Port not found");
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTextChanged(TMP_InputField inputField, string text)
    {
        Debug.Log($"Input Field '{inputField.name}' Text Changed: {text}");

        // Filter out non-numeric characters
        string result = Regex.Replace(text, "[^0-9]", "");
        Debug.Log($"Filtered Text: {result}");
        inputField.text = result;
        
        if(inputField == aThresholdInp)
        {
            if (int.TryParse(result, out int number)) GameData.aThreshold = number;
        }
        else if (inputField == bThresholdInp)
        {
            if (int.TryParse(result, out int number)) GameData.bThreshold = number;
        }
        else if (inputField == jumpThresholdInp)
        {
            if (int.TryParse(result, out int number)) GameData.jumpThreshold = number;
        }
    }

    public void UpdateButtonPress(string _button, bool _pressed)
    {
        if(guiImages.ContainsKey(_button))
        {
            //guiImages[_button].gameObject.SetActive(_pressed);
            ToggleUIImage(guiImages[_button], _pressed);
        }
    }

    public void SwitchADebugging()
    {
        if(GameData.aDebugging)
        {
            GameData.aDebugging = false;
        }
        else
        {
            GameData.aDebugging = true;
        }

        ToggleUIImage(userADebug, GameData.aDebugging);
    }

    public void SwitchBDebugging()
    {
        if(GameData.bDebugging)
        {
            GameData.bDebugging = false;
        }
        else
        {
            GameData.bDebugging = true;
        }

        ToggleUIImage(userBDebug, GameData.bDebugging);
    }

    public void SwitchJumpDebugging()
    {
        if(GameData.jumpDebugging)
        {
            GameData.jumpDebugging = false;
        }
        else
        {
            GameData.jumpDebugging = true;
        }

        ToggleUIImage(userXDebug, GameData.jumpDebugging);
        ToggleUIImage(userYDebug, GameData.jumpDebugging);
    }

    void ToggleUIImage(Image _img, bool on)
    {
        if(on) _img.gameObject.SetActive(true);
        else _img.gameObject.SetActive(false);
    }

    public void IncreasePort()
    {
        GameData.port++;
        if(GameData.port >= 5) GameData.port = 1;
        portText.text = GameData.port.ToString();
    }

    public void DecreasePort()
    {
        GameData.port--;
        if(GameData.port <= 0) GameData.port = 4;
        portText.text = GameData.port.ToString();
    }
}
