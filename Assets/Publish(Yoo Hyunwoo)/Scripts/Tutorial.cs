using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public bool isDev;
    public List<string> unlockStageNames; //튜토리얼 페이지들이 요구하는 최저 클리어 스테이지 명
    public List<GameObject> buttons; //튜토리얼 앞/뒤 이동버튼
    public List<GameObject> pageList;
    public List<int> ingameAnimation = new List<int>(); //인게임에서 애니메이션 트리거를 조절하기 위함(트리거의 page_ 뒷 번호를 작성. 메인 타이틀 씬에서는 절대 건들지 말 것)
    private int pageIdx = 1; //현재 튜토리얼 페이지 번호(1부터 시작)

    private void Awake()
    {
        pageList = new List<GameObject>();
        foreach(Transform page in transform.Find("Pages"))
        {
            pageList.Add(page.gameObject);
        }
    }

    private void OnEnable()
    {
        UpdatePage(1);
    }

    private void OnDisable()
    {
        Animator pageAnimator = pageList[pageIdx - 1].GetComponentInChildren<Animator>();

        //pageAnimator.SetInteger("page", 0);
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
        }

        if (page == pageList.Count)
        {
            buttons[1].SetActive(false);
        }
        else
        {
            buttons[1].SetActive(true);
        }

        if (buttons[1].activeSelf == true)
        {
            if (isDev || (unlockStageNames[page] == "" || PlayerPrefs.GetInt(unlockStageNames[page], 0) > 0)) // 다음 도움말을 볼 수 있는 스테이지까지 올라간 경우
            {
                buttons[1].GetComponent<Button>().interactable = true;
            }
            else
            {
                buttons[1].GetComponent<Button>().interactable = false;
                buttons[1].GetComponent<TutorialNextButton>().lockedStageName = unlockStageNames[page];
            }
        }

        pageList[pageIdx - 1].SetActive(false);
        pageList.Find(x => x.name.Equals("Page_" + page)).SetActive(true);
        pageIdx = page;

        Animator pageAnimator = pageList[pageIdx - 1].GetComponentInChildren<Animator>();
        if (pageAnimator != null)
        {
            if (ingameAnimation.Count == 0)
            {
                pageAnimator.SetTrigger("page_" + page);
            }
            else //인게임에서는 1페이지부터 시작이어서 page로는 원래 다른 페이지의 애니메이션이 실행이 안된다.
            {
                if (ingameAnimation[page - 1] > 0)
                {
                    pageAnimator.SetTrigger("page_" + ingameAnimation[page - 1]);
                }
            }
        }
    }

    public void ShowPrevPage()
    {
        UpdatePage(pageIdx - 1);
        SoundManager.instance.Play("Btn");
    }
    public void ShowNextPage()
    {
        UpdatePage(pageIdx + 1);
        SoundManager.instance.Play("Btn");
    }
    public void CloseUI()
    {
        SoundManager.instance.Play("Back");
        transform.parent.gameObject.SetActive(false);
    }
}
