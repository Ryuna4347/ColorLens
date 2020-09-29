using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void Save(int starNum)
    {
        string sceneName= SceneManager.GetActiveScene().name;
        if(PlayerPrefs.GetInt(sceneName, 0)<=starNum) 
            PlayerPrefs.SetInt(sceneName,starNum);
    }
}
