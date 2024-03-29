﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 목표점에 대한 스크립트
/// </summary>
public class Objective2 : MonoBehaviour
{
    [SerializeField] private string colorNow;
    [SerializeField] private Sprite completeSprite;
    private List<string> colorCombination; //colorNow 색상 안에 들어가는 3원색 리스트
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        colorCombination= CommonFunc.GetColorCombination(colorNow);
        RefreshObjectiveColor();
    }

    private void RefreshObjectiveColor()
    {
        colorNow = CommonFunc.GetColorName(colorCombination); //현재 남아있는 색상 조합은 무슨 색인지 갱신
        Color newColor = new Color(0, 0, 0, 1);
        newColor.r = colorCombination.Contains("Red") ? 1 : 0;
        newColor.g = colorCombination.Contains("Green") ? 1 : 0;
        newColor.b = colorCombination.Contains("Blue") ? 1 : 0;
        spriteRenderer.color = newColor;
    }

    /// <summary>
    /// 상자
    /// </summary>
    /// <param name="colorName"></param>
    /// <returns></returns>
    public bool CheckColor(string colorName)
    {
        List<string> compareColorList = CommonFunc.GetColorCombination(colorName);

        if(colorNow.Equals(colorName))
        {
            return true;
        }
        else
        {
            foreach(string compareColor in compareColorList)
            {
                if(colorCombination.Contains(compareColor))
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 현재 색상에서 캐릭터의 색상을 제외한다.(같은 경우는 목표점도 아예 변경)
    /// </summary>
    public void EraseColor(string colorName)
    {
        List<string> compareColorList = CommonFunc.GetColorCombination(colorName);

        if (colorNow.Equals(colorName))
        {
            CompleteObjective();
        }
        else
        {
            foreach (string compareColor in compareColorList)
            {
                if(colorCombination.Contains(compareColor)) //겹치는 색이 있으면 제거
                {
                    colorCombination.Remove(compareColor);
                }

                if(colorCombination.Count == 0)
                {
                    CompleteObjective();
                    return;
                }
            }
            RefreshObjectiveColor();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="objective"></param>
    /// <returns></returns>
    private void CompleteObjective()
    {
        spriteRenderer.sprite = completeSprite;

        GetComponent<Collider2D>().enabled = false; //이후 충돌 금지
        GameManager.instance.ControlColorBoxCount();
    }
}
