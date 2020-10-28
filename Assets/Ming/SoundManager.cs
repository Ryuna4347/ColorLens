using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    private AudioSource source;
    public AudioClip Btn, Back, ClearBtnAppear, StarAppear, Clear, Division, Multiply, Restart, GameOver, Die,DisAppear,Move;

    public float savedBGM = 1;
    public float savedSE = 1;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play(string clipName,float volume=1,float pitch=1)
    {
        AudioClip clip = null;
        switch (clipName)
        {
            case "Btn":
                clip = Btn;
                break;
            case "Back":
                clip = Back;
                break;
            case "ClearBtnAppear":
                clip = ClearBtnAppear;
                break;
            case "StarAppear":
                clip = StarAppear;
                break;
            case "Clear":
                clip = Clear;
                break;
            case "Division":
                clip = Division;
                break;
            case "Multiply":
                clip = Multiply;
                break;
            case "Restart":
                clip = Restart;
                break;
            case "GameOver":
                clip = GameOver;
                break;
            case "Die":
                clip = Die;
                break;
            case "DisAppear":
                clip = DisAppear;
                break;
            case "Move":
                clip = Move;
                break;
        }
        source.volume = volume*savedSE;
        source.pitch = pitch;
        source.PlayOneShot(clip);
    }
}
