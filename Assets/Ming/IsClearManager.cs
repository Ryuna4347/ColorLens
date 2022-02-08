using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsClearManager : MonoBehaviour
{
    public static IsClearManager instance;
    public Dictionary<int,int> stars; //스테이지 번호, 해당 스테이지 별 수
    [Tooltip("현재 구현된 최대 챕터")]public int maximumChapter;
    [Tooltip("한 챕터 당 스테이지 수")]public int stagesPerChapter;
    [Tooltip("전체 완료시 최종 컷신 프리팹")] public GameObject finalCutScene;

    void Awake()
    {
        instance = this;
        stars = new Dictionary<int, int>();
        for (int i = 1; i <= maximumChapter; i++)
        {
            for(int j = 1; j <= stagesPerChapter; j++)
            {
                stars.Add(12 * (i - 1) + j, PlayerPrefs.GetInt(i + "-" + j, 0));
            }
        }
    }

    public int GetStar(int stageNum)
    {
        return stars[stageNum];
    }
}
