using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetMusic : MonoBehaviour
{
    public AudioClip music;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "1-1")
        {
            BgmManager.instance.Set(null,SoundManager.instance.savedBGM);
        }
        else
        {
#if UNITY_EDITOR
            if (SceneManager.GetActiveScene().name.Equals("MapTest"))
                return;
#endif
            BgmManager.instance.Set(music, SoundManager.instance.savedBGM);
        }
    }
    
    public void PlayBGM()
    {
        BgmManager.instance.Set(music, SoundManager.instance.savedBGM);
    }
    
}
