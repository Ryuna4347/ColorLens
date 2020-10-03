﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public Image scene;
    public Sprite[] sceneImages;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "1-1" || SceneManager.GetActiveScene().name == "2-1" ||
            SceneManager.GetActiveScene().name == "3-1" || SceneManager.GetActiveScene().name == "4-1")
        {
           if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name,0) == 0)
                 {
                     PlayerPrefs.SetInt(SceneManager.GetActiveScene().name,1);
                     StartCoroutine(cutSceneCor());
                 }   
        }
    }

    IEnumerator cutSceneCor()
    {
        List<Image> imgs=new List<Image>();
        for (int i = sceneImages.Length-1; i >=0; i--)
        {
            imgs.Add(Instantiate(scene,transform));
            imgs[sceneImages.Length-1-i].sprite = sceneImages[i];
        }
        Color color=Color.white;
        for (int i = sceneImages.Length-1; i >=0; i--)
        {
            yield return new WaitUntil(()=>Input.GetMouseButtonDown(0));
            color=Color.white;
            while (color.a>0)//무한반복
            {
                color = imgs[i].color;
                color.a -= 0.01f;
                imgs[i].color = color;
                yield return new WaitForSeconds(0.01f); //0.2초의 간격을 두고 실행됨  
            }

            imgs[i].gameObject.SetActive(false);
        }
        //FindObjectOfType<Pause>().CanPuse = true;
    }
}
