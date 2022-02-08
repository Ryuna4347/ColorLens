using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private int lev;
    [Tooltip("별을 추가로 획득할 수 있는 이동횟수 기준 2개")] [SerializeField] private List<int> baseMoveCount;
    [SerializeField] private int width;
    [SerializeField] private int height;
    public int Lev { get { return lev; } }
    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public List<int> GetBaseMoveCount()
    {
        return baseMoveCount;
    }
}
