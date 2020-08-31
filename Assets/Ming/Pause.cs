using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)||Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0) //일시정지상태일때 
            {
                pausePanel.SetActive(false); //일시정지 품   
                Time.timeScale = 1;
            }
            else
            {
                pausePanel.SetActive(true);
                Time.timeScale = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) //R키 누르면 현재 씬 재시작
        {
            SoundManager.instance.Play("Restart",1,1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
