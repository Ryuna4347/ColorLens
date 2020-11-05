using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    None, Breakable, Reverse, Rotate
}

public class TileBase : MonoBehaviour
{
    [SerializeField] protected TileType tileType;

    public virtual Direction? GetNextDirection(Direction direction, int time) {
        return direction;
    }
}
