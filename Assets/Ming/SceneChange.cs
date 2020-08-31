using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void ChangeScene(string SceneName)
    {
        Time.timeScale = 1;
        SoundManager.instance.Play("Btn",1,1);
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
        Time.timeScale = 1;
        SceneManager.LoadScene((int.Parse(SceneManager.GetActiveScene().name) + 1).ToString());
    }

    public void Move(int dir)
    {
        GameManager.instance.MoveBtn(dir);
    }
}
