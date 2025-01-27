using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    bool aActive, jumpActive;
    public Image aImage, jumpImage, aSpamWarning, jumpSpamWarning;
    public Sprite defaultSprite, checkedSprite;
    bool resettingA, resettingJump;
    public AudioSource buzzerSound;
    public void AButton()
    {
        ActivateA(!aActive);
    }
    public void JumpButton()
    {
        ActivateJump(!jumpActive);
    }

    void ActivateA(bool activate)
    {
        aActive = activate;
        GameData.aDebugging = aActive;

        if(activate)
        {
            aImage.sprite = checkedSprite;
            aSpamWarning.gameObject.SetActive(true);
        }   
        else
        {
            aImage.sprite = defaultSprite;
            aSpamWarning.gameObject.SetActive(false);
        }
    }

    void ActivateJump(bool activate)
    {
        jumpActive = activate;
        GameData.jumpDebugging = jumpActive;

        if(activate)
        {
            jumpImage.sprite = checkedSprite;
            jumpSpamWarning.gameObject.SetActive(true);
        }   
        else
        {
            jumpImage.sprite = defaultSprite;
            jumpSpamWarning.gameObject.SetActive(false);
        }
    }

    public void SpamWarning(string _action)
    {
        Debug.Log("Spam Warning! " + _action);

        if(_action == "A")
        {
            if(!resettingA)
            {
                Color color = aSpamWarning.color; 
                color.a = 1; 
                aSpamWarning.color = color; 
                PlayAudioAtRandomPitch(buzzerSound);
                StartCoroutine(ResetSpamWarning(_action));
            }
        }

        if(_action == "Jump")
        {
            if(!resettingJump)
            {
                Color color = jumpSpamWarning.color; 
                color.a = 1; 
                jumpSpamWarning.color = color;
                PlayAudioAtRandomPitch(buzzerSound);
                StartCoroutine(ResetSpamWarning(_action));
            }
        }
    }

    IEnumerator ResetSpamWarning(string _action)
    {
        if(_action == "A") resettingA = true;
        if(_action == "Jump") resettingJump = true;

        yield return new WaitForSeconds(1.5f);

        if(_action == "A")
        {
            Color color = aSpamWarning.color; 
            color.a = .2f; 
            aSpamWarning.color = color; 
        }
        if(_action == "Jump")
        {
            Color color = jumpSpamWarning.color; 
            color.a = .2f; 
            jumpSpamWarning.color = color; 
        }

        if(_action == "A") resettingA = false;
        if(_action == "Jump") resettingJump = false;
    }

    void PlayAudioAtRandomPitch(AudioSource _audioSource)
    {
        _audioSource.pitch = UnityEngine.Random.Range(0.5f, 1.5f);
        _audioSource.time = 0.2f;
        _audioSource.Play();
    }   
}
