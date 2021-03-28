using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : ObjectBase
{
    [Tooltip("대각거울이 역슬래시처럼 되어있는가?")]public bool isReflected;

    public Direction GetMirrorReflectDirection(Direction dir)
    {
        if (!isReflected)
        {
            switch (dir)
            {
                case Direction.UP:
                    return Direction.RIGHT;
                case Direction.DOWN:
                    return Direction.LEFT;
                case Direction.LEFT:
                    return Direction.DOWN;
                case Direction.RIGHT:
                    return Direction.UP;
                case Direction.DOWN_RIGHT:
                    return Direction.UP_LEFT;
                case Direction.UP_LEFT:
                    return Direction.DOWN_RIGHT;
                case Direction.DOWN_LEFT:
                case Direction.UP_RIGHT:
                    return Direction.RETURN;
            }
        }
        else
        {
            switch (dir)
            {
                case Direction.UP:
                    return Direction.LEFT;
                case Direction.DOWN:
                    return Direction.RIGHT;
                case Direction.LEFT:
                    return Direction.UP;
                case Direction.RIGHT:
                    return Direction.DOWN;
                case Direction.DOWN_LEFT:
                    return Direction.UP_RIGHT;
                case Direction.UP_RIGHT:
                    return Direction.DOWN_LEFT;
                case Direction.DOWN_RIGHT:
                case Direction.UP_LEFT:
                    return Direction.RETURN;
            }
        }
        return Direction.RETURN;
    }
}
