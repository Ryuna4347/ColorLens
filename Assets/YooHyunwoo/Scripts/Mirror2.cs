using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror2 : MonoBehaviour
{
    [Tooltip("대각거울이 역슬래시처럼 되어있는가?")]public bool isReflected;

    public Direction GetMirrorReflectDirection(int dir)
    {
        if (!isReflected)
        {
            switch (dir)
            {
                case (int)Direction.UP:
                    return Direction.RIGHT;
                case (int)Direction.DOWN:
                    return Direction.LEFT;
                case (int)Direction.LEFT:
                    return Direction.DOWN;
                case (int)Direction.RIGHT:
                    return Direction.UP;
                case (int)Direction.DOWN_RIGHT:
                    return Direction.UP_LEFT;
                case (int)Direction.UP_LEFT:
                    return Direction.DOWN_RIGHT;
                case (int)Direction.DOWN_LEFT:
                case (int)Direction.UP_RIGHT:
                    return Direction.RETURN;
            }
        }
        else
        {
            switch (dir)
            {
                case (int)Direction.UP:
                    return Direction.LEFT;
                case (int)Direction.DOWN:
                    return Direction.RIGHT;
                case (int)Direction.LEFT:
                    return Direction.UP;
                case (int)Direction.RIGHT:
                    return Direction.DOWN;
                case (int)Direction.DOWN_LEFT:
                    return Direction.UP_RIGHT;
                case (int)Direction.UP_RIGHT:
                    return Direction.DOWN_LEFT;
                case (int)Direction.DOWN_RIGHT:
                case (int)Direction.UP_LEFT:
                    return Direction.RETURN;
            }
        }
        return Direction.RETURN;
    }
}
