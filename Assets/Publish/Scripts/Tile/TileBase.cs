using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    NONE, BREAKABLE, REVERSE, ROTATE
}

public class TileBase : MonoBehaviour
{
    [SerializeField] protected TileType tileType;
    public TileType GetTileType { get { return tileType; } }

    public virtual Direction? GetNextDirection(Direction direction, int time) {
        return direction;
    }
}
