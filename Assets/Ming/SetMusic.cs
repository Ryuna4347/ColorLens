using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetMusic : MonoBehaviour
{
    public AudioClip music;
    public float volume=1;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "1-1")
        {
            BgmManager.instance.Set(null, volume);
        }
        else
        {
            BgmManager.instance.Set(music, volume);
        }
    }
    
    public void PlayBGM()
    {
        BgmManager.instance.Set(music, volume);
    }
}
