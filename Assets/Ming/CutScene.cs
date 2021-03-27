using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 컷신이 필요한 씬에서 이미지를 불러와 1회용 컷신을 화면에 보여주기 위한 클래스
/// </summary>
public class CutScene : MonoBehaviour
{
    public GameObject cutscenePrefab;
    [SerializeField]private List<Sprite> sceneImages;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Contains("-1"))
        {
            if (PlayerPrefs.GetInt("Cut_"+SceneManager.GetActiveScene().name, 0) == 0)
            {
                PlayerPrefs.SetInt("Cut_"+SceneManager.GetActiveScene().name, 1);
                StartCoroutine(cutSceneCor());
            }
            else //한 번 컷신을 본 경우 컷신을 더 이상 보여주지 않고 바로 플레이로 진입
            {
                GameObject.Find("SetMusic").GetComponent<SetMusic>().PlayBGM();
                GameManager.instance.CheckTutorial();
            }
        }
    }

    /// <summary>
    /// 컷신을 마우스 좌클릭에 따라 순차적으로 보여주기 위한 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator cutSceneCor()
    {
        sceneImages = new List<Sprite>(Resources.LoadAll<Sprite>("CutScene/Chapter_"+ SceneManager.GetActiveScene().name.Split('-')[0]));

        GameObject cutsceneObj = GameObject.Instantiate<GameObject>(cutscenePrefab, transform);
        Image cutsceneImg = cutsceneObj.transform.GetChild(0).GetComponent<Image>();
        Text cutsceneText = cutsceneObj.transform.GetChild(1).GetComponent<Text>();

        cutsceneImg.sprite = sceneImages[0];
        for (int i = 0; i < sceneImages.Count; i++)
        {
            Color color = Color.white;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

            while (color.a > 0)
            {
                color.a -= 0.03f;

                yield return new WaitForSeconds(0.03f);

                cutsceneImg.color = color;
                if (i == sceneImages.Count - 1) //맨 마지막 컷신은 주위에 있는 검은 배경도 같이 투명화되도록
                {
                    cutsceneText.color = color;
                    cutsceneObj.GetComponent<Image>().color = new Color(0,0,0,color.a);
                }
            }

            if (i == sceneImages.Count - 1)
            {
                break;
            }

            cutsceneImg.sprite = sceneImages[i + 1];
            while (color.a < 1) //fade in
            {
                color.a += 0.03f;

                yield return new WaitForSeconds(0.03f);

                cutsceneImg.color = color;
            }
        }
        GameManager.instance.CheckTutorial();
        GameObject.Find("SetMusic").GetComponent<SetMusic>().PlayBGM();
        Destroy(cutsceneObj);
    }
}
