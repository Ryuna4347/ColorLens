using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePanel : MonoBehaviour
{
    public List<GameObject> pauseCharacters;

    private void OnEnable()
    {
        pauseCharacters[Random.Range(0, pauseCharacters.Count)].SetActive(true);
    }

    private void OnDisable()
    {
        pauseCharacters.Find(x => x.activeSelf == true).SetActive(false);    
    }


    public void ResumeStage()
    {
        SoundManager.instance.Play("Btn");
        GameManager.instance.CheckPause();
    }
}
