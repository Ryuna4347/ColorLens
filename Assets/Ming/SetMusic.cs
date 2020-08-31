using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetMusic : MonoBehaviour
{
    public AudioClip music;
    public float volume=1;
    void Start()
    {
        BgmManager.instance.Set(music,volume);
    }
    
}
