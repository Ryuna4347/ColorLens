using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsClearManager : MonoBehaviour
{
    public static IsClearManager instance;
    
 
    public int[] stars;
    void Awake()
    {
        instance = this;
        for (int i = 0; i < ClearSave.instance.stageKeys.Length; i++)
        {
            stars[i] = PlayerPrefs.GetInt(ClearSave.instance.stageKeys[i], 0);
        }
    }

    public int GetStar(int stageNum)
    {
        stageNum--;
        for (int i = 0; i < ClearSave.instance.stageKeys.Length; i++)
        {
            if (stageNum == i)
                return stars[i];
        }
        return 0;
    }
}
