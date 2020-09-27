using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
            Clear(3,10);
    }

    public void Clear(int Stars,int walkCount)
    {
        StartCoroutine(ClearCor(Stars,walkCount));
    }
    IEnumerator ClearCor(int Stars,int walkCount)
    {
        clearPanel.SetActive(true);
        SoundManager.instance.Play("Clear",1,1);
        ClearSave.instance.Save(Stars);
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
        for (int i = 0; i < Stars; i++)
        {
            starAnims[i].Play("StarAnim");
            SoundManager.instance.Play("StarAppear",1,1f);
            yield return new WaitForSeconds(0.3f);
        }

        if (Stars == 3)
        {
            yield return new WaitForSeconds(0.2f);
            crownAnim.Play("CrownAnim");
            SoundManager.instance.Play("StarAppear",1,1.5f);
            yield return new WaitForSeconds(0.5f);
        }

        foreach (Animator anim in Btns)
        {
            anim.Play("TextAnim");
        }
    }
}
