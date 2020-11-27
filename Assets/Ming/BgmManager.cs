using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmManager : MonoBehaviour
{
    public static BgmManager instance;
    private AudioSource source;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            source = GetComponent<AudioSource>();
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (SoundManager.instance.isSoundbgm)
            source.volume = SoundManager.instance.savedBGM;
        else
            source.volume = 0;
    }

    public void Set(AudioClip clip, float volume = 1f)
    {
        if (source.clip != clip)
        {
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
    }
}
