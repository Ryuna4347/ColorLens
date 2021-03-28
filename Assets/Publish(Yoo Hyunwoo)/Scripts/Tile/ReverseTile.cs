using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseTile : TileBase
{
    public override Direction? GetNextDirection(Direction direction, int time)
    {
        if (time != 0) //출발 이외의 방향은 원래 이동방향대로 움직인다.
        {
            return direction;
        }
        else //출발 방향은 반대가 된다.
        {
            return (int)direction > 4 ? direction - 4 : direction + 4;
        }
    }
}
