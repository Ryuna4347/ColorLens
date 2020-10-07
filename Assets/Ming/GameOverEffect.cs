using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverEffect : MonoBehaviour
{
    public static GameOverEffect instance;
    public float AppearSpeed;
    public Image panel;
   
    public Animator playerAnim;
    public Text GameOverText;
    public Animator BG;
    public Animator Text;
    public Animator[] Btns;
    public Animator BrokenStar;
    public GameObject gameOverPanel;
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        //if(Input.GetKeyDown(KeyCode.X))
        //   GameOver();
    }

    public void GameOver()
    {
        StartCoroutine(GameOverCor());
    }
    IEnumerator GameOverCor()
    {
        gameOverPanel.SetActive(true);
        SoundManager.instance.Play("GameOver",1,1);
        GameOverText.text = "게임 오버!";
        Color color;
        while (panel.color.a<=0.5f)
        {
            color = panel.color;
            color.a += AppearSpeed * Time.deltaTime;
            panel.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BG.Play("TextAnim");
        BrokenStar.Play("TextAnim");
        playerAnim.Play("CrownAnim");
        SoundManager.instance.Play("ClearBtnAppear",1,1);
      
        yield return new WaitForSeconds(0.5f);
        foreach (Animator anim in Btns)
        {
            anim.Play("TextAnim");
        }
        
        yield return new WaitForSeconds(0.5f);
        Text.Play("TextAnim");
        SoundManager.instance.Play("ClearBtnAppear",1,1);
    }
}
