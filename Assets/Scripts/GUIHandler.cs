using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEditor.Rendering;

public class GUIHandler : MonoBehaviour
{
    [SerializeField] Image aButtonImg, bButtonImg, xButtonImg, yButtonImg, zButtonImg, startPressImg, hitstunImg, netplayImg, waitingForGameImg, knockdownImg, connectedImg, disconnectedImg, dpadDownImg, dpadUpImg, dpadLeftImg, dpadRightImg, attackAfterMissedTechCheck, jumpDuringHitstunCheck;
    Dictionary<string, Image> guiImages;
    [SerializeField] public TextMeshProUGUI portText, fpsText;
    private float fps;
    // Start is called before the first frame update
    void Awake()
    {
        guiImages = new()
        {
            ["aPress"] = aButtonImg,
            ["bPress"] = bButtonImg,
            ["yPress"] = yButtonImg,
            ["xPress"] = xButtonImg,
            ["zPress"] = zButtonImg,
            ["startPress"] = startPressImg,

            ["hitstunImg"] = hitstunImg,
            ["knockdownImg"] = knockdownImg,

            ["dpadDown"] = dpadDownImg,
            ["dpadUp"] = dpadUpImg,
            ["dpadLeft"] = dpadLeftImg,
            ["dpadRight"] = dpadRightImg,

            ["connectedImg"] = connectedImg,
            ["disconnectedImg"] = disconnectedImg,
            ["waitingForGameImg"] = waitingForGameImg,
            ["netplayImg"] = netplayImg,
        };

        if (int.TryParse(portText.text, out int number) && number < 5) GameData.port = number;
        else throw new System.Exception("Port not found");

    }

    void Start()
    {
        if(jumpDuringHitstunCheck.gameObject.activeSelf) GameData.jumpDuringHitstunDebugging = true;
        if(attackAfterMissedTechCheck.gameObject.activeSelf) GameData.attackAfterMissedTechDebugging = true;
    }

    void Update()
    {
        // fps = Mathf.Lerp(fps, 1.0f / Time.unscaledDeltaTime, 0.1f);
        // fpsText.text = "FPS: " + Mathf.RoundToInt(fps).ToString();
    }

    public void UpdateButtonPress(string _button, bool _pressed)
    {
        if(guiImages.ContainsKey(_button))
        {
            //guiImages[_button].gameObject.SetActive(_pressed);
            ToggleUIImage(guiImages[_button], _pressed);
        }
    }

    public void SwitchAttackAfterMissedTechDebugging()
    {
        if(GameData.attackAfterMissedTechDebugging)
        {
            GameData.attackAfterMissedTechDebugging = false;
        }
        else
        {
            GameData.attackAfterMissedTechDebugging = true;
        }

        ToggleUIImage(attackAfterMissedTechCheck, GameData.attackAfterMissedTechDebugging);
    }

    public void SwitchJumpDuringHitstunDebugging()
    {
        if(GameData.jumpDuringHitstunDebugging)
        {
            GameData.jumpDuringHitstunDebugging = false;
        }
        else
        {
            GameData.jumpDuringHitstunDebugging = true;
        }

        ToggleUIImage(jumpDuringHitstunCheck, GameData.jumpDuringHitstunDebugging);
    }

    public void ToggleUIImage(Image _img, bool on)
    {
        if(on) _img.gameObject.SetActive(true);
        else _img.gameObject.SetActive(false);
    }

    public void IncreasePort()
    {
        if(!GameData.inNetplay)
        {
            GameData.port++;
            if(GameData.port >= 5) GameData.port = 1;
            Debug.Log("Current Port: " + GameData.port);
            portText.text = GameData.port.ToString();
            GameData.localPort = GameData.port;
        }
    }

    public void DecreasePort()
    {
        if(!GameData.inNetplay)
        {
            GameData.port--;
            if(GameData.port <= 0) GameData.port = 4;
            Debug.Log("Current Port: " + GameData.port);
            portText.text = GameData.port.ToString();
            GameData.localPort = GameData.port;
        }
    }

    public Image GetImageFromDicrionary(string _img)
    {
        if(guiImages.ContainsKey(_img)) return guiImages[_img];
        else return null;
    }
}
