using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectBtn : MonoBehaviour
{
    public GameObject[] stars;
    public int stageNum;

    private void Start()
    {
        for (int i = 0; i < IsClearManager.instance.GetStar(stageNum); i++)
        {
            stars[i].SetActive(true);
        }
    }
}
