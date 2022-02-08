using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public GameObject tutorialCanvas;
    public GameObject creditCanvas;
    private string sceneName;

    private void Start()
    {
        sceneName = SceneManager.GetActiveScene().name;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (sceneName.Equals("Title"))
            {
                if (tutorialCanvas.activeSelf == true)
                {
                    tutorialCanvas.SetActive(false);
                }
                else if(creditCanvas.activeSelf == true)
                {
                    creditCanvas.SetActive(false);
                }
                else
                {
                    Application.Quit();
                }
            }
            else if (sceneName.Equals("ChapterSelect"))
                SceneManager.LoadScene("Title");
            else if (sceneName.Equals("chap1") || sceneName.Equals("chap2") || sceneName.Equals("chap3"))
                SceneManager.LoadScene("ChapterSelect");
        }
    }

    public void ChangeScene(string scene)
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Btn", 1, 1);
        if (scene.Equals("ChapterStage"))
        {
            string chapterNum = sceneName.Split('-')[0];
            SceneManager.LoadScene("Chap"+chapterNum);
        }
        else
        {
            SceneManager.LoadScene(scene);
        }
    }
    public void Back(string scene)
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Back",1,1);
        SceneManager.LoadScene(scene);
    }
    public void Exit()
    {
        SoundManager.instance.Play("Back",1,1);
        Application.Quit();
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Restart",1,1);
        SceneManager.LoadScene(sceneName);
    }
    public void NextStage()
    {
        string nextStageName;
        string[] chapStage = sceneName.Split('-');
        if (chapStage[1] != "12")
        {
            nextStageName = chapStage[0] + '-' + (int.Parse(chapStage[1]) + 1);
        }
        else
        {
            nextStageName = Convert.ToString(int.Parse(chapStage[0])+1) + "-1";
        }
        Time.timeScale = 1;
        SoundManager.instance.Play("Btn", 1, 1);
        SceneManager.LoadScene(nextStageName);
    }

}
