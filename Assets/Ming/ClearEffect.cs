using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClearEffect : MonoBehaviour
{
    public static ClearEffect instance;
    
    public Image panel;
    public float AppearSpeed;
    public Animator[] starAnims;
    public Animator crownAnim;
    public Animator playerAnim;
    public Animator textAnim;
    public Animator[] Btns;
    public Text walkCountText;
    public Animator BG;
    public GameObject clearPanel;

    public int stars;
    public int walkCount;

    private void Awake()
    {
        instance = this;
    }

    public void Clear()
    {
        StartCoroutine(ClearCor());
    }

    public void Clear(int star, int count)
    {
        stars = star;
        walkCount = count;
        StartCoroutine(ClearCor());
    }
    IEnumerator ClearCor()
    {
        clearPanel.SetActive(true);
        SoundManager.instance.Play("Clear",1,1);
        ClearSave.instance.Save(stars);
        walkCountText.text = walkCount+"번의 이동횟수로 클리어!";
        Color color;
        while (panel.color.a<=0.5f)
        {
            color = panel.color;
            color.a += AppearSpeed * Time.deltaTime;
            panel.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        BG.Play("TextAnim");
        playerAnim.Play("CrownAnim");
        textAnim.Play("TextAnim");
        SoundManager.instance.Play("ClearBtnAppear",1,1);
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < stars; i++)
        {
            starAnims[i].Play("StarAnim");
            SoundManager.instance.Play("StarAppear",1,1f);
            yield return new WaitForSeconds(0.3f);
        }

        if (stars == 3)
        {
            yield return new WaitForSeconds(0.2f);
            crownAnim.Play("CrownAnim");
            SoundManager.instance.Play("StarAppear",1,1.5f);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (Animator anim in Btns)
        {
            if (anim != null)
                anim.Play("TextAnim");
        }

        if(GameManager.instance.SceneName.Equals("7-12"))
        {
            clearPanel.transform.Find("NextButton").GetComponent<Button>().enabled = false;
        }
        yield return new WaitForSeconds(0.4f);
        AdmobManager.instance.ShowFrontAd();
    }
}
