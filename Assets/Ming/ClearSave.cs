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
        else if(Input.GetKeyDown(KeyCode.F2))
        {
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j <= 12; j++)
                {
                    PlayerPrefs.SetInt(i.ToString()+"-"+j.ToString(),3);
                }
            }
        }
    }

    public void Save(int starNum)
    {
        string sceneName= SceneManager.GetActiveScene().name;
        if(PlayerPrefs.GetInt(sceneName, 0)<=starNum) 
            PlayerPrefs.SetInt(sceneName,starNum);
    }
}
