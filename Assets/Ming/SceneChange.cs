using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "Title")
                Application.Quit();
            else if (SceneManager.GetActiveScene().name == "StageSelect")
                SceneManager.LoadScene("Title");
            else if (SceneManager.GetActiveScene().name == "chap1" || SceneManager.GetActiveScene().name == "chap2" || SceneManager.GetActiveScene().name == "chap3")
                SceneManager.LoadScene("StageSelect");
        }
    }

    public void ChangeScene(string SceneName)
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Btn", 1, 1);
        SceneManager.LoadScene(SceneName);
    }
    public void Back(string SceneName)
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Back",1,1);
        SceneManager.LoadScene(SceneName);
    }
    public void Exit()
    {
        SoundManager.instance.Play("Back",1,1);
        Application.Quit();
    }

    public void Restart()
    {
        SoundManager.instance.Play("Restart",1,1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextStage()
    {
        string[] chapStage = SceneManager.GetActiveScene().name.Split('-');
        string nextStageName = chapStage[0] +'-' + (int.Parse(chapStage[1])+1);
        Time.timeScale = 1;
        SceneManager.LoadScene(nextStageName);
    }

    public void Move(int dir)
    {
        GameManager.instance.MoveBtn(dir);
    }
}
