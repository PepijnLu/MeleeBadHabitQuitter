using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomButtonBehaviour : MonoBehaviour
{
    Dictionary<string, int> buttonPressesPerSecond;
    [SerializeField] AudioSource buzzerSound;
    [SerializeField] StateHandler stateHandler;

    void Start()
    {
        buttonPressesPerSecond = new()
        {
            ["aPress"] = 0,
            ["bPress"] = 0,
            ["yPress"] = 0,
            ["xPress"] = 0
        };

        StartCoroutine(CountDown());
    }

    public void RegisterButtonPress(string _button, bool pressed)
    {
        if(pressed && buttonPressesPerSecond.ContainsKey(_button)) buttonPressesPerSecond[_button]++;

        if(_button == "yPress" || _button == "xPress")
        {
            // if((buttonPressesPerSecond["yPress"] + buttonPressesPerSecond["xPress"] >= GameData.jumpThreshold) && GameData.jumpDebugging)
            // {
            //     buttonPressesPerSecond["yPress"] = 0;
            //     buttonPressesPerSecond["xPress"] = 0;
            //     PlayAudioAtRandomPitch(buzzerSound);
            // }

            if(stateHandler.inHitStun && !buzzerSound.isPlaying && GameData.jumpDebugging) PlayAudioAtRandomPitch(buzzerSound);
        }
        else if((buttonPressesPerSecond["aPress"] >= GameData.aThreshold) && GameData.aDebugging)
        {
            buttonPressesPerSecond[_button] = 0;
            PlayAudioAtRandomPitch(buzzerSound);
        }
        else if((buttonPressesPerSecond["bPress"] >= GameData.bThreshold) && GameData.bDebugging)
        {
            buttonPressesPerSecond[_button] = 0;
            PlayAudioAtRandomPitch(buzzerSound);
        }
    }

    IEnumerator CountDown()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            List<string> keys = new List<string>(buttonPressesPerSecond.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                string key = keys[i];
                if(buttonPressesPerSecond[key] > 0) buttonPressesPerSecond[key]--;
            }
        }
    }

    void PlayAudioAtRandomPitch(AudioSource _audioSource)
    {
        _audioSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        _audioSource.time = 0.2f;
        _audioSource.Play();
    }   
}
