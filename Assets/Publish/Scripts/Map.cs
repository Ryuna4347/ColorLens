using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private int lev;
    [Tooltip("별을 추가로 획득할 수 있는 이동횟수 기준 2개")] [SerializeField] private List<int> baseMoveCount;

    public List<int> GetBaseMoveCount()
    {
        return baseMoveCount;
    }
    public int GetLevel()
    {
        return lev;
    }
}
