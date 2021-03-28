using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGM_SE_MGr : MonoBehaviour
{
    private float bgmvalue, sevalue;
    string bgmkey="bgmkey";
    private string sekey = "sekey";
    public Slider BGM;
    public Slider SE;
    public GameObject Panel;
    private bool isSoundbgm = true;
    private string soundKeybgm = "soundkeybgm";
    private bool isSoundbgs = true;
    private string soundKeybgs = "soundkeybgs";
    public GameObject bgm1, bgm2, bgs1, bgs2;
    private void Start()
    {
        bgmvalue = PlayerPrefs.GetFloat(bgmkey, 1f);
        sevalue = PlayerPrefs.GetFloat(sekey, 1f);
        BGM.value = bgmvalue;
        SE.value = sevalue;

        if (PlayerPrefs.GetInt(soundKeybgs, 0) == 0)
        {
            bgs1.SetActive(true);
            bgs2.SetActive(false);
            isSoundbgs = true;
        }
        else
        {
            bgs2.SetActive(true);
            bgs1.SetActive(false);
            isSoundbgs = false;
        }

        if (PlayerPrefs.GetInt(soundKeybgm, 0) == 0)
        {
            bgm1.SetActive(true);
            bgm2.SetActive(false);
            isSoundbgm = true;
        }
        else
        {
            bgm2.SetActive(true);
            bgm1.SetActive(false);
            isSoundbgm = false;
        }
        
        
        SoundManager.instance.isSoundbgs = isSoundbgs;
        SoundManager.instance.isSoundbgm = isSoundbgm;
    }

    public void SetBGM()
    {
        SoundManager.instance.savedBGM = BGM.value;
        PlayerPrefs.SetFloat(bgmkey,BGM.value);
    }

    public void SetSE()
    {
        SoundManager.instance.savedSE = SE.value;
        PlayerPrefs.SetFloat(sekey,SE.value);
    }

    public void Open()
    {
        if(Panel.activeSelf) 
            Panel.SetActive(false);
        else
            Panel.SetActive(true);
    }

    public void SetBgmSound()
    {
        isSoundbgm = !isSoundbgm;
        SoundManager.instance.isSoundbgm = isSoundbgm;
        if(isSoundbgm)
            PlayerPrefs.SetInt(soundKeybgm,0);
        else
            PlayerPrefs.SetInt(soundKeybgm,1);
        if (isSoundbgm)
        {
            bgm1.SetActive(true);
            bgm2.SetActive(false);
        }
        else
        {
            bgm2.SetActive(true);
            bgm1.SetActive(false);
        }
    }

    public void SetBgsSound()
    {
        isSoundbgs = !isSoundbgs;
        SoundManager.instance.isSoundbgs = isSoundbgs;
        if(isSoundbgs)
            PlayerPrefs.SetInt(soundKeybgs,0);
        else
            PlayerPrefs.SetInt(soundKeybgs,1);
        if (isSoundbgs)
        {
            bgs1.SetActive(true);
            bgs2.SetActive(false);
        }
        else
        {
            bgs2.SetActive(true);
            bgs1.SetActive(false);
        }
    }
}
