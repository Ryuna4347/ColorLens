using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFilter : ObjectBase
{
    [SerializeField] private string filterColor;
    [SerializeField] private List<string> filterColorComb;

    private void Awake()
    {
        List<string> colorList = new List<string> { "Red", "Blue", "Green", "Magenta", "Cyan", "Yellow", "White" };
        if(colorList.Contains(filterColor) == false)
        {
#if UNITY_EDITOR
            Debug.LogError(gameObject.name + "의 filterColor가 정확하지 않습니다.");
#endif
        }
        filterColorComb = CommonFunc.GetColorCombination(filterColor);
#if UNITY_EDITOR
        Color color = new Color(0,0,0,0.6f);
        if (filterColorComb.Contains("Red"))
            color.r = 1;
        if (filterColorComb.Contains("Green"))
            color.g = 1;
        if (filterColorComb.Contains("Blue"))
            color.b = 1;
        GetComponent<SpriteRenderer>().color = color;
#endif
    }

    /// <summary>
    /// 캐릭터와 필터의 색상이 동일해서 그냥 지나갈 수 있는가?
    /// </summary>
    /// <returns></returns>
    public bool CanCharacterPenetrate(string characColor)
    {
        return filterColor.Equals(characColor);
    }

    /// <summary>
    /// 필터에 해당되는 색을 제거하고 남은 색을 반환한다.
    /// </summary>
    /// <param name="charac"></param>
    public string FilterCharacter(string characColor)
    {
        List<string> colorCombination = CommonFunc.GetColorCombination(characColor);
        List<string> copyCombination = new List<string>(colorCombination);

        foreach (string partialColor in copyCombination)
        {
            if (!filterColorComb.Contains(partialColor))
            {
                colorCombination.Remove(partialColor);
            }
        }
        return CommonFunc.GetColorName(colorCombination);
    }
}
