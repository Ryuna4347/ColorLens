using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectBtn : MonoBehaviour
{
    public GameObject[] stars;
    public int stageNum;

    private void Start()
    {
        if(stageNum % 12 != 1) // 각 챕터 1스테이지의 경우 그냥 통과(2-1,3-1 등은 어차피 챕터 선택 씬에서 interactable이 false가 될 것)
        {
            if (IsClearManager.instance.GetStar(stageNum - 1)>0)
            {
                GetComponent<Button>().interactable = true;
                transform.Find("Lock").gameObject.SetActive(false);
                transform.Find("NotClear").gameObject.SetActive(true);
            }
            else
            {
                return;
            }
        }

        for (int i = 0; i < IsClearManager.instance.GetStar(stageNum); i++)
        {
            stars[i].SetActive(true);
        }
    }
}
