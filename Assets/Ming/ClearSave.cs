using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSave : MonoBehaviour
{
    public static ClearSave instance;
    public string[] stageKeys;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void Save(int stageNum, int starNum)
    {
        if(PlayerPrefs.GetInt(stageKeys[stageNum-1],0)<=starNum) 
            PlayerPrefs.SetInt(stageKeys[stageNum-1],starNum);
    }
}
