using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTile : TileBase
{
    [SerializeField]private Direction nextDirection;
    [SerializeField]private bool isClockwise;
    [SerializeField]private float rotateTime;
    [SerializeField] private GameObject arrowSpriteObj;

    //배속에 따른 회전속도 변경 추가 필요

    private void Awake()
    {
        GameManager.moveTurnEnded += Rotate;
    }

    private void Start()
    {
        arrowSpriteObj = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnDestroy()
    {
        GameManager.moveTurnEnded -= Rotate;
    }

    public override Direction? GetNextDirection(Direction direction, int time)
    {
        return nextDirection;
    }

    private IEnumerator RotateTransform()
    {
        float degree = isClockwise ? -90 : 90;
        float time = 0.01f;

        while(time < rotateTime)
        {
            yield return new WaitForSeconds(0.01f);
            arrowSpriteObj.transform.Rotate(arrowSpriteObj.transform.forward, degree / rotateTime * 0.01f);
            time += 0.01f;
        }
    }

    public void Rotate()
    {
        if (!tileType.Equals(TileType.ROTATE))
            return;
        if(isClockwise)
        {
            nextDirection += 2;
            if(nextDirection > (Direction)8)
            {
                nextDirection -= 8;
            }
        }
        else
        {
            nextDirection -= 2;
            if (nextDirection < (Direction)0)
            {
                nextDirection += 8;
            }
        }
        StartCoroutine("RotateTransform");
    }
}
