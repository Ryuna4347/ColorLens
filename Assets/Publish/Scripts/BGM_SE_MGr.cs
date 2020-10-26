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

    private void Start()
    {
        bgmvalue = PlayerPrefs.GetFloat(bgmkey, 1f);
        sevalue = PlayerPrefs.GetFloat(sekey, 1f);
        BGM.value = bgmvalue;
        SE.value = sevalue;
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
        Panel.SetActive(true);
    }

    public void Close()
    {
        Panel.SetActive(false);
    }
}
