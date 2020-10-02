using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public List<string> unlockStageNames; //튜토리얼 페이지들이 요구하는 최저 클리어 스테이지 명
    public List<GameObject> buttons; //튜토리얼 앞/뒤 이동버튼
    public List<GameObject> pageList;
    private int pageIdx = 1; //현재 튜토리얼 페이지 번호(1부터 시작)

    private void Awake()
    {
        pageList = new List<GameObject>();
        foreach(Transform page in transform.Find("Pages"))
        {
            pageList.Add(page.gameObject);
        }

        UpdatePage(1);
    }

    private void UpdatePage(int page)
    {
        if(page == 1)
        {
            buttons[0].SetActive(false);
        }
        else
        {
            buttons[0].SetActive(true);
            if(page == 10)
            {
                buttons[1].SetActive(false);
            }
            else
            {
                buttons[1].SetActive(true);
            }
        }

        if(unlockStageNames[page]=="" || PlayerPrefs.GetInt(unlockStageNames[page],0)>0) // 다음 도움말을 볼 수 있는 스테이지까지 올라간 경우
        {
            buttons[1].GetComponent<Button>().interactable = true;
        }
        else
        {
            buttons[1].GetComponent<Button>().interactable = false;
        }

        pageList[pageIdx - 1].SetActive(false);
        pageList.Find(x => x.name.Equals("Page_" + page)).SetActive(true);
        pageIdx = page;

        Animator pageAnimator = pageList[pageIdx - 1].GetComponentInChildren<Animator>();
        if(pageAnimator != null)
        {
            pageAnimator.SetInteger("page", page);
        }
    }

    public void ShowPrevPage()
    {
        UpdatePage(pageIdx - 1);
    }
    public void ShowNextPage()
    {
        UpdatePage(pageIdx + 1);
    }
}
