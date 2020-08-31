using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lens : MonoBehaviour
{
    [Tooltip("오목렌즈인가?")]public bool isConcave;
    [Tooltip("렌즈가 수평으로 되어있는가?")] public bool isHorizontal;
    [Tooltip("렌즈의 좌측 또는 위쪽 부분인가?")] public bool isUpLeft;

    public Direction GetConcaveRefractDirection(int dir)
    {
        if (isConcave)
        {
            if (isUpLeft)
            {
                if (!isHorizontal)
                {
                    if (dir == (int)Direction.DOWN) //수직 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.RIGHT:
                        case (int)Direction.DOWN_RIGHT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_RIGHT;
                        case (int)Direction.LEFT:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.UP_LEFT:
                            return Direction.UP_LEFT;
                    }
                }
                else
                {
                    if (dir == (int)Direction.RIGHT) //수평 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.UP:
                        case (int)Direction.UP_LEFT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_LEFT;
                        case (int)Direction.DOWN:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.DOWN_RIGHT:
                            return Direction.DOWN_LEFT;
                    }
                }
            }
            else //오목렌즈 오른쪽
            {
                if (!isHorizontal)
                {
                    if (dir == (int)Direction.UP) //수직 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.RIGHT:
                        case (int)Direction.DOWN_RIGHT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.DOWN_RIGHT;
                        case (int)Direction.LEFT:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.UP_LEFT:
                            return Direction.DOWN_LEFT;
                    }
                }
                else
                {
                    if (dir == (int)Direction.LEFT) //수평 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.UP:
                        case (int)Direction.UP_LEFT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_RIGHT;
                        case (int)Direction.DOWN:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.DOWN_RIGHT:
                            return Direction.DOWN_RIGHT;
                    }
                }
            }
        }
        else //볼록렌즈
        {
            if (isUpLeft)
            {
                if (!isHorizontal)
                {
                    if (dir == (int)Direction.DOWN) //수직 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.RIGHT:
                        case (int)Direction.DOWN_RIGHT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.DOWN_RIGHT;
                        case (int)Direction.LEFT:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.UP_LEFT:
                            return Direction.DOWN_LEFT;
                    }
                }
                else
                {
                    if (dir == (int)Direction.RIGHT) //수평 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.UP:
                        case (int)Direction.UP_LEFT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_RIGHT;
                        case (int)Direction.DOWN:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.DOWN_RIGHT:
                            return Direction.DOWN_RIGHT;
                    }
                }
            }
            else //볼록렌즈 오른쪽()
            {
                if (!isHorizontal)
                {
                    if (dir == (int)Direction.UP) //수직 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.RIGHT:
                        case (int)Direction.DOWN_RIGHT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_RIGHT;
                        case (int)Direction.LEFT:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.UP_LEFT:
                            return Direction.UP_LEFT;
                    }
                }
                else
                {
                    if (dir == (int)Direction.LEFT) //수평 접근 체크
                    {
                        return Direction.RETURN;
                    }
                    switch (dir)
                    {
                        case (int)Direction.UP:
                        case (int)Direction.UP_LEFT:
                        case (int)Direction.UP_RIGHT:
                            return Direction.UP_LEFT;
                        case (int)Direction.DOWN:
                        case (int)Direction.DOWN_LEFT:
                        case (int)Direction.DOWN_RIGHT:
                            return Direction.DOWN_LEFT;
                    }
                }
            }
        }
        return Direction.RETURN; //딱히 나올 일 없음
    }

}
