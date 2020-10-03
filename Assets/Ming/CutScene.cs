using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CutScene : MonoBehaviour
{
    public Image scene;
    public Sprite[] sceneImages;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "1-1")
        {
            if (PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0) == 0)
            {
                PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, 1);
                StartCoroutine(cutSceneCor());
            }
            else //한 번 컷신을 본 경우 컷신을 더 이상 보여주지 않는다.
            {
                GameManager.instance.CheckTutorial();
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
        for (int i = sceneImages.Length - 1; i >= 0; i--)
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            color = Color.white;
            while (color.a > 0)//무한반복
            {
                color = imgs[i].color;
                color.a -= 0.01f;
                imgs[i].color = color;

                if(i == 0) //맨 마지막 컷신은 주위에 있는 검은 배경도 같이 투명화되도록
                {
                    foreach(Transform background in imgs[i].gameObject.transform)
                    {
                        background.gameObject.GetComponent<Image>().color = new Color(0,0,0,color.a);
                    }
                }

                yield return new WaitForSeconds(0.01f); //0.2초의 간격을 두고 실행됨  
            }
            imgs[i].gameObject.SetActive(false);
        }
        GameManager.instance.CheckTutorial();
    }
}
