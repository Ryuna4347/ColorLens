using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePage : MonoBehaviour
{
  public GameObject[] pages;
  public int index = 0;
  public GameObject leftBtn, rightBtn;
  private void Update()
  {
    for (int i = 0; i < pages.Length; i++)
    {
      if (i == index)
        pages[i].SetActive(true);
      else
        pages[i].SetActive(false);
    }

    if (index == 0)
    {
      leftBtn.SetActive(false);
      rightBtn.SetActive(true);
    }

    if (index == pages.Length-1)
    {
      rightBtn.SetActive(false);
      leftBtn.SetActive(true);
    }
  }

  public void left()
  {
    index--;
  }

  public void right()
  {
    index++;
  }
}
